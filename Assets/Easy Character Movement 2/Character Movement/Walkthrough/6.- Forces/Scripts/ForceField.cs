using System;
using UnityEngine;

namespace EasyCharacterMovement.CharacterMovementWalkthrough.Forces
{
    /// <summary>
    /// This example shows how to implement a ForceField for characters using the CharacterMovement component.
    /// </summary>

    public class ForceField : MonoBehaviour
    {
        public ForceMode forceMode = ForceMode.Force;
        public float forceMagnitude = 15.0f;

        // Cached CharacterMovement component

        private CharacterMovement characterMovement { get; set; }

        private void OnTriggerEnter(Collider other)
        {
            // Cache CharacterMovement (if any)

            characterMovement = other.GetComponent<CharacterMovement>();
        }

        private void OnTriggerExit(Collider other)
        {
            // If our cached character leaves the trigger, remove cached CharacterController

            if (other.TryGetComponent(out CharacterMovement component) &&
                characterMovement.gameObject == component.gameObject)
            {
                characterMovement = null;
            }
        }

        private void Update()
        {
            // If a character is inside ForceField trigger area, add force!

            if (characterMovement)
            {
                // If the character is grounded, pause ground constraint so it can leave the ground

                if (characterMovement.isGrounded)
                    characterMovement.PauseGroundConstraint();

                // Add continuous force
                
                characterMovement.AddForce(transform.up * forceMagnitude, forceMode);
            }
        }
    }
}
