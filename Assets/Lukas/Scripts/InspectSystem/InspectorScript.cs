using Cinemachine;
using Scripts.InspectSystem;
using UnityEngine;
using UnityEngine.InputSystem;

public class InspectorScript : MonoBehaviour
    {
        [SerializeField] Inspectable inspectPrefab;
        [SerializeField] Transform holder;
        [SerializeField] PlayerInput playerInput;
        [SerializeField] CanvasGroup inspectorGroup;
        [SerializeField] CinemachineBrain cinemachineBrain;
        bool isRotating;
        
        public void SetInspectPrefab(Inspectable _inspectPrefab)
        {
            var mainCam = Camera.main;
            inspectPrefab = Instantiate(_inspectPrefab, holder);
            Debug.Log(Camera.main);
            ObjectScaler.FitObjectInView(Camera.main, holder, inspectPrefab.gameObject, 0.6f, inspectPrefab.VisualScalar);
            if (inspectPrefab.transform.forward != mainCam.transform.forward)
            {
                var visualForward = inspectPrefab.transform.up;
                var correction = Quaternion.FromToRotation(visualForward, -mainCam.transform.forward);
                inspectPrefab.transform.localRotation = correction * inspectPrefab.transform.localRotation;
            }
        }

        public void OpenInspectUI()
        {
            playerInput.SwitchCurrentActionMap("InspectMap");
            inspectorGroup.gameObject.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            
            inspectPrefab.CurrentRotation = inspectPrefab.transform.rotation;
        }

        public void CloseInspectUI(InputAction.CallbackContext _context)
        {
            inspectorGroup.gameObject.SetActive(false);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            playerInput.SwitchCurrentActionMap("Player");
            Destroy(inspectPrefab.gameObject);
        }

        public void OnLeftClick(InputAction.CallbackContext _context)
        {
            if (_context.started)
                isRotating = true;

            if (_context.canceled)
            {
                isRotating = false;
                inspectPrefab.LastMousePosition = null;
            }
        }

        public void OnMousePosition(InputAction.CallbackContext _context)
        {
            if (!isRotating)
                return;

            var currMousePos = _context.ReadValue<Vector2>();
            var currPos = GetTrackballVector(currMousePos);

            if (inspectPrefab.LastMousePosition.HasValue)
            {
                var lastPos = GetTrackballVector(inspectPrefab.LastMousePosition.Value);
                var axis = Vector3.Cross(lastPos, currPos);
                float angle = Vector3.Angle(lastPos, currPos);

                var rot = Quaternion.AngleAxis(angle * inspectPrefab.RotationSpeed, axis);
                inspectPrefab.CurrentRotation = rot * inspectPrefab.CurrentRotation;
                inspectPrefab.transform.rotation = inspectPrefab.CurrentRotation;
            }

            inspectPrefab.LastMousePosition = currMousePos;
        }

        Vector3 GetTrackballVector(Vector2 _mousePos)
        {
            float x = (2f * _mousePos.x - Screen.width) / Screen.width;
            float y = (2f * _mousePos.y - Screen.height) / Screen.height;
            var v = new Vector3(x, y, 0f);

            float lengthSquared = x * x + y * y;
            if (lengthSquared <= 1f)
                v.z = Mathf.Sqrt(1f - lengthSquared);
            else
                v.Normalize();

            return v;
        }
    }
