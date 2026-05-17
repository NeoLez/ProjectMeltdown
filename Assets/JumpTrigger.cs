using Root.Controller;
using UnityEngine;

namespace Root
{
    public class TrainBoardingTrigger : MonoBehaviour
    {
        [SerializeField] private Transform boardingPoint;
        [SerializeField] private float maxDistance;
        [SerializeField] private float maxAngle;

        private MovementController _movementController;
        private CameraController _cameraController;

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
            var player = GameManager.Player.transform;

            // Condicion 1: estar abajo del punto de referencia
            if (player.position.y >= boardingPoint.position.y) return;
            // Condicion 2: estar en el piso
            if (_movementController.GetState() != CharacterState.Grounded) return;
            // Condicion 3: estar en la distancia maxima
            if (Vector3.Distance(player.position, transform.position) > maxDistance) return;
            // Condicion 4: mirar hacia el tren dentro del angulo permitido
            Vector3 dirToTrain = (transform.position - player.position).normalized;
            Vector2 forwardPlayer = _cameraController.GetHorizontalDirectionForwardVector();
            Vector3 playerForward = new Vector3(forwardPlayer.x, 0, forwardPlayer.y);
            float angle = Vector3.Angle(playerForward, dirToTrain);
            if (angle > maxAngle) return;
            // Si se cumple todo lo mencionado anteriormente, teletrasporta al jugador en el punto de referencia
            player.position = boardingPoint.position;
        }
    }
}