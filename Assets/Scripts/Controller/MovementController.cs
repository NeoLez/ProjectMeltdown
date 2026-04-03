using UnityEngine;
using UnityEngine.InputSystem;

namespace Root.Controller {
    [RequireComponent(typeof(Rigidbody), typeof(CameraController), typeof(CapsuleCollider))]
    public class MovementController : MonoBehaviour {
        private PlayerInputActions _input;
        private Rigidbody _rb;
        private CameraController _cameraController;
        private CapsuleCollider _capsuleCollider;

        [SerializeField] private float movementSpeed;
        [SerializeField] private float groundCheckRayLength;
        [SerializeField] private LayerMask groundLayer;


        [SerializeField] private float airMovementSnappiness;
        [SerializeField] private float defaultGroundMovementSnappiness;

        [SerializeField] private AnimationCurve angleBasedSpeedLimit;

        [SerializeField] private float maxSlopeAngle;
        private float _currentSlopeAngle;
        [SerializeField] private CharacterState _currentState = CharacterState.Grounded;
        private Vector3 _currentSurfaceNormal;

        void Start() {
            _input = GameManager.Input;

            _rb = GetComponent<Rigidbody>();
            _cameraController = GetComponent<CameraController>();
            _capsuleCollider = GetComponent<CapsuleCollider>();

            _input.Movement.Jump.started += InputJumpStarted;
            _input.Movement.Jump.canceled += InputJumpEnded;
        }

        private Vector2 _moveDir = Vector2.zero;

        void Update() {
            Vector2 input = _input.Movement.MoveDir.ReadValue<Vector2>();
            _moveDir = input;
        }

        private Vector3 _prevSpeed = Vector3.zero;
        private Vector3 _speed = Vector3.zero;
        private Vector3 _gravitySpeed = Vector3.zero;
        private Vector3 _prevJumpSpeed = Vector3.zero;
        private Vector3 _targetGroundSpeed = Vector3.zero;
        private Vector3 _currentGroundSpeed = Vector3.zero;
        private PhysicsMaterial _currentPhysicsMaterial;

        private void FixedUpdate() {
            UpdateMovementState();
            HandleMovement();
            HandleJumping();

            _rb.linearVelocity = _speed + _gravitySpeed + _prevJumpSpeed + _currentGroundSpeed;
        }

        public void SetGroundSpeed(Vector3 groundSpeed) {
            _targetGroundSpeed = groundSpeed;
        }

        private void HandleMovement() {
            Vector3 worldMoveDir = (_cameraController.GetHorizontalDirectionForwardVector() * _moveDir.y +
                                    _cameraController.GetHorizontalDirectionRightVector() * _moveDir.x).Swizzle_x0y();
            _speed = worldMoveDir * movementSpeed;

            switch (_currentState) {
                case CharacterState.Air:
                    _speed = Vector3.Lerp(_prevSpeed, _speed, airMovementSnappiness);
                    _currentGroundSpeed = Vector3.Lerp(_currentGroundSpeed, _targetGroundSpeed,
                        airMovementSnappiness * 0.05f);
                    _gravitySpeed += Physics.gravity * Time.fixedDeltaTime;
                    break;
                case CharacterState.Grounded:
                    float groundSlipperiness = defaultGroundMovementSnappiness;
                    if (_currentPhysicsMaterial != null) {
                        groundSlipperiness = _currentPhysicsMaterial.dynamicFriction;
                    }
                    
                    _speed = Vector3.ProjectOnPlane(_speed, _currentSurfaceNormal);
                    _speed = Vector3.Lerp(_prevSpeed, _speed, groundSlipperiness);
                    _currentGroundSpeed = Vector3.Lerp(_currentGroundSpeed, _targetGroundSpeed, groundSlipperiness);
                    if (_gravitySpeed.y < 0) //Never make gravitySpeed 0 if it points away from the floor (caused by jump). 
                        _gravitySpeed =
                            Vector3.zero; //Set to 0 to prevent it from accumulating speed while the player is standing.
                    break;
                case CharacterState.Sliding:
                    _speed = Vector3.ProjectOnPlane(_speed, _currentSurfaceNormal);
                    if (_speed.y > 0) //Prevent player from climbing up the slope under any circumstance
                        _speed.y = 0;

                    _speed = Vector3.Lerp(_prevSpeed, _speed, defaultGroundMovementSnappiness);
                    _currentGroundSpeed = Vector3.Lerp(_currentGroundSpeed, _targetGroundSpeed,
                        airMovementSnappiness * 0.05f);

                    //Remove horizontal components from gravity vector and add them to the character velocity
                    //This makes sure that the lateral speed gained from sliding in slope lerps correctly when getting out of the slope
                    _gravitySpeed += Vector3
                        .ProjectOnPlane(Physics.gravity * Time.fixedDeltaTime, _currentSurfaceNormal).Swizzle_0y0();
                    Vector3 gravityClone = _gravitySpeed.Swizzle_xyz();
                    gravityClone.y = 0;
                    _speed += gravityClone;
                    break;
            }

            _prevSpeed = _speed;
        }

        public void AdjustMovementSpeedWithSlope() {
            if (_currentSurfaceNormal != Vector3.zero && _currentSurfaceNormal != Vector3.up) {
                float slopeSpeedCoefficient = angleBasedSpeedLimit.Evaluate(_currentSlopeAngle / 90f);

                //Calculates the two basis vectors for the new reference system that is relative to the plane rotation
                Vector3 planeOppositeX = Vector3.ProjectOnPlane(_currentSurfaceNormal, Vector3.up);
                Vector3 planeOppositeY = planeOppositeX.Swizzle_zyx();
                planeOppositeY.x *= -1;

                //Transforms the speed to that system (matrix multiplication)
                _speed = new Vector3(planeOppositeX.x * _speed.x + planeOppositeY.x * _speed.z, _speed.y,
                    planeOppositeX.z * _speed.x + planeOppositeY.z * _speed.z);

                //Adjusts the component of the velocity that goes toward the plane
                if (_speed.x > 0)
                    _speed.x *= slopeSpeedCoefficient;

                //Calculates the inverse of the matrix above, and uses it to go back to the normal basis
                float det = planeOppositeX.x * planeOppositeY.z - planeOppositeY.x * planeOppositeX.z;
                planeOppositeX /= det;
                planeOppositeY /= det;
                _speed = new Vector3(planeOppositeY.z * _speed.x - planeOppositeY.x * _speed.z, _speed.y,
                    -planeOppositeX.z * _speed.x + planeOppositeX.x * _speed.z);
            }
        }

        private void HandleJumping() {
            if (!_currentlyJumping && _currentState == CharacterState.Grounded)
                _currentJumps = maxJumps;

            if (!_currentlyJumping && ShouldStartJump() && _currentJumps > 0) {
                _currentlyJumping = true;
                _gravitySpeed = Vector3.up * initialJumpVelocity;
                _jumpStartTime = Time.fixedTime;
                _currentJumps--;
                
            }

            if (_currentlyJumping) {
                if (!_jumpInputRegistered) {
                    //Reset if player let go of the jump
                    _currentlyJumping = false;
                    _jumpInputRegistered = false;
                    _prevJumpSpeed = Vector3.zero;
                }
                else {
                    //Player keeps pressing jump
                    float percentage = (Time.fixedTime - _jumpStartTime) / maxJumpDuration;
                    if (percentage > 1.0) {
                        //Player pressed jump until the extra force was fully used
                        _currentlyJumping = false;
                        _jumpInputRegistered = false;
                        _prevJumpSpeed = Vector3.zero;
                    }
                    else {
                        //Continue applying constant upward force
                        _prevJumpSpeed = Vector3.up * (jumpCurve.Evaluate(percentage) * jumpHoldSpeed);
                    }
                }
            }
        }

        private void UpdateMovementState() {
            if (Physics.SphereCast(_rb.position, _capsuleCollider.radius - 0.001f, Vector3.down, out RaycastHit hit,
                    groundCheckRayLength, groundLayer)) {
                //Do another raycast since the normal vector obtained through the SphereCast collider are inaccurate
                if (Physics.Raycast(hit.point + Vector3.up, Vector3.down, out hit, 2f, groundLayer)) {
                    _currentSurfaceNormal = hit.normal;
                    _currentSlopeAngle = Vector3.Angle(_currentSurfaceNormal, Vector3.up);

                    if (_currentSlopeAngle <= maxSlopeAngle) {
                        _currentState = CharacterState.Grounded;
                        _currentPhysicsMaterial = hit.collider.gameObject.GetComponent<Collider>().material;
                    }
                    else {
                        _currentState = CharacterState.Sliding;
                    }
                }
            }
            else {
                _currentSurfaceNormal = Vector3.up;
                _currentState = CharacterState.Air;
            }
        }


        #region Jump

        [SerializeField] private float jumpInputBufferTime;
        private bool _jumpInputRegistered;
        private float _jumpInputStartTime;
        private bool _currentlyJumping;
        private float _jumpStartTime;
        private byte _currentJumps;
        [SerializeField] private byte maxJumps;
        [SerializeField] private float maxJumpDuration;
        [SerializeField] private float initialJumpVelocity;
        [SerializeField] private float jumpHoldSpeed;
        [SerializeField] private AnimationCurve jumpCurve;

        private void InputJumpStarted(InputAction.CallbackContext ctx) {
            _jumpInputRegistered = true;
            _jumpInputStartTime = Time.fixedTime;
        }

        private void InputJumpEnded(InputAction.CallbackContext ctx) {
            _jumpInputRegistered = false;
        }

        /// <summary>
        /// Returns whether the player should jump taking input buffer into account to jump immediately when player touches the ground
        /// </summary>
        private bool ShouldStartJump() {
            return _jumpInputRegistered && (
                _currentState == CharacterState.Air ||
                _currentState == CharacterState.Sliding ||
                _currentState == CharacterState.Grounded && Time.fixedTime - _jumpInputStartTime <= jumpInputBufferTime
            );
        }

        #endregion

        public CharacterState GetState() {
            return _currentState;
        }
    }
}

public enum CharacterState {
    Grounded,
    Sliding,
    Air,
}
