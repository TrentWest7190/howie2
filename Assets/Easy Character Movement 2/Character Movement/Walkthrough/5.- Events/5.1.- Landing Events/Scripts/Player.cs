using EasyCharacterMovement;
using UnityEngine;

namespace EasyCharacterMovement.CharacterMovementWalkthrough.LandingAndEvents
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

        [Space(15f)]
        [Tooltip("Initial velocity (instantaneous vertical velocity) when jumping.")]
        public float jumpImpulse = 6.5f;

        [Tooltip("Friction to apply when falling.")]
        public float airFriction = 0.1f;

        [Range(0.0f, 1.0f)]
        [Tooltip("When falling, amount of horizontal movement control available to the character.\n" +
                 "0 = no control, 1 = full control at max acceleration.")]
        public float airControl = 0.3f;

        [Tooltip("The character's gravity.")]
        public Vector3 gravity = Vector3.down * 9.81f;

        [Space(15f)]
        [Tooltip("Character's height when standing.")]
        public float standingHeight = 2.0f;

        [Tooltip("Character's height when crouching.")]
        public float crouchingHeight = 1.25f;

        [Tooltip("The max speed modifier while crouching.")]
        [Range(0.0f, 1.0f)]
        public float crouchingSpeedModifier = 0.5f;

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

        /// <summary>
        /// Jump input.
        /// </summary>

        public bool jump { get; set; }

        /// <summary>
        /// Crouch input command.
        /// </summary>

        public bool crouch { get; set; }

        /// <summary>
        /// Is the character crouching?
        /// </summary>

        public bool isCrouching { get; protected set; }

        #endregion

        #region EVENTS

        /// <summary>
        /// FoundGround event handler.
        /// </summary>

        private void OnFoundGround(ref FindGroundResult foundGround)
        {
            Debug.Log("Found ground...");

            // Determine if the character has landed

            if (!characterMovement.wasOnGround && foundGround.isWalkableGround)
            {
                Debug.Log("Landed!");
            }
        }

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

            // Jump input

            jump = Input.GetButton($"Jump");

            // Crouch input

            crouch = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.C);
        }

        /// <summary>
        /// Update the character's rotation.
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
        /// Handle character's Crouch / UnCrouch.
        /// </summary>

        private void Crouching()
        {
            // Process crouch input command

            if (crouch)
            {
                // If already crouching, return

                if (isCrouching)
                    return;

                // Set capsule crouching height

                characterMovement.SetHeight(crouchingHeight);

                // Update Crouching state

                isCrouching = true;
            }
            else
            {
                // If not crouching, return

                if (!isCrouching)
                    return;

                // Check if character can safely stand up

                if (!characterMovement.CheckHeight(standingHeight))
                {
                    // Character can safely stand up, set capsule standing height

                    characterMovement.SetHeight(standingHeight);

                    // Update crouching state

                    isCrouching = false;
                }
            }
        }

        /// <summary>
        /// Handle jumping state.
        /// </summary>

        private void Jumping()
        {
            if (jump && characterMovement.isGrounded)
            {
                // Pause ground constraint so character can jump off ground

                characterMovement.PauseGroundConstraint();

                // perform the jump

                Vector3 jumpVelocity = Vector3.up * jumpImpulse;

                characterMovement.LaunchCharacter(jumpVelocity, true);
            }
        }

        /// <summary>
        /// Perform character movement.
        /// </summary>

        private void Move()
        {
            float targetSpeed = isCrouching ? maxSpeed * crouchingSpeedModifier : maxSpeed;

            Vector3 desiredVelocity = movementDirection * targetSpeed;

            // Update character’s velocity based on its grounding status

            if (characterMovement.isGrounded)
                GroundedMovement(desiredVelocity);
            else
                NotGroundedMovement(desiredVelocity);

            // Handle jumping state

            Jumping();

            // Handle crouching state

            Crouching();
            
            // Perform movement using character's current velocity

            characterMovement.Move();
        }

        #endregion

        #region MONOBEHAVIOR

        private void Awake()
        {
            characterMovement = GetComponent<CharacterMovement>();
        }

        private void OnEnable()
        {
            // Subscribe to CharacterMovement events

            characterMovement.FoundGround += OnFoundGround;
        }

        private void OnDisable()
        {
            // Un-Subscribe from CharacterMovement events

            characterMovement.FoundGround -= OnFoundGround;
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
