using System.Collections;
using UnityEngine;

namespace EasyCharacterMovement.CharacterMovementExamples
{
    /// <summary>
    /// This example show how to create a more complete character controller (pretty close to the BaseCharacterController found in original Easy Character Movement),
    /// using the new CharacterMovement component and its SimpleMove function.
    /// </summary>

    public class AdvancedCharacterController : MonoBehaviour
    {
        #region EDITOR EXPOSED FIELDS

        [Space(15f)]
        [SerializeField]
        private float _rotationRate;

        [Space(15f)]
        [SerializeField]
        private float _maxSpeed;

        [SerializeField]
        private float _minAnalogSpeed;

        [SerializeField]
        private float _maxAcceleration;

        [SerializeField]
        private float _groundBrakingDeceleration;

        [SerializeField]
        private float _groundFriction;        
        
        [Space(15f)]
        [SerializeField]
        private float _airBrakingDeceleration;

        [SerializeField]
        private float _airFriction;

        [Range(0.0f, 1.0f)]
        [SerializeField]
        private float _airControl;

        [Space(15f)]
        [SerializeField]
        private bool _useSeparateBrakingFriction;

        [SerializeField]
        private float _brakingFriction;

        [Space(15f)]
        [SerializeField]
        private bool _canEverCrouch;

        [SerializeField]
        private float _unCrouchedHeight;

        [SerializeField]
        private float _crouchedHeight;

        [SerializeField]
        [Range(0.0f, 1.0f)]
        private float _crouchedSpeedModifier;
        
        [Space(15f)]
        [Tooltip("The max number of jumps the Character can perform.")]
        [SerializeField]
        private int _jumpMaxCount;

        [Tooltip("Initial velocity (instantaneous vertical velocity) when jumping.")]
        [SerializeField]
        private float _jumpImpulse;

        [Tooltip("The maximum time (in seconds) to hold the jump. eg: Variable height jump.")]
        [SerializeField]
        private float _jumpMaxHoldTime;

        [Tooltip("How early before hitting the ground you can trigger a jump (in seconds).")]
        [SerializeField]
        private float _jumpPreGroundedTime;

        [Tooltip("How long after leaving the ground you can trigger a jump (in seconds).")]
        [SerializeField]
        private float _jumpPostGroundedTime;

        [Space(15f)]
        [SerializeField]
        private Vector3 _gravity;

        [SerializeField]
        private float _gravityScale;

        #endregion

        #region FIELDS

        private bool _enableLateFixedUpdateCoroutine;
        private Coroutine _lateFixedUpdateCoroutine;

        private Transform _transform;
        private CharacterMovement _characterMovement;

        protected bool _isCrouching;
        protected bool _crouchButtonPressed;

        protected bool _jumpButtonPressed;
        protected float _jumpButtonHeldDownTime;
        protected float _jumpHoldTime;
        protected int _jumpCount;
        protected bool _isJumping;

        protected float _fallingTime;
        protected bool _useGravity = true;

        private Vector3 _movementDirection;

        #endregion

        #region PROPERTIES

        public new Transform transform
        {
            get
            {
#if UNITY_EDITOR
                if (_transform == null)
                    _transform = GetComponent<Transform>();
#endif

                return _transform;
            }
        }

        protected CharacterMovement characterMovement
        {
            get
            {
#if UNITY_EDITOR
                if (_characterMovement == null)
                    _characterMovement = GetComponent<CharacterMovement>();
#endif

                return _characterMovement;
            }
        }

        public float rotationRate
        {
            get => _rotationRate;
            set => _rotationRate = Mathf.Max(0.0f, value);
        }

        public float maxSpeed
        {
            get => _maxSpeed;
            set => _maxSpeed = Mathf.Max(0.0f, value);
        }

        public float minAnalogSpeed
        {
            get => _minAnalogSpeed;
            set => _minAnalogSpeed = Mathf.Max(0.0f, value);
        }

        public float maxAcceleration
        {
            get => _maxAcceleration;
            set => _maxAcceleration = Mathf.Max(0.0f, value);
        }

        public float groundBrakingDeceleration
        {
            get => _groundBrakingDeceleration;
            set => _groundBrakingDeceleration = Mathf.Max(0.0f, value);
        }

        public float groundFriction
        {
            get => _groundFriction;
            set => _groundFriction = Mathf.Max(0.0f, value);
        }
        
        public float airBrakingDeceleration
        {
            get => _airBrakingDeceleration;
            set => _airBrakingDeceleration = Mathf.Max(0.0f, value);
        }

        public float airFriction
        {
            get => _airFriction;
            set => _airFriction = Mathf.Max(0.0f, value);
        }

        public float airControl
        {
            get => _airControl;
            set => _airControl = Mathf.Max(0.0f, value);
        }

        public bool useSeparateBrakingFriction
        {
            get => _useSeparateBrakingFriction;
            set => _useSeparateBrakingFriction = value;
        }

        public float brakingFriction
        {
            get => _brakingFriction;
            set => _brakingFriction = Mathf.Max(0.0f, value);
        }

        public bool canEverCrouch
        {
            get => _canEverCrouch;
            set => _canEverCrouch = value;
        }

        public float unCrouchedHeight
        {
            get => _unCrouchedHeight;
            set => _unCrouchedHeight = Mathf.Max(0.0f, value);
        }

        public float crouchedHeight
        {
            get => _crouchedHeight;
            set => _crouchedHeight = Mathf.Max(0.0f, value);
        }

        public float crouchedSpeedModifier
        {
            get => _crouchedSpeedModifier;
            set => _crouchedSpeedModifier = Mathf.Clamp01(value);
        }

        public int jumpMaxCount
        {
            get => _jumpMaxCount;
            set => _jumpMaxCount = Mathf.Max(0, value);
        }

        public float jumpImpulse
        {
            get => _jumpImpulse;
            set => _jumpImpulse = Mathf.Max(0.0f, value);
        }

        public float jumpMaxHoldTime
        {
            get => _jumpMaxHoldTime;
            set => _jumpMaxHoldTime = Mathf.Max(0.0f, value);
        }

        public float jumpPreGroundedTime
        {
            get => _jumpPreGroundedTime;
            set => _jumpPreGroundedTime = Mathf.Max(0.0f, value);
        }

        public float jumpPostGroundedTime
        {
            get => _jumpPostGroundedTime;
            set => _jumpPostGroundedTime = Mathf.Max(0.0f, value);
        }

        public Vector3 gravity
        {
            get => _gravity;
            set => _gravity = value;
        }

        public float gravityScale
        {
            get => _gravityScale;
            set => _gravityScale = value;
        }

        public bool useGravity
        {
            get => _useGravity;
            set => _useGravity = value;
        }

        public bool enableLateFixedUpdate
        {
            get => _enableLateFixedUpdateCoroutine;
            set
            {
                _enableLateFixedUpdateCoroutine = value;
                EnableLateFixedUpdate(_enableLateFixedUpdateCoroutine);
            }
        }

        #endregion

        #region METHODS

        private void EnableLateFixedUpdate(bool enable)
        {
            if (enable)
            {
                if (_lateFixedUpdateCoroutine != null)
                    StopCoroutine(_lateFixedUpdateCoroutine);

                _lateFixedUpdateCoroutine = StartCoroutine(LateFixedUpdate());
            }
            else
            {
                if (_lateFixedUpdateCoroutine != null)
                    StopCoroutine(_lateFixedUpdateCoroutine);
            }
        }
        
        public virtual float GetMaxSpeed()
        {
            float actualMaxSpeed = maxSpeed;
            if (IsCrouching())
                actualMaxSpeed *= crouchedSpeedModifier;

            return actualMaxSpeed;
        }

        public virtual float GetMaxAcceleration()
        {
            if (characterMovement.isGrounded)
                return maxAcceleration;

            return maxAcceleration * airControl;
        }

        public virtual float GetMaxBrakingDeceleration()
        {
            return characterMovement.isGrounded ? groundBrakingDeceleration : airBrakingDeceleration;
        }

        public virtual float GetMinAnalogSpeed()
        {
            return minAnalogSpeed;
        }

        public virtual float GetFriction()
        {
            return characterMovement.isGrounded ? groundFriction : airFriction;
        }

        public virtual Vector3 GetVelocity()
        {
            return characterMovement.velocity;
        }

        public Vector3 GetPosition()
        {
            return characterMovement.position;
        }

        public void SetPosition(Vector3 newPosition, bool updateGround)
        {
            characterMovement.SetPosition(newPosition, updateGround);
        }

        public Vector3 GetUpVector()
        {
            return transform.up;
        }

        public Vector3 GetRightVector()
        {
            return transform.right;
        }

        public Vector3 GetForwardVector()
        {
            return transform.forward;
        }
        
        public Quaternion GetRotation()
        {
            return characterMovement.rotation;
        }

        public void SetRotation(Quaternion newRotation)
        {
            characterMovement.rotation = newRotation;
        }

        protected virtual void RotateTowards(Vector3 worldDirection, bool isPlanar = true)
        {
            Vector3 characterUp = transform.up;

            if (isPlanar)
                worldDirection = worldDirection.projectedOnPlane(characterUp);

            if (worldDirection.isZero())
                return;

            Quaternion targetRotation = Quaternion.LookRotation(worldDirection, characterUp);

            characterMovement.rotation = 
                Quaternion.Slerp(characterMovement.rotation, targetRotation, rotationRate * Mathf.Deg2Rad * Time.deltaTime);
        }

        protected virtual void UpdateRotation()
        {
            RotateTowards(GetMovementDirection());
        }

        public virtual void SetMovementDirection(Vector3 movementDirection)
        {
            _movementDirection = movementDirection;
        }

        public virtual Vector3 GetMovementDirection()
        {
            return _movementDirection;
        }

        public virtual Vector3 GetDesiredVelocity()
        {
            return GetMovementDirection() * GetMaxSpeed();
        }

        public virtual Vector3 GetGravity()
        {
            return gravity * gravityScale;
        }

        public virtual void Crouch()
        {
            _crouchButtonPressed = true;
        }
        
        public virtual void StopCrouching()
        {
            _crouchButtonPressed = false;
        }

        public virtual bool IsCrouching()
        {
            return _isCrouching;
        }

        protected virtual bool CanCrouch()
        {
            if (!canEverCrouch)
                return false;

            return characterMovement.isGrounded;
        }

        public virtual bool CanUnCrouch()
        {
            // Check if there's room to expand capsule

            bool overlapped = characterMovement.CheckHeight(unCrouchedHeight);

            return !overlapped;
        }

        protected virtual void Crouching()
        {
            if (_crouchButtonPressed && !IsCrouching())
            {
                if (!CanCrouch())
                    return;
                
                characterMovement.SetHeight(crouchedHeight);
                _isCrouching = true;
            }
            else if (IsCrouching() && _crouchButtonPressed == false)
            {
                if (!CanUnCrouch())
                    return;

                characterMovement.SetHeight(unCrouchedHeight);
                _isCrouching = false;
            }
        }

        public void Jump()
        {
            _jumpButtonPressed = true;
        }

        public void StopJumping()
        {
            // Input state

            _jumpButtonPressed = false;
            _jumpButtonHeldDownTime = 0.0f;

            // Jump holding state

            _isJumping = false;
            _jumpHoldTime = 0.0f;
        }

        public virtual bool IsJumping()
        {
            return _isJumping;
        }

        public virtual int GetJumpCount()
        {
            return _jumpCount;
        }

        protected virtual bool CanJump()
        {
            // Do not allow to jump while crouched

            if (IsCrouching())
                return false;

            // Cant jump if no jumps available

            if (jumpMaxCount == 0 || _jumpCount >= jumpMaxCount)
                return false;

            // Is fist jump ?

            if (_jumpCount == 0)
            {
                // On first jump,
                // can jump if is grounded or is falling (e.g. not grounded) BUT withing post grounded time

                bool canJump =  characterMovement.isGrounded ||
                               !characterMovement.isGrounded && jumpPostGroundedTime > 0.0f && _fallingTime < jumpPostGroundedTime;

                // Missed post grounded time ?

                if (!characterMovement.isGrounded && !canJump)
                {
                    // Missed post grounded time,
                    // can jump if have any 'in-air' jumps but the first jump counts as the in-air jump

                    canJump = jumpMaxCount > 1;
                    if (canJump)
                        _jumpCount++;
                }

                return canJump;
            }

            // In air jump conditions

            return !characterMovement.isGrounded;
        }

        protected virtual Vector3 CalcJumpImpulse()
        {
            Vector3 characterUp = GetUpVector();

            float verticalSpeed = Vector3.Dot(GetVelocity(), characterUp);
            float actualJumpImpulse = Mathf.Max(verticalSpeed, jumpImpulse);

            return characterUp * actualJumpImpulse;
        }

        protected virtual void DoJump()
        {
            // Update held down timer

            if (_jumpButtonPressed)
                _jumpButtonHeldDownTime += Time.deltaTime;

            // Wants to jump and not already jumping..

            if (_jumpButtonPressed && !IsJumping())
            {
                // If jumpPreGroundedTime is enabled,
                // allow to jump only if held down time is less than tolerance

                if (jumpPreGroundedTime > 0.0f)
                {
                    bool canJump = _jumpButtonHeldDownTime <= jumpPreGroundedTime;
                    if (!canJump)
                        return;
                }

                // Can perform the requested jump ?
                
                if (!CanJump())
                    return;

                // Jump!

                characterMovement.PauseGroundConstraint();
                characterMovement.LaunchCharacter(CalcJumpImpulse(), true);
                
                _jumpCount++;
                _isJumping = true;
            }
        }

        protected virtual void Jumping()
        {
            // Check jump input state and attempts to do the requested jump

            DoJump();

            // Perform jump hold, applies an opposite gravity force proportional to _jumpHoldTime.

            if (IsJumping() && _jumpButtonPressed && jumpMaxHoldTime > 0.0f && _jumpHoldTime < jumpMaxHoldTime)
            {
                Vector3 actualGravity = GetGravity();

                float actualGravityMagnitude = actualGravity.magnitude;
                Vector3 actualGravityDirection = actualGravityMagnitude > 0.0f
                    ? actualGravity / actualGravityMagnitude
                    : Vector3.zero;

                float jumpProgress = Mathf.InverseLerp(0.0f, jumpMaxHoldTime, _jumpHoldTime);
                float proportionalForce = Mathf.LerpUnclamped(actualGravityMagnitude, 0.0f, jumpProgress);

                Vector3 proportionalJumpForce = -actualGravityDirection * proportionalForce;
                characterMovement.AddForce(proportionalJumpForce);

                _jumpHoldTime += Time.deltaTime;
            }

            // If 'falling' update falling time

            if (!characterMovement.isGrounded)
                _fallingTime += Time.deltaTime;
            else if (!characterMovement.wasGrounded)
            {
                // If landed, reset jump info

                _jumpCount = 0;
                _fallingTime = 0.0f;
            }
        }
        
        protected virtual void Move()
        {
            // Get desired velocity

            Vector3 desiredVelocity = GetDesiredVelocity();

            // Handle crouching state

            Crouching();

            // Handle Jumping state

            Jumping();

            // Perform character movement using the CharacterMovement built-in friction based movement

            float actualBrakingFriction = useSeparateBrakingFriction ? brakingFriction : GetFriction();

            characterMovement.SimpleMove(desiredVelocity, GetMaxSpeed(), GetMaxAcceleration(),
                GetMaxBrakingDeceleration(), GetFriction(), actualBrakingFriction, GetGravity());
        }

        protected virtual void HandleInput()
        {
            // Movement input (in world space)

            Vector3 movementDirection = Vector3.zero;

            movementDirection += Vector3.right * Input.GetAxisRaw($"Horizontal");
            movementDirection += Vector3.forward * Input.GetAxisRaw($"Vertical");

            movementDirection = Vector3.ClampMagnitude(movementDirection, 1.0f);

            SetMovementDirection(movementDirection);

            // Jump input
            
            if (Input.GetButtonDown($"Jump"))
                Jump();
            else if (Input.GetButtonUp($"Jump"))
                StopJumping();

            // Crouch input

            if (Input.GetKeyDown(KeyCode.C))
                Crouch();
            else if (Input.GetKeyUp(KeyCode.C))
                StopCrouching();
        }

        protected virtual void OnReset()
        {
            _rotationRate = 540.0f;

            _maxSpeed = 6.0f;
            _minAnalogSpeed = 0.0f;
            _maxAcceleration = 20.0f;
            _groundBrakingDeceleration = 20.0f;
            _groundFriction = 8.0f;

            _useSeparateBrakingFriction = false;
            _brakingFriction = 0.0f;

            _airBrakingDeceleration = 0.0f;
            _airFriction = 0.5f;
            _airControl = 0.3f;

            _canEverCrouch = false;
            _unCrouchedHeight = 2.0f;
            _crouchedHeight = 1.0f;
            _crouchedSpeedModifier = 0.5f;

            _jumpMaxCount = 1;
            _jumpImpulse = 6.0f;
            _jumpMaxHoldTime = 0.35f;
            _jumpPreGroundedTime = 0.15f;
            _jumpPostGroundedTime = 0.15f;

            _gravity = Physics.gravity;
            _gravityScale = 1.0f;
        }

        protected virtual void OnOnValidate()
        {
            rotationRate = _rotationRate;

            maxSpeed = _maxSpeed;
            minAnalogSpeed = _minAnalogSpeed;
            maxAcceleration = _maxAcceleration;
            groundBrakingDeceleration = _groundBrakingDeceleration;
            groundFriction = _groundFriction;

            brakingFriction = _brakingFriction;

            airBrakingDeceleration = _airBrakingDeceleration;
            airFriction = _airFriction;
            airControl = _airControl;

            unCrouchedHeight = _unCrouchedHeight;
            crouchedHeight = _crouchedHeight;
            crouchedSpeedModifier = _crouchedSpeedModifier;

            jumpMaxCount = _jumpMaxCount;
            jumpImpulse = _jumpImpulse;
            jumpMaxHoldTime = _jumpMaxHoldTime;
            jumpPreGroundedTime = _jumpPreGroundedTime;
            jumpPostGroundedTime = _jumpPostGroundedTime;

            gravityScale = _gravityScale;
        }

        protected virtual void OnAwake()
        {
            // Cache components

            _transform = GetComponent<Transform>();
            _characterMovement = GetComponent<CharacterMovement>();

            // By default enable late fixed update
            
            enableLateFixedUpdate = true;

            // Enable platform and physics interactions
            
            characterMovement.impartPlatformMovement = true;
            characterMovement.impartPlatformRotation = true;
            characterMovement.impartPlatformVelocity = true;

            characterMovement.enablePhysicsInteraction = true;
        }

        protected virtual void OnStart()
        {
            // DEFAULT EMPTY
        }

        protected virtual void OnOnEnable()
        {
            if (_enableLateFixedUpdateCoroutine)
                EnableLateFixedUpdate(true);
        }

        protected virtual void OnOnDisable()
        {
            if (_enableLateFixedUpdateCoroutine)
                EnableLateFixedUpdate(false);
        }

        protected virtual void OnFixedUpdate()
        {
            // DEFAULT EMPTY
        }

        protected virtual void OnLateFixedUpdate()
        {
            UpdateRotation();

            Move();
        }
        
        protected virtual void OnUpdate()
        {
            HandleInput();
        }

        #endregion

        #region MONOBEHAVIOUR

        private protected void Reset()
        {
            OnReset();
        }

        private protected void OnValidate()
        {
            OnOnValidate();
        }

        private protected virtual void Awake()
        {
            OnAwake();
        }

        private protected void Start()
        {
            OnStart();
        }

        private protected virtual void OnEnable()
        {
            OnOnEnable();
        }

        private protected virtual void OnDisable()
        {
            OnOnDisable();
        }

        private protected virtual void FixedUpdate()
        {
            OnFixedUpdate();
        }

        private protected virtual void Update()
        {
            OnUpdate();
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

        #endregion
    }
}
