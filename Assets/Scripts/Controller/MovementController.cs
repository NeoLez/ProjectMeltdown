using UnityEngine;
using UnityEngine.InputSystem;

namespace Root.Controller
{
    [RequireComponent(typeof(Rigidbody), typeof(CameraController), typeof(CapsuleCollider))]
    public class MovementController : MonoBehaviour
    {
        private PlayerInputActions _input;
        private Rigidbody _rb;
        private CameraController _cameraController;
        private CapsuleCollider _collider;
        [SerializeField] private CharacterState _currentState;

        [SerializeField] private float groundCheckRayLength;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private float movementSpeed;
        [SerializeField] private float runningMovementSpeed;
        [SerializeField] private float verticalJumpSpeed;

        [SerializeField] private Vector3 _currentSurfaceNormal;
        [SerializeField] private float _currentSlopeAngle;
        [SerializeField] private float maxSlopeAngle;
        [SerializeField] private PhysicsMaterial _currentPhysicsMaterial;

        [SerializeField] private float currentVerticalSpeed;

        [Header("Footsteps")]
        [SerializeField] private float footstepDistance = 1.5f;
        [SerializeField] private float runningFootstepDistance = 2.5f;

        private bool jump;
        private float _distanceTravelled;
        private Vector3 _lastPosition;

        private void Start()
        {
            _input = GameManager.Input;
            _rb = GetComponent<Rigidbody>();
            _cameraController = GetComponent<CameraController>();
            _collider = GetComponent<CapsuleCollider>();

            _input.Movement.Enable();
            _input.Movement.Jump.performed += Jump;

            _lastPosition = transform.position;
        }

        private void OnDestroy()
        {
            _input.Movement.Jump.performed -= Jump;
        }

        private void FixedUpdate()
        {
            if (jump)
            {
                currentVerticalSpeed = verticalJumpSpeed;
                jump = false;
            }

            UpdateState();

            if (_currentState == CharacterState.Grounded)
            {
                currentVerticalSpeed = 0;
            }
            else
            {
                currentVerticalSpeed += Physics.gravity.y * Time.deltaTime;
            }

            Vector2 input = _input.Movement.MoveDir.ReadValue<Vector2>();
            Vector3 worldMoveDir = (_cameraController.GetHorizontalDirectionForwardVector() * input.y +
                                    _cameraController.GetHorizontalDirectionRightVector() * input.x).Swizzle_x0y();

            
            
            _rb.linearVelocity = worldMoveDir * (_input.Movement.Run.IsPressed() ? runningMovementSpeed : movementSpeed) + Vector3.up * currentVerticalSpeed;

            if (_currentState == CharacterState.Grounded && _rb.linearVelocity.magnitude > 0.1f)
            {
                _distanceTravelled += Vector3.Distance(transform.position, _lastPosition);
                if (_distanceTravelled >= (_input.Movement.Run.IsPressed() ? runningFootstepDistance : footstepDistance))
                {
                    _distanceTravelled = 0;
                    PlayFootstep();
                }
            }

            _lastPosition = transform.position;
        }

        private Vector3 prevLocalPos;

        private void UpdateState()
        {
            if (currentVerticalSpeed > 0)
            {
                _currentState = CharacterState.Air;
                _currentSurfaceNormal = Vector3.up;
                return;
            }
            if (Physics.SphereCast(transform.position, _collider.radius - 0.001f, Vector3.down, out RaycastHit hit,
                    groundCheckRayLength, groundLayer))
            {
                if (Physics.Raycast(hit.point + Vector3.up, Vector3.down, out hit, 2f, groundLayer))
                {
                    _currentSurfaceNormal = hit.normal;
                    _currentSlopeAngle = Vector3.Angle(_currentSurfaceNormal, Vector3.up);

                    if (_currentSlopeAngle <= maxSlopeAngle)
                    {
                        _currentState = CharacterState.Grounded;
                        _currentPhysicsMaterial = hit.collider.gameObject.GetComponent<Collider>().material;
                    }
                    else
                    {
                        _currentState = CharacterState.Air;
                        _currentSurfaceNormal = Vector3.up;
                    }
                }
            }
            else
            {
                _currentSurfaceNormal = Vector3.up;
                _currentState = CharacterState.Air;
            }
        }

        private void PlayFootstep()
        {
            SoundSystem.MaterialType material = SoundSystem.MaterialType.None;

            if (Physics.SphereCast(
                    transform.position,
                    _collider.radius - 0.001f,
                    Vector3.down,
                    out RaycastHit hit,
                    groundCheckRayLength,
                    groundLayer))
            {
                if (hit.collider.TryGetComponent(out SoundSystem.MaterialTypeComponent matComp))
                    material = matComp.materialType;
            }

            var so = SoundSystem.FootstepMaterialDatabase.Dictionary[material];
            so?.PlaySound();
        }

        private void Jump(InputAction.CallbackContext _)
        {
            if (_currentState == CharacterState.Grounded)
            {
                jump = true;
            }
        }
        
        /*public void AdjustMovementSpeedWithSlope() {
            if (_currentSurfaceNormal != Vector3.zero && _currentSurfaceNormal != Vector3.up) {
                float slopeSpeedCoefficient = angleBasedSpeedLimit.Evaluate(_currentSlopeAngle / 90f);
            
                //Calculates the two basis vectors for the new reference system that is relative to the plane rotation
                Vector3 planeOppositeX = Vector3.ProjectOnPlane(_currentSurfaceNormal, Vector3.up);
                Vector3 planeOppositeY = planeOppositeX.Swizzle_zyx();
                planeOppositeY.x *= -1;
            
                //Transforms the speed to that system (matrix multiplication)
                _speed = new Vector3(planeOppositeX.x * _speed.x + planeOppositeY.x * _speed.z, _speed.y, planeOppositeX.z * _speed.x + planeOppositeY.z * _speed.z);
            
                //Adjusts the component of the velocity that goes toward the plane
                if (_speed.x > 0)
                    _speed.x *= slopeSpeedCoefficient;

                //Calculates the inverse of the matrix above, and uses it to go back to the normal basis
                float det = planeOppositeX.x * planeOppositeY.z - planeOppositeY.x * planeOppositeX.z;
                planeOppositeX /= det;
                planeOppositeY /= det;
                _speed = new Vector3(planeOppositeY.z * _speed.x - planeOppositeY.x * _speed.z, _speed.y, -planeOppositeX.z * _speed.x + planeOppositeX.x * _speed.z);
            }
        }*/

        public CharacterState GetState()
        {
            return _currentState;
        }
    }
}

public enum CharacterState
{
    Grounded,
    Air,
}