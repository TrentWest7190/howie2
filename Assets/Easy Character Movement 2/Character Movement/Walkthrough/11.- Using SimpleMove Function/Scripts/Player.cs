using EasyCharacterMovement;
using System.Collections;
using UnityEngine;

namespace EasyCharacterMovement.CharacterMovementWalkthrough.UsingSimpleMove
{
    public class Player : MonoBehaviour
    {
        #region EDITOR EXPOSED FIELDS

        [Space(15f)]
        [Tooltip("Change in rotation per second (Deg / s).")]
        public float rotationRate = 540.0f;

        [Space(15f)]
        [Tooltip("The maximum ground speed when grounded." +
                 " Also determines maximum lateral speed when falling.")]
        public float maxSpeed = 5.0f;

        [Tooltip("Max Acceleration (rate of change of velocity).")]
        public float maxAcceleration = 20.0f;

        [Tooltip("Deceleration when grounded and not applying acceleration." +
                 " This is a constant opposing force that directly lowers velocity by a constant value. ")]
        public float groundBrakingDeceleration = 20.0f;

        [Tooltip("Setting that affects movement control. Higher values allow faster changes in direction.")]
        public float groundFriction = 8.0f;

        [Space(15f)]
        [Tooltip("The rate at which the character's slows down when braking (i.e. no input received).")]
        public float airBrakingDeceleration;

        [Tooltip("Friction to apply to horizontal air movement when falling.")]
        public float airFriction = 0.3f;
        
        [Range(0.0f, 1.0f)]
        [Tooltip("When falling, amount of horizontal movement control available to the character.\n" +
                 "0 = no control, 1 = full control at max acceleration.")]
        public float airControl = 0.3f;

        [Space(15f)]
        [Tooltip("If true, brakingFriction will be used to slow the character to a stop (when there is no Acceleration)," +
                 " otherwise current friction such as groundFriction is used.")]
        public bool useSeparateBrakingFriction;

        [Tooltip("Friction (drag) coefficient applied when braking (whenever Acceleration = 0, or if character is exceeding max speed).\n" +
                 "Only used if useSeparateBrakingFriction setting is true, otherwise current friction such as groundFriction is used.")]
        public float brakingFriction;

        [Space(15f)]
        [Tooltip("Initial velocity (instantaneous vertical velocity) when jumping.")]
        public float jumpImpulse = 6.5f;

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

        #region FIELDS

        private Coroutine _lateFixedUpdateCoroutine;

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

        #region EVENT HANDLERS

        /// <summary>
        /// Collided event handler.
        /// </summary>

        private void OnCollided(ref CollisionResult inHit)
        {
            Debug.Log($"{name} collided with: {inHit.collider.name}");
        }

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
        /// Returns maximum speed for the current state.
        /// E.g. if crouching apply crouchingSpeedModifier. 
        /// </summary>

        private float GetMaxSpeed()
        {
            if (characterMovement.isGrounded)
                return isCrouching ? maxSpeed * crouchingSpeedModifier : maxSpeed;

            return maxSpeed;
        }

        /// <summary>
        /// Return character's max acceleration for the current state.
        /// </summary>

        private float GetMaxAcceleration()
        {
            return characterMovement.isGrounded ? maxAcceleration : maxAcceleration * airControl;
        }

        /// <summary>
        /// Returns maximum deceleration for the current state when braking (ie when there is no acceleration).
        /// </summary>

        private float GetBrakingDeceleration()
        {
            return characterMovement.isGrounded ? groundBrakingDeceleration : airBrakingDeceleration;
        }

        /// <summary>
        /// Return character's friction for the current state.
        /// </summary>

        private float GetFriction()
        {
            return characterMovement.isGrounded ? groundFriction : airFriction;
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
            // Handle jumping state

            Jumping();

            // Handle crouching state

            Crouching();
            
            // Perform friction-based movement using the CharacterMovement SimpleMove as this will handle all this for us!

            float actualMaxSpeed = GetMaxSpeed();
            float actualAcceleration = GetMaxAcceleration();
            float actualBrakingDeceleration = GetBrakingDeceleration();
            float actualFriction = GetFriction();
            float actualBrakingFriction = useSeparateBrakingFriction ? brakingFriction : GetFriction();
            
            Vector3 desiredVelocity = movementDirection * actualMaxSpeed;

            characterMovement.SimpleMove(desiredVelocity, actualMaxSpeed, actualAcceleration, actualBrakingDeceleration,
                actualFriction, actualBrakingFriction, gravity);
        }

        /// <summary>
        /// Post-Physics update, used to sync our character with physics.
        /// </summary>

        private void OnLateFixedUpdate()
        {
            UpdateRotation();
            Move();
        }

        #endregion

        #region MONOBEHAVIOR

        private void Awake()
        {
            // Cache CharacterMovement component

            characterMovement = GetComponent<CharacterMovement>();

            // Enable default physic interactions

            characterMovement.enablePhysicsInteraction = true;
        }

        private void OnEnable()
        {
            // Start LateFixedUpdate coroutine

            if (_lateFixedUpdateCoroutine != null)
                StopCoroutine(_lateFixedUpdateCoroutine);

            _lateFixedUpdateCoroutine = StartCoroutine(LateFixedUpdate());

            // Subscribe to CharacterMovement events

            characterMovement.FoundGround += OnFoundGround;
            characterMovement.Collided += OnCollided;
        }

        private void OnDisable()
        {
            // Ends LateFixedUpdate coroutine

            if (_lateFixedUpdateCoroutine != null)
                StopCoroutine(_lateFixedUpdateCoroutine);

            // Un-Subscribe from CharacterMovement events

            characterMovement.FoundGround -= OnFoundGround;
            characterMovement.Collided -= OnCollided;
        }

        private IEnumerator LateFixedUpdate()
        {
            WaitForFixedUpdate waitTime = new WaitForFixedUpdate();

            while (true)
            {
                yield return waitTime;

                OnLateFixedUpdate();
            }
        }

        private void Update()
        {
            HandleInput();
        }

        #endregion
    }
}
