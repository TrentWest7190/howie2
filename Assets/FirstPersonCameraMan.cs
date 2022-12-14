using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyCharacterMovement;
using UnityEngine.InputSystem;
using Cinemachine;

public class FirstPersonCameraMan : FirstPersonCharacter
{
  [SerializeField] private float zoomSensitivity = 0.5f;
  protected InputAction zoomInputAction { get; set; }
  protected InputAction resetZoomAction { get; set; }

  private CinemachineVirtualCamera _vCam;
  private float initialFOV;
  private float initialDutch;

  protected override void OnAwake()
  {
    base.OnAwake();
    initialFOV = vCam.m_Lens.FieldOfView;
    initialDutch = vCam.m_Lens.Dutch;
  }

  protected override void OnStart()
  {
    base.OnStart();
    characterLook.UnlockCursor();
  }

  protected CinemachineVirtualCamera vCam
  {
    get
    {
      if (_vCam == null)
        _vCam = GetComponentInChildren<CinemachineVirtualCamera>();

      return _vCam;
    }
  }

  private bool _isZooming = false;

  protected override void InitPlayerInput()
  {
    base.InitPlayerInput();

    zoomInputAction = inputActions.FindAction("Zoom");
    if (zoomInputAction != null)
    {
      zoomInputAction.started += OnZoom;
      zoomInputAction.canceled += OnZoom;
      zoomInputAction.Enable();
    }

    resetZoomAction = inputActions.FindAction("Reset Zoom");
    if (resetZoomAction != null)
    {
      resetZoomAction.started += OnResetZoom;
      resetZoomAction.Enable();
    }
  }

  protected void OnZoom(InputAction.CallbackContext context)
  {
    if (context.started)
    {
      Debug.Log("Zoom started");
      _isZooming = true;
      SetMovementMode(MovementMode.None);
    }

    if (context.canceled)
    {
      Debug.Log("Zoom ended");
      _isZooming = false;
      SetMovementMode(MovementMode.Flying);
    }
  }

  protected void OnResetZoom(InputAction.CallbackContext context)
  {
    if (context.started)
    {
      vCam.m_Lens.FieldOfView = initialFOV;
      vCam.m_Lens.Dutch = initialDutch;
    }
  }

  protected override void HandleInput()
  {
    base.HandleInput();

    if (!characterLook.IsCursorLocked())
      return;

    Vector2 movementInput = GetMovementInput();

    Vector3 movementDirection = Vector3.zero;

    movementDirection += GetEyeForwardVector() * movementInput.y;
    movementDirection += GetRightVector() * movementInput.x;

    if (_isZooming)
    {
      Vector2 mouseLookInput = GetMouseLookInput();
      vCam.m_Lens.FieldOfView -= mouseLookInput.y * zoomSensitivity;
      vCam.m_Lens.Dutch -= mouseLookInput.x * zoomSensitivity;
    }

    if (jumpButtonPressed)
      movementDirection += GetUpVector();

    if (_crouchButtonPressed)
      movementDirection -= GetUpVector();

    SetMovementDirection(movementDirection);


  }

  public override float GetMaxSpeed()
  {
    return IsSprinting() ? maxFlySpeed * sprintSpeedModifier : maxFlySpeed;
  }
}
