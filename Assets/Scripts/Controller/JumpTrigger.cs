using Root.Controller;
using UnityEngine;

namespace Root
{
    [RequireComponent(typeof(BoxCollider))]
    public class JumpTrigger : MonoBehaviour
    {
        [SerializeField] private Transform boardingPoint;
        [SerializeField] private float maxDistance;
        [SerializeField] private float maxAngle;

        private MovementController _movementController;
        private CameraController _cameraController;
        private BoxCollider _boxCollider;

        private void Start()
        {
            _movementController = GameManager.Player.GetComponent<MovementController>();
            _cameraController = GameManager.Player.GetComponent<CameraController>();

            _boxCollider = GetComponent<BoxCollider>();

            GameManager.Input.Movement.Jump.performed += TryBoard;
        }

        private void OnDestroy()
        {
            GameManager.Input.Movement.Jump.performed -= TryBoard;
        }

        private void TryBoard(UnityEngine.InputSystem.InputAction.CallbackContext _)
        {
            Transform player = GameManager.Player.transform;

            // Condición 1: estar debajo del punto de entrada
            if (player.position.y >= boardingPoint.position.y)
                return;

            // Condición 2: estar tocando el suelo
            if (_movementController.GetState() != CharacterState.Grounded)
                return;

            // Condición 3: estar suficientemente cerca
            if (Vector3.Distance(player.position, transform.position) > maxDistance)
                return;

            // Condición 4: mirar hacia el trigger dentro del ángulo permitido
            Vector3 dirToTrigger = (transform.position - player.position).normalized;

            Vector2 forward = _cameraController.GetHorizontalDirectionForwardVector();
            Vector3 playerForward = new Vector3(forward.x, 0f, forward.y);

            float angle = Vector3.Angle(playerForward, dirToTrigger);

            if (angle > maxAngle)
                return;

            // Centro real del trigger 
            Vector3 triggerCenter = transform.TransformPoint(_boxCollider.center);

            // Offset entre el centro del trigger y el objetivo 
            Vector3 offset = boardingPoint.position - triggerCenter;

            // Mantener la posición relativa dentro del trigger
            Vector3 targetPosition = player.position + new Vector3(offset.x, 0f, offset.z);

            // Forzar la altura de la plataforma
            targetPosition.y = boardingPoint.position.y;

            player.position = targetPosition;
        }
    }
}