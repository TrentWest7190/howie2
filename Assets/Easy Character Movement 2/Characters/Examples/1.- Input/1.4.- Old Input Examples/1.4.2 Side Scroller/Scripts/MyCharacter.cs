using UnityEngine;

namespace EasyCharacterMovement.Examples.OldInput.SideScrollerExample
{
    /// <summary>
    /// This shows how to use the old input system to perform typical side scroller movement.
    /// </summary>

    public class MyCharacter : Character
    {
        /// <summary>
        /// Handles the character input using old input system.
        /// </summary>

        protected override void HandleInput()
        {
            // Add horizontal input movement (in world space)

            Vector3 movementDirection = Vector3.right * Input.GetAxisRaw("Horizontal");

            SetMovementDirection(movementDirection);

            // Perform instant side to side lock rotation.

            if (movementDirection.x != 0.0f)
                SetYaw(movementDirection.x * 90.0f);

            // Jump

            if (Input.GetButtonDown("Jump"))
                Jump();
            else if (Input.GetButtonUp("Jump"))
                StopJumping();

            // Crouch

            if (Input.GetKeyDown(KeyCode.LeftControl))
                Crouch();
            else if (Input.GetKeyUp(KeyCode.LeftControl))
                StopCrouching();

            // Sprint

            if (Input.GetKeyDown(KeyCode.LeftShift))
                Sprint();
            else if (Input.GetKeyUp(KeyCode.LeftShift))
                StopSprinting();
        }
    }
}