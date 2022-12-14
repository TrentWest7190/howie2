using UnityEngine;
using UnityEngine.InputSystem;

namespace EasyCharacterMovement.Examples.Cinemachine.ThirdPersonExample
{
    /// <summary>
    /// This example shows how to extend the Character class to implement a Cinemachine based third person camera.
    ///
    /// This uses the Cinemachine 3rd person follow method to implement the third person camera.
    /// </summary>

    public class CMThirdPersonCharacter : Character
    {
        #region EDITOR EXPOSED FIELDS

        public Transform cameraTarget;

        public float cameraTurnSensitivity = 0.1f;
        public float cameraLookUpSensitivity = 0.1f;

        #endregion

        #region FIELDS

        private float _pitch;
        private float _yaw;

        private Vector2 _lookInput;

        #endregion        

        #region INPUT ACTIONS

        protected InputAction mouseLookInputAction { get; set; }

        #endregion

        #region INPUT ACTION HANDLERS

        /// <summary>
        /// Mouse Look input action handler.
        /// </summary>

        protected virtual void OnMouseLook(InputAction.CallbackContext context)
        {
            _lookInput = context.ReadValue<Vector2>();
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Rotate the camera along its yaw.
        /// </summary>

        public void AddCameraYawInput(float value)
        {
            _yaw = MathLib.Clamp0360(_yaw + value);
        }

        /// <summary>
        /// Rotate the camera along its pitch.
        /// </summary>

        public void AddCameraPitchInput(float value)
        {
            _pitch = Mathf.Clamp(_pitch + value, -80.0f, 80.0f);
        }

        /// <summary>
        /// Extends HandleInput method to add camera input.
        /// </summary>

        protected override void HandleInput()
        {
            // Call base method

            base.HandleInput();

            // Camera input (mouse look),
            // Rotates the camera target independently of the Character's rotation,
            // basically we are manually rotating the Cinemachine camera here

            if (IsDisabled())
                return;

            if (_lookInput.x != 0.0f)
                AddCameraYawInput(_lookInput.x * cameraTurnSensitivity);

            if (_lookInput.y != 0.0f)
                AddCameraPitchInput(-_lookInput.y * cameraLookUpSensitivity);
        }

        protected override void OnLateUpdate()
        {
            // Call base method

            base.OnLateUpdate();

            // Set final camera rotation

            cameraTarget.rotation = Quaternion.Euler(_pitch, _yaw, 0.0f);
        }

        protected override void InitPlayerInput()
        {
            // Call base method

            base.InitPlayerInput();

            // Setup mouse look input action

            if (inputActions == null)
                return;

            mouseLookInputAction = inputActions.FindAction("Mouse Look");
            if (mouseLookInputAction != null)
            {
                mouseLookInputAction.started += OnMouseLook;
                mouseLookInputAction.performed += OnMouseLook;
                mouseLookInputAction.canceled += OnMouseLook;

                mouseLookInputAction.Enable();
            }
        }

        protected override void DeinitPlayerInput()
        {
            // Call base method

            base.DeinitPlayerInput();

            if (mouseLookInputAction != null)
            {
                mouseLookInputAction.started -= OnMouseLook;
                mouseLookInputAction.performed -= OnMouseLook;
                mouseLookInputAction.canceled -= OnMouseLook;

                mouseLookInputAction.Disable();
                mouseLookInputAction = null;
            }
        }

        /// <summary>
        /// Overrides OnStart to initialize this.
        /// </summary>

        protected override void OnStart()
        {
            // Call base method

            base.OnStart();

            // Cache camera's initial orientation (yaw / pitch)

            Vector3 cameraTargetEulerAngles = cameraTarget.eulerAngles;

            _pitch = cameraTargetEulerAngles.x;
            _yaw = cameraTargetEulerAngles.y;
        }

        #endregion
    }
}
