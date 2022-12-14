using UnityEngine;

namespace EasyCharacterMovement.CharacterMovementWalkthrough.MovingPlatforms
{
    public class RotatingPlatform : MonoBehaviour
    {
        #region FIELDS

        [SerializeField]
        private float _rotationSpeed = 30.0f;

        [SerializeField]
        private Vector3 _axis = Vector3.up;

        #endregion

        #region PRIVATE FIELDS

        private Rigidbody _rigidbody;

        private float _angle;

        #endregion

        #region PROPERTIES

        public float rotationSpeed
        {
            get => _rotationSpeed;
            set => _rotationSpeed = value;
        }

        public float angle
        {
            get => _angle;
            set => _angle = Clamp0360(value);
        }

        #endregion

        #region MONOBEHAVIOUR

        /// <summary>
        /// Clamps the given angle into 0 - 360 degrees range.
        /// </summary>

        public static float Clamp0360(float eulerAngles)
        {
            float result = eulerAngles - Mathf.CeilToInt(eulerAngles / 360f) * 360f;
            if (result < 0) result += 360f;

            return result;
        }

        public void OnValidate()
        {
            rotationSpeed = _rotationSpeed;
        }

        public void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.isKinematic = true;
        }

        public void FixedUpdate()
        {
            angle += rotationSpeed * Time.deltaTime;

            _rigidbody.MoveRotation(Quaternion.Euler(_axis * angle));
        }

        #endregion
    }
}