using System;
using Root.Controller;
using UnityEngine;

namespace Root
{
    [RequireComponent(typeof(BoxCollider))]
    public class JumpTrigger : MonoBehaviour
    {
        [SerializeField] private Transform boardingPoint;
        [SerializeField] private float maxAngle;

        private MovementController _movementController;
        private CameraController _cameraController;
        private bool _playerIsTouchingTrigger;

        private void Start()
        {
            _movementController = GameManager.Player.GetComponent<MovementController>();
            _cameraController = GameManager.Player.GetComponent<CameraController>();

            GameManager.Input.Movement.Jump.performed += TryBoard;
        }

        private void OnDestroy()
        {
            GameManager.Input.Movement.Jump.performed -= TryBoard;
        }

        private void TryBoard(UnityEngine.InputSystem.InputAction.CallbackContext _)
        {
            Transform player = GameManager.Player.transform;

            if (_movementController.GetState() != CharacterState.Grounded || !_playerIsTouchingTrigger)
                return;
            Vector3 movementOffset = boardingPoint.position - transform.position;
            movementOffset.y = 0;

            if (Vector3.Angle(movementOffset, _cameraController.GetHorizontalDirectionForwardVector().Swizzle_x0y()) > maxAngle)
                return;
            
            movementOffset.y = boardingPoint.position.y - player.position.y + player.GetComponent<CapsuleCollider>().height / 2f;

            player.position += movementOffset;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == GameManager.Player.gameObject)
                _playerIsTouchingTrigger = true;
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject == GameManager.Player.gameObject)
                _playerIsTouchingTrigger = false;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;

            const float approximateResolution = 10f;

            var back  = transform.TransformPoint(new Vector3(0, 0, -0.5f));
            var front = transform.TransformPoint(new Vector3(0, 0,  0.5f));

            var totalLength = Vector3.Distance(back, front);
            var divisionAmount = Mathf.Ceil(Mathf.Max(1, Mathf.RoundToInt(totalLength / approximateResolution)) / 2f) * 2;
            
            var offset = boardingPoint.position - transform.position;
            var vertical = transform.up * Vector3.Dot(offset, transform.up);
            var horizontal = offset - vertical;

            for (int i = 0; i <= divisionAmount; i++)
            {
                var start = Vector3.Lerp(back, front, i / (float)divisionAmount);
                var corner = start + vertical;
                
                DrawArrow(start, corner, transform.right);
                DrawArrow(corner, corner + horizontal, transform.up);
            }
        }

        private static void DrawArrow(Vector3 from, Vector3 to, Vector3 axis, float headLength = 0.4f, float headAngle = 25f)
        {
            var delta = to - from;
            if (delta.sqrMagnitude == 0f) return;

            Gizmos.DrawLine(from, to);

            var dir = delta.normalized;
            headLength = Mathf.Min(headLength, delta.magnitude * 0.4f);
            
            var wing = -dir * headLength;
            Gizmos.DrawLine(to, to + Quaternion.AngleAxis( headAngle, axis) * wing);
            Gizmos.DrawLine(to, to + Quaternion.AngleAxis(-headAngle, axis) * wing);
        }
    }
}