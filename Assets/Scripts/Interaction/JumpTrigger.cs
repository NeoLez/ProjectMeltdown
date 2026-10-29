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
            var offset = boardingPoint.position - transform.position;
            Gizmos.DrawLine(transform.position, transform.position + new Vector3(0,offset.y,0));
            Gizmos.DrawLine(transform.position + new Vector3(0,offset.y,0), transform.position + offset);
        }
    }
}