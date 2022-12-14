using UnityEngine;

namespace EasyCharacterMovement.Examples.OldInput.TopDownExample
{
    /// <summary>
    /// This example shows how to replace the Unity Input system with old input system.
    /// </summary>

    public class MyCharacter : AgentCharacter
    {
        /// <summary>
        /// Handles the character input using old input system.
        /// </summary>

        private void HandleCharacterInput()
        {
            // Movement (click-to-move)

            if (Input.GetMouseButton(0))
            {
                Ray ray = camera.ScreenPointToRay(Input.mousePosition);

                LayerMask groundMask = characterMovement.collisionLayers;

                QueryTriggerInteraction triggerInteraction = characterMovement.triggerInteraction;

                if (Physics.Raycast(ray, out RaycastHit hitResult, Mathf.Infinity, groundMask, triggerInteraction))
                    MoveToLocation(hitResult.point);
            }

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

        /// <summary>
        /// Override the HandleInput method to use old input system.
        /// </summary>

        protected override void HandleInput()
        {
            HandleCharacterInput();
        }
    }
}
