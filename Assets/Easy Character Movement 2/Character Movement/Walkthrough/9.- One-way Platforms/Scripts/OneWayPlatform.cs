using EasyCharacterMovement;
using UnityEngine;

namespace EasyCharacterMovement.CharacterMovementWalkthrough.OneWayPlatforms
{
    /// <summary>
    /// This basic example shows one easy method to implement a one-way platform.
    ///
    /// This enables / disables the platform / character collisions when the Character's enter / exits the platform trigger volume.
    /// 
    /// </summary>

    public class OneWayPlatform : MonoBehaviour
    {
        [Tooltip("The platform's collider")]
        public Collider platform;

        private void OnTriggerEnter(Collider other)
        {
            // If the entered collider is using the CharacterMovement component,
            // make the character ignore the platform collider

            if (other.TryGetComponent(out CharacterMovement characterMovement))
                characterMovement.IgnoreCollision(platform);
        }

        private void OnTriggerExit(Collider other)
        {
            // Resume collisions against the platform when character leaves the trigger volume

            if (other.TryGetComponent(out CharacterMovement characterMovement))
                characterMovement.IgnoreCollision(platform, false);
        }
    }

}