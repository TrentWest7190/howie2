using EasyCharacterMovement;
using UnityEngine;

namespace EasyCharacterMovement.CharacterMovementWalkthrough.MovingOurPlayer
{
    public class Player : MonoBehaviour
    {
        #region EDITOR EXPOSED FIELDS

        [Tooltip("Change in rotation per second (Deg / s).")]
        public float rotationRate = 540.0f;

        [Space(15f)]
        [Tooltip("The character's maximum speed.")]
        public float maxSpeed = 5.0f;

        [Tooltip("Max Acceleration (rate of change of velocity).")]
        public float maxAcceleration = 20.0f;

        [Tooltip("Setting that affects movement control. Higher values allow faster changes in direction.")]
        public float groundFriction = 8.0f;

        [Tooltip("Friction to apply when falling.")]
        public float airFriction = 0.1f;

        [Range(0.0f, 1.0f)]
        [Tooltip("When falling, amount of horizontal movement control available to the character.\n" +
                 "0 = no control, 1 = full control at max acceleration.")]
        public float airControl = 0.3f;

        [Tooltip("The character's gravity.")]
        public Vector3 gravity = Vector3.down * 9.81f;

        #endregion

        #region PROPERTIES

        /// <summary>
        /// Cached CharacterMovement component.
        /// </summary>

        public CharacterMovement characterMovement { get; private set; }

        /// <summary>
        /// Desired movement direction vector in world-space.
        /// </summary>

        public Vector3 movementDirection { get; set; }

        #endregion

        #region METHODS

        /// <summary>
        /// Handle Player input.
        /// </summary>

        private void HandleInput()
        {
            // Read Input values

            float horizontal = Input.GetAxisRaw($"Horizontal");
            float vertical = Input.GetAxisRaw($"Vertical");

            // Create a Movement direction vector (in world space)

            movementDirection = Vector3.zero;

            movementDirection += Vector3.forward * vertical;
            movementDirection += Vector3.right * horizontal;

            // Make Sure it won't move faster diagonally

            movementDirection = Vector3.ClampMagnitude(movementDirection, 1.0f);
        }

        /// <summary>
        /// Updates the character's rotation.
        /// </summary>

        private void UpdateRotation()
        {
            // Rotate towards character's movement direction

            characterMovement.RotateTowards(movementDirection, rotationRate * Time.deltaTime);
        }

        /// <summary>
        /// Move the character when on walkable ground.
        /// </summary>

        private void GroundedMovement(Vector3 desiredVelocity)
        {
            characterMovement.velocity = Vector3.Lerp(characterMovement.velocity, desiredVelocity,
                1f - Mathf.Exp(-groundFriction * Time.deltaTime));
        }

        /// <summary>
        /// Move the character when falling or on not-walkable ground.
        /// </summary>

        private void NotGroundedMovement(Vector3 desiredVelocity)
        {
            // Current character's velocity

            Vector3 velocity = characterMovement.velocity;

            // If moving into non-walkable ground, limit its contribution.
            // Allow movement parallel, but not into it because that may push us up.
            
            if (characterMovement.isOnGround && Vector3.Dot(desiredVelocity, characterMovement.groundNormal) < 0.0f)
            {
                Vector3 groundNormal = characterMovement.groundNormal;
                Vector3 groundNormal2D = groundNormal.onlyXZ().normalized;

                desiredVelocity = desiredVelocity.projectedOnPlane(groundNormal2D);
            }

            // If moving...

            if (desiredVelocity != Vector3.zero)
            {
                // Accelerate horizontal velocity towards desired velocity

                Vector3 horizontalVelocity = Vector3.MoveTowards(velocity.onlyXZ(), desiredVelocity,
                    maxAcceleration * airControl * Time.deltaTime);

                // Update velocity preserving gravity effects (vertical velocity)
                
                velocity = horizontalVelocity + velocity.onlyY();
            }

            // Apply gravity

            velocity += gravity * Time.deltaTime;

            // Apply Air friction (Drag)

            velocity -= velocity * airFriction * Time.deltaTime;

            // Update character's velocity

            characterMovement.velocity = velocity;
        }

        /// <summary>
        /// Perform character movement.
        /// </summary>

        private void Move()
        {
            // Create our desired velocity using the previously created movement direction vector

            Vector3 desiredVelocity = movementDirection * maxSpeed;

            // Update character’s velocity based on its grounding status

            if (characterMovement.isGrounded)
                GroundedMovement(desiredVelocity);
            else
                NotGroundedMovement(desiredVelocity);

            // Perform movement using character's current velocity

            characterMovement.Move();
        }

        #endregion

        #region MONOBEHAVIOR

        private void Awake()
        {
            characterMovement = GetComponent<CharacterMovement>();
        }

        private void Update()
        {
            HandleInput();
            UpdateRotation();
            Move();
        }

        #endregion
    }
}
