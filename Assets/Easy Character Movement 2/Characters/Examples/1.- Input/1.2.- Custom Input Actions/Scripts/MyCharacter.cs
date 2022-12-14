using UnityEngine;
using UnityEngine.InputSystem;

namespace EasyCharacterMovement.Examples.NewInput.CustomInputActionsExample
{
    /// <summary> 
    /// This shows how to extend the Character class and the steps needed to add custom input action.
    /// </summary>

    public sealed class MyCharacter : Character
    {
        #region INPUT ACTIONS

        /// <summary>
        /// Interact InputAction.
        /// </summary>

        private InputAction interactInputAction { get; set; }

        #endregion

        #region INPUT ACTION HANDLERS

        /// <summary>
        /// Interact input action handler.
        /// </summary>

        private void OnInteract(InputAction.CallbackContext context)
        {
            if (context.started)
                Interact();
            else if (context.canceled)
                StopInteracting();
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Start interaction.
        /// </summary>

        public void Interact()
        {
            Debug.Log("Player Pressed Interaction Button");
        }

        /// <summary>
        /// Stops interaction.
        /// </summary>

        public void StopInteracting()
        {
            Debug.Log("Player Released Interaction Button");
        }

        /// <summary>
        /// Setup player input actions.
        /// </summary>

        protected override void InitPlayerInput()
        {
            // Setup base input actions (eg: Movement, Jump, Sprint, Crouch)

            base.InitPlayerInput();

            // Setup Interact input action handlers

            interactInputAction = inputActions.FindAction("Interact");
            if (interactInputAction != null)
            {
                interactInputAction.started += OnInteract;
                interactInputAction.canceled += OnInteract;

                interactInputAction.Enable();
            }
        }

        /// <summary>
        /// Unsubscribe from input action events and disable input actions.
        /// </summary>

        protected override void DeinitPlayerInput()
        {
            base.DeinitPlayerInput();

            if (interactInputAction != null)
            {
                interactInputAction.started -= OnInteract;
                interactInputAction.canceled -= OnInteract;

                interactInputAction.Disable();
                interactInputAction = null;
            }
        }

        #endregion
    }
}
