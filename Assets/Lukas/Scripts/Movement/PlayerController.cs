using System;
using Cinemachine;
using Scripts.MinigameSystem.Memory;
using Scripts.Scriptables.SceneLoader;
using Scripts.Scriptables.Settings;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Scripts.Movement
{
    public class PlayerController : MonoBehaviour
    {
        Vector2 playerLook;
        Vector2 moveInput;
        Rigidbody playerRigidbody;

        float xRotation;

        [Header("Game Settings")]
        [SerializeField] SettingsSO settings;

        [SerializeField] SceneLoader pauseMenuLoader;

        [Header("Camera Settings")]
        [SerializeField] bool isMainHub;

        [SerializeField] Camera mainUnityCamera;

        [SerializeField] CinemachineBrain cinemachineBrain;

        [SerializeField] CinemachineVirtualCamera firstPersonCamera;
        [SerializeField] CinemachineVirtualCamera thirdPersonCamera;
        [SerializeField] [Range(0, 90)] float cameraClampAngle;
        [SerializeField] float lookSensitivity = 2.0f;
        CinemachineInputProvider cinemachineInputProvider;

        [Header("Render Layer Settings")]
        [SerializeField] LayerMask firstPersonCullingMask;

        [SerializeField] LayerMask thirdPersonCullingMask;

        [Header("Collision Based Settings")]
        [SerializeField] LayerMask firstPersonBlockingMask;

        [SerializeField] LayerMask thirdPersonBlockingMask;

        [Header("Movement Settings")]
        [SerializeField] float moveSpeed = 1f;

        [SerializeField] float rotationTime = 0.1f;
        float turnVelocity;

        [Header("MemoryGame Specific")]
        [SerializeField] LayerMask memoryCardLayer;

        [Header("Animation Settings")]
        [SerializeField] Animator animator;

        SwapBlocker swapBlocker;

        public Action<float> OnScroll;

        public bool IsPaused;

        bool isMoving;
        bool isTurning;

        void Awake()
        {
            swapBlocker = new SwapBlocker();
            playerRigidbody = GetComponent<Rigidbody>();
            Cursor.lockState = CursorLockMode.Locked;
            if (isMainHub) return;
            firstPersonCamera.MoveToTopOfPrioritySubqueue();
            mainUnityCamera.cullingMask = firstPersonCullingMask;
            cinemachineInputProvider = thirdPersonCamera.GetComponent<CinemachineInputProvider>();
            cinemachineInputProvider.enabled = false;
        }

        void Start()
        {
            if (isMainHub) return;
            firstPersonCamera.MoveToTopOfPrioritySubqueue();
            cinemachineBrain.ManualUpdate();
        }

        void FixedUpdate()
        {
            DoMove();
            if (!isTurning && playerRigidbody.velocity.magnitude > 0.1f)
            {
                animator.SetBool("IsWalking", true);
            }
        }

        public void FP_Look(InputAction.CallbackContext _callbackContext)
        {
            if (IsPaused) return;
            if (!isMainHub)
            {
                if ((CinemachineVirtualCamera)cinemachineBrain.ActiveVirtualCamera != firstPersonCamera || cinemachineBrain.IsBlending) return;
            }

            playerLook = _callbackContext.ReadValue<Vector2>();
            float lookX = playerLook.x * (lookSensitivity * settings.MouseSensitivity);
            float lookY = playerLook.y * (lookSensitivity * settings.MouseSensitivity);

            xRotation -= lookY;
            xRotation = Mathf.Clamp(xRotation, -cameraClampAngle, cameraClampAngle);

            if (isMainHub)
            {
                mainUnityCamera.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
            }
            else
            {
                firstPersonCamera.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
            }


            transform.Rotate(Vector3.up * lookX);
        }

        public void TP_Look(InputAction.CallbackContext _callbackContext)
        {
            if (isMainHub || IsPaused) return;
            cinemachineInputProvider.enabled = _callbackContext.phase switch
            {
                InputActionPhase.Started => true,
                InputActionPhase.Canceled => false,
                _ => cinemachineInputProvider.enabled
            };
        }

        public void Move(InputAction.CallbackContext _callbackContext)
        {
            if (_callbackContext.phase == InputActionPhase.Canceled)
            {
                animator.SetBool("IsWalking", false);
            }

            moveInput = _callbackContext.ReadValue<Vector2>();
            if (!IsThirdPersonActive()) return;
            if (moveInput.sqrMagnitude < 0.01f) return;

            var input = new Vector3(moveInput.x, 0f, moveInput.y);
            var moveDirection = GetMoveDirection(input);

            if (playerRigidbody.velocity.sqrMagnitude < 0.01f)
            {
                float angle = Vector3.SignedAngle(transform.forward, moveDirection, Vector3.up);

                switch (angle)
                {
                    case > 45f and < 135f:
                        animator.SetTrigger("TurnR");
                        isTurning = true;
                        animator.SetBool("IsWalking", false);
                        return;
                    case < -45f and > -135f:
                        animator.SetTrigger("TurnL");
                        isTurning = true;
                        animator.SetBool("IsWalking", false);
                        return;
                    default:
                    {
                        if (Mathf.Abs(angle) >= 135f)
                        {
                            animator.SetTrigger("TurnAround");
                            isTurning = true;
                            animator.SetBool("IsWalking", false);
                            return;
                        }

                        break;
                    }
                }
            }

            if (isTurning)
            {
                animator.SetBool("IsWalking", false);
                return;
            }

            animator.SetBool("IsWalking", moveInput.magnitude > 0.1f);
        }

        public void EndIsTurning()
        {
            isTurning = false;
        }

        void DoMove()
        {
            if (IsPaused) return;
            if (!isMainHub)
            {
                if (cinemachineBrain.IsBlending || moveInput.sqrMagnitude < 0.01f)
                {
                    StopHorizontalMovement();
                    return;
                }
            }

            var input = new Vector3(moveInput.x, 0, moveInput.y);
            var moveDirection = GetMoveDirection(input);
            if (isTurning) return;
            ApplyMovementVelocity(moveDirection);
            if (isMainHub) return;
            if (IsThirdPersonActive())
            {
                RotateTowards(moveDirection);
            }
        }

        void StopHorizontalMovement()
        {
            var velocity = playerRigidbody.velocity;
            playerRigidbody.velocity = new Vector3(0, velocity.y, 0);
        }

        Vector3 GetMoveDirection(Vector3 _input)
        {
            if (isMainHub || !IsThirdPersonActive()) return transform.TransformDirection(_input).normalized;
            var camTransform = thirdPersonCamera.transform;
            var forward = Vector3.ProjectOnPlane(camTransform.forward, Vector3.up).normalized;
            var right = Vector3.ProjectOnPlane(camTransform.right, Vector3.up).normalized;
            var moveDirection = forward * _input.z + right * _input.x;

            return moveDirection.normalized;
        }

        void ApplyMovementVelocity(Vector3 _moveDirection)
        {
            var velocity = _moveDirection * moveSpeed;
            velocity.y = playerRigidbody.velocity.y;

            Debug.DrawRay(transform.position, _moveDirection * 2, Color.red, 0.1f);

            playerRigidbody.velocity = velocity;

            animator.ResetTrigger("TurnL");
            animator.ResetTrigger("TurnR");
        }

        void OnAnimatorMove()
        {
            if (!animator) return;
            transform.rotation *= animator.deltaRotation;
        }

        bool IsThirdPersonActive()
        {
            if (isMainHub) return false;
            return (CinemachineVirtualCamera)cinemachineBrain.ActiveVirtualCamera == thirdPersonCamera;
        }

        void RotateTowards(Vector3 _moveDirection)
        {
            float currentY = transform.eulerAngles.y;
            float targetY = Mathf.Atan2(_moveDirection.x, _moveDirection.z) * Mathf.Rad2Deg;
            float smoothY = Mathf.SmoothDampAngle(currentY, targetY, ref turnVelocity, rotationTime);
            transform.rotation = Quaternion.Euler(0f, smoothY, 0f);
        }

        public void SwapCamera(InputAction.CallbackContext _callbackContext)
        {
            if (isMainHub) return;
            if (_callbackContext.phase != InputActionPhase.Started) return;
            if ((CinemachineVirtualCamera)cinemachineBrain.ActiveVirtualCamera == firstPersonCamera && !swapBlocker.GetSwapBlocked(transform, thirdPersonBlockingMask))
            {
                thirdPersonCamera.transform.LookAt(transform.forward);
                thirdPersonCamera.MoveToTopOfPrioritySubqueue();
                mainUnityCamera.cullingMask = thirdPersonCullingMask;
                Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("FirstPersonOnly"), true);
                Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("ThirdPersonOnly"), false);
                cinemachineInputProvider.enabled = true;
            }
            else if ((CinemachineVirtualCamera)cinemachineBrain.ActiveVirtualCamera == thirdPersonCamera && !swapBlocker.GetSwapBlocked(transform, firstPersonBlockingMask))
            {
                firstPersonCamera.MoveToTopOfPrioritySubqueue();
                firstPersonCamera.transform.forward = transform.forward;
                mainUnityCamera.cullingMask = firstPersonCullingMask;
                Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("ThirdPersonOnly"), true);
                Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("FirstPersonOnly"), false);
                cinemachineInputProvider.enabled = false;
            }
        }

        public void MemoryGameClick(InputAction.CallbackContext _callbackContext)
        {
            if (_callbackContext.phase != InputActionPhase.Started) return;
            Physics.Raycast(mainUnityCamera.ScreenPointToRay(Input.mousePosition), out var hit, memoryCardLayer);
            Debug.Log(hit.collider.gameObject.name);
            var card = hit.collider.gameObject.GetComponentInParent<MemoryCard>();
            if (card == null) return;
            card.Select();
        }

        public void RotateFlower(InputAction.CallbackContext _context)
        {
            float yDelta = _context.ReadValue<Vector2>().y;
            OnScroll?.Invoke(yDelta);
        }

        public void PauseGame(InputAction.CallbackContext _context)
        {
            if (_context.phase != InputActionPhase.Started) return;
            if (IsPaused) return;
            IsPaused = true;
            cinemachineInputProvider.enabled = false;
            pauseMenuLoader.LoadSceneAdditive();
        }

        public void Unpause()
        {
            if (!IsPaused) return;
            IsPaused = false;
            if (cinemachineInputProvider == null) return;
            cinemachineInputProvider.enabled = true;
        }
    }
}