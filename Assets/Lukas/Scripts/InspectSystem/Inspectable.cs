using System;
using Scripts.InteractionSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Scripts.InspectSystem
{
    public class Inspectable : MonoBehaviour, IInteractable
    {
        public float RotationSpeed = 5f;

        public Quaternion CurrentRotation;
        public Vector3? LastMousePosition = null;

        [SerializeField] public float VisualScalar = 6f;

        bool gotInspected;

        void Inspect()
        {
            var inspectorScript = FindObjectOfType<InspectorScript>();
            inspectorScript.SetInspectPrefab(this);
            inspectorScript.OpenInspectUI();
        }

        public void Interact()
        {
            if (gotInspected) return;
            Inspect();
            gotInspected = true;
        }

        public void ShowInteractIcon()
        {
            Debug.Log("No InteractIcon Logic! This might be okay!");
        }

        public void HideInteractIcon()
        {
            Debug.Log("No InteractIcon Logic! This might be okay!");
        }


        // void Start()
        // {
        //     CurrentRotation = transform.rotation;
        //     Cursor.lockState = CursorLockMode.None;
        //     Cursor.visible = true;
        // }
//
        // void Update()
        // {
        //     if (Mouse.current.leftButton.isPressed)
        //     {
        //         var mousePosition = Mouse.current.position.ReadValue();
        //         var currPos = GetTrackballVector(mousePosition);
//
        //         if (LastMousePosition.HasValue)
        //         {
        //             var lastPos = GetTrackballVector(LastMousePosition.Value);
        //             var axis = Vector3.Cross(lastPos, currPos);
        //             float angle = Vector3.Angle(lastPos, currPos);
//
        //             var rotation = Quaternion.AngleAxis(angle * RotationSpeed, axis);
        //             CurrentRotation = rotation * CurrentRotation;
        //             transform.rotation = CurrentRotation;
        //         }
//
        //         LastMousePosition = mousePosition;
        //     }
        //     else
        //     {
        //         LastMousePosition = null;
        //     }
        // }
//
        // Vector3 GetTrackballVector(Vector2 _mousePosition)
        // {
        //     float x = (2f * _mousePosition.x - Screen.width) / Screen.width;
        //     float y = (2f * _mousePosition.y - Screen.height) / Screen.height;
        //     var normalizedVector = new Vector3(x, y, 0);
//
        //     float squareLength = x * x + y * y;
        //     if (squareLength <= 1f)
        //     {
        //         normalizedVector.z = Mathf.Sqrt(1f - squareLength);
        //     }
        //     else
        //     {
        //         normalizedVector.Normalize();
        //     }
//
        //     return normalizedVector;
        // }
    }
}