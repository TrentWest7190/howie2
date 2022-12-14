using UnityEngine;
using UnityEngine.InputSystem;

namespace EasyCharacterMovement.Examples.Gameplay.TwinStickExample
{
    /// <summary>
    /// This example shows how to extend the Character class to perform a typical twin-stick shooter movement,
    /// where the character is moved with left stick and aim with the right stick. 
    /// </summary>

    public class TwinStickCharacter : Character
    {
        #region FIELDS

        private Vector2 _fireInput;

        #endregion

        #region INPUT ACTIONS

        private InputAction fireInputAction { get; set; }

        #endregion

        #region INPUT ACTION HANDLERS

        private void OnFire(InputAction.CallbackContext context)
        {
            _fireInput = context.ReadValue<Vector2>();
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Override UpdateRotation method to add support for right stick aim rotation.
        /// </summary>

        protected override void UpdateRotation()
        {
            // If no fire input...

            if (_fireInput.isZero())
            {
                // Rotate towards movement direction

                Vector3 movementDirection = GetMovementDirection();

                RotateTowards(movementDirection);
            }
            else
            {
                // Rotate towards fire input direction,
                // if camera is assigned, transform fire direction to be relative to camera's view direction

                Vector3 fireDirection = new Vector3(_fireInput.x, 0.0f, _fireInput.y);

                if (cameraTransform != null)
                    fireDirection = fireDirection.relativeTo(cameraTransform);

                RotateTowards(fireDirection);
            }
        }

        /// <summary>
        /// Override SetupPlayerInput to add fire input action.
        /// </summary>
        
        protected override void InitPlayerInput()
        {
            // Setup default input actions

            base.InitPlayerInput();

            // If no actions, return

            if (inputActions == null)
                return;

            // Setup fire input action

            fireInputAction = inputActions.FindAction("Fire");
            if (fireInputAction != null)
            {
                fireInputAction.started += OnFire;
                fireInputAction.performed += OnFire;
                fireInputAction.canceled += OnFire;

                fireInputAction.Enable();
            }
        }

        /// <summary>
        /// Unsubscribe from input action events and disable input actions.
        /// </summary>

        protected override void DeinitPlayerInput()
        {
            base.DeinitPlayerInput();

            if (fireInputAction != null)
            {
                fireInputAction.started -= OnFire;
                fireInputAction.performed -= OnFire;
                fireInputAction.canceled -= OnFire;

                fireInputAction.Disable();
                fireInputAction = null;
            }
        }

        #endregion
    }
}
