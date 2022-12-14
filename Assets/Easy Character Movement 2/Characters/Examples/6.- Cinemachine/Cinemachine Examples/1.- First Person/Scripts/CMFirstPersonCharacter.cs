using UnityEngine;

namespace EasyCharacterMovement.Examples.Cinemachine.FirstPersonExample
{
    /// <summary>
    /// This example shows how to extend the FirstPersonCharacter to use Cinemachine Cameras
    /// instead of default camera, this way you can take advantage of Cinemachine features
    /// while retaining the same ECM2 functionality.
    ///
    /// Here shows how to replace the default programatically animation to use Cinemachine to perform crouch / un crouch animation.
    /// </summary>

    public class CMFirstPersonCharacter : FirstPersonCharacter
    {
        #region EDITOR EXPOSED FIELDS

        [Header("Cinemachine")]
        public GameObject cmWalkingCamera;
        public GameObject cmCrouchedCamera;

        #endregion

        #region METHODS

        protected override void AnimateEye()
        {
            // Removes programatically crouch / un crouch animation as this will be handled by Cinemachine cameras
        }

        protected override void OnCrouched()
        {
            // Call base method

            base.OnCrouched();

            // Transition to crouched cinemachine camera

            cmWalkingCamera.SetActive(false);
            cmCrouchedCamera.SetActive(true);
        }

        protected override void OnUnCrouched()
        {
            // Call base method

            base.OnUnCrouched();

            // Transition to un crouched cinemachine camera

            cmCrouchedCamera.SetActive(false);
            cmWalkingCamera.SetActive(true);
        }

        #endregion
    }
}