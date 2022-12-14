using UnityEngine;
using UnityEngine.InputSystem;

namespace EasyCharacterMovement.Examples.Gameplay.ChangeGravityDirectionExample
{
    /// <summary>
    /// This example shows how to extend a Character to change Gravity direction at run-time.
    /// </summary>

    public class MyCharacter : Character
    {
        #region INPUT ACTIONS

        public InputAction toggleGravityDirection { private get; set; }

        #endregion

        #region METHODS

        /// <summary>
        /// Overrides HandleInput method to replace the default input method with an horizontal only movement.
        /// </summary>

        protected override void HandleInput()
        {
            // Add horizontal only movement (in world space)

            Vector2 movementInput = GetMovementInput();

            Vector3 movementDirection = Vector3.right * movementInput.x;
            SetMovementDirection(movementDirection);

            //// Snap side to side rotation

            //if (movementInput.x != 0.0f)
            //    SetYaw(movementInput.x * 90.0f);

            // Toggle gravity direction if character is on air (e.g. Jumping)

            if (toggleGravityDirection.triggered && !IsGrounded())
                gravityScale *= -1.0f;
        }

        /// <summary>
        /// Extends UpdateRotation method to orient the Character towards gravity direction.
        /// </summary>

        protected override void UpdateRotation()
        {
            // Call base method implementation

            base.UpdateRotation();

            // Append gravity-direction rotation

            Quaternion targetRotation = Quaternion.FromToRotation(GetUpVector(), -GetGravityDirection()) * characterMovement.rotation;

            characterMovement.rotation = Quaternion.RotateTowards(characterMovement.rotation, targetRotation, rotationRate * deltaTime);
        }

        /// <summary>
        /// Extends SetupPlayerInput to init Toggle Gravity Direction InputAction.
        /// </summary>

        protected override void InitPlayerInput()
        {
            base.InitPlayerInput();

            toggleGravityDirection = inputActions.FindAction("Toggle Gravity Direction");
            toggleGravityDirection?.Enable();
        }

        /// <summary>
        /// Unsubscribe from input action events and disable input actions.
        /// </summary>

        protected override void DeinitPlayerInput()
        {
            base.DeinitPlayerInput();

            if (toggleGravityDirection != null)
            {
                toggleGravityDirection.Disable();
                toggleGravityDirection = null;
            }
        }

        #endregion
    }
}
