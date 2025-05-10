using System;
using System.Collections;
using Cinemachine;
using Lukas.Scripts;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    Vector2 playerLook;
    Vector2 moveInput;
    Rigidbody playerRigidbody;

    float xRotation;

    [Header("Camera Settings")]
    [SerializeField] Camera mainUnityCamera;

    [SerializeField] CinemachineBrain cinemachineBrain;

    [SerializeField] CinemachineVirtualCamera firstPersonCamera;
    [SerializeField] CinemachineVirtualCamera thirdPersonCamera;
    [SerializeField] [Range(0, 90)] float cameraClampAngle;
    [SerializeField] float lookSensitivity = 2.0f;

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

    SwapBlocker swapBlocker;

    void Awake()
    {
        swapBlocker = new SwapBlocker();
        playerRigidbody = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        firstPersonCamera.MoveToTopOfPrioritySubqueue();
        mainUnityCamera.cullingMask = firstPersonCullingMask;
    }

    void FixedUpdate()
    {
        DoMove();
    }

    public void FP_Look(InputAction.CallbackContext _callbackContext)
    {
        if ((CinemachineVirtualCamera)cinemachineBrain.ActiveVirtualCamera != firstPersonCamera || cinemachineBrain.IsBlending) return;
        playerLook = _callbackContext.ReadValue<Vector2>();
        float lookX = playerLook.x * lookSensitivity;
        float lookY = playerLook.y * lookSensitivity;

        xRotation -= lookY;
        xRotation = Mathf.Clamp(xRotation, -cameraClampAngle, cameraClampAngle);

        firstPersonCamera.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);

        transform.Rotate(Vector3.up * lookX);
    }

    public void Move(InputAction.CallbackContext _callbackContext)
    {
        moveInput = _callbackContext.ReadValue<Vector2>();
    }
    void DoMove()
    {
        if (cinemachineBrain.IsBlending || moveInput.sqrMagnitude < 0.01f)
        {
            StopHorizontalMovement();
            return;
        }

        var input = new Vector3(moveInput.x, 0, moveInput.y);
        var moveDirection = GetMoveDirection(input);
        ApplyMovementVelocity(moveDirection);

        if (IsThirdPersonActive() && ShouldRotate(moveDirection))
        {
            RotateTowards(moveDirection);
        }
    }

    void StopHorizontalMovement()
    {
        var v = playerRigidbody.velocity;
        playerRigidbody.velocity = new Vector3(0, v.y, 0);
    }

    Vector3 GetMoveDirection(Vector3 _input)
    {
        if (!IsThirdPersonActive()) return transform.TransformDirection(_input).normalized;
        var camFollow = thirdPersonCamera.Follow;
        var forward = Vector3.Scale(camFollow.forward, new Vector3(1, 0, 1)).normalized;
        var right = Vector3.Scale(camFollow.right, new Vector3(1, 0, 1)).normalized;
        return (forward * _input.z + right * _input.x).normalized;
    }

    void ApplyMovementVelocity(Vector3 _moveDirection)
    {
        var velocity = _moveDirection * moveSpeed;
        velocity.y = playerRigidbody.velocity.y;
        playerRigidbody.velocity = velocity;
    }

    bool IsThirdPersonActive()
    {
        return (CinemachineVirtualCamera)cinemachineBrain.ActiveVirtualCamera == thirdPersonCamera;
    }

    bool ShouldRotate(Vector3 _moveDirection)
    {
        float alignment = Vector3.Dot(transform.forward, _moveDirection);
        return alignment > -0.2f;
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
        if (_callbackContext.phase != InputActionPhase.Started) return;
        if ((CinemachineVirtualCamera)cinemachineBrain.ActiveVirtualCamera == firstPersonCamera && !swapBlocker.GetSwapBlocked(transform, thirdPersonBlockingMask))
        {
            thirdPersonCamera.MoveToTopOfPrioritySubqueue();
            mainUnityCamera.cullingMask = thirdPersonCullingMask;
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("FirstPersonOnly"), true);
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("ThirdPersonOnly"), false);
        }
        else if ((CinemachineVirtualCamera)cinemachineBrain.ActiveVirtualCamera == thirdPersonCamera && !swapBlocker.GetSwapBlocked(transform, firstPersonBlockingMask))
        {
            firstPersonCamera.MoveToTopOfPrioritySubqueue();
            mainUnityCamera.cullingMask = firstPersonCullingMask;
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("ThirdPersonOnly"), true);
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("FirstPersonOnly"), false);
        }
    }
}