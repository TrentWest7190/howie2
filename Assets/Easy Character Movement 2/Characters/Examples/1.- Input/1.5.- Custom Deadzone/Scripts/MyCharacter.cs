using UnityEngine;

namespace EasyCharacterMovement.Examples.NewInput.DeadzoneExample
{
    /// <summary>
    /// This example, show how to implement different types of deadzone, extending the GetMovementInput method.
    /// Source: https://www.gamedeveloper.com/disciplines/doing-thumbstick-dead-zones-right
    /// </summary>

    public class MyCharacter : ThirdPersonCharacter
    {
        private static Vector2 AxialDeadzone(Vector2 input)
        {
            if (Mathf.Abs(input.x) < 0.2f)
                input.x = 0.0f;

            if (Mathf.Abs(input.y) < 0.2f)
                input.y = 0.0f;
            
            return input;
        }

        private static Vector2 RadialDeadzone(Vector2 input)
        {
            const float kDeadzone = 0.25f;

            if (input.magnitude < kDeadzone)
                input = Vector2.zero;

            return input;
        }

        private static Vector2 ScaledRadialDeadzone(Vector2 input)
        {
            const float kDeadzone = 0.25f;
            
            if(input.magnitude < kDeadzone)
                input = Vector2.zero;
            else
                input = input.normalized * ((input.magnitude - kDeadzone) / (1.0f - kDeadzone));
            
            return input;
        }

        /// <summary>
        /// Extends GetMovementInput method to add a custom deadzone.
        /// </summary>

        protected override Vector2 GetMovementInput()
        {
            // Implements a basic 0.2 x 0.2 axial deadzone area

            Vector2 movementInput = base.GetMovementInput();

            return AxialDeadzone(movementInput);
        }

        /// <summary>
        /// Extends GetMouseLookInput method to add a custom deadzone.
        /// </summary>

        protected override Vector2 GetMouseLookInput()
        {
            // Implements a basic 0.2 x 0.2 axial deadzone area

            Vector2 lookInput = base.GetMouseLookInput();

            return AxialDeadzone(lookInput);
        }

        /// <summary>
        /// Extends GetMouseLookInput method to add a custom deadzone.
        /// </summary>

        protected override Vector2 GetControllerLookInput()
        {
            // Implements a basic 0.2 x 0.2 axial deadzone area

            Vector2 lookInput = base.GetControllerLookInput();

            return AxialDeadzone(lookInput);
        }
    }
}
