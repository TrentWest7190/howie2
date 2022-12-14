using EasyCharacterMovement;
using UnityEngine;

namespace EasyCharacterMovement.CharacterMovementWalkthrough.Forces
{
    /// <summary>
    /// This example shows how to implement a bouncer for characters using the CharacterMovement component.
    /// </summary>

    public class Bouncer : MonoBehaviour
    {
        public float launchImpulse = 15.0f;

        public bool overrideVerticalVelocity;
        public bool overrideLateralVelocity;

        private void OnTriggerEnter(Collider other)
        {
            // Check if the entered collider is using the CharacterMovement component

            if (other.TryGetComponent(out CharacterMovement characterMovement))
            {
                // If necessary, temporarily disable the character's ground constraint so it leave the ground

                if (characterMovement.isGrounded)
                    characterMovement.PauseGroundConstraint();

                // Launch character!

                characterMovement.LaunchCharacter(transform.up * launchImpulse, overrideVerticalVelocity,
                    overrideLateralVelocity);
            }
        }
    }
}
