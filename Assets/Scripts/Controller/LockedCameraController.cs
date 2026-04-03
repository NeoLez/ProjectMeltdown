using UnityEngine;
using UnityEngine.InputSystem;

namespace Root.Controller {
    public class LockedCameraController : MonoBehaviour {
        [SerializeField] private float MouseSensitivity;
        [SerializeField] private float Speed;
        [SerializeField] private float MovementSpeed;
        [SerializeField] private Transform cam;
        [SerializeField] private Transform mouseTarget;

        private PlayerInputActions _input;
        private Vector2 _prevMousePos = Vector2.zero;

        private void Awake() {
            _input = GameManager.Input;
        }

        private void Update() {
            Vector2 mousePos = GameManager.Input.CameraMovement.MousePosition.ReadValue<Vector2>();
            
            mousePos.y -= Screen.height / 2;
            mousePos.x -= Screen.width / 2;
            
            cam.position = Vector3.Lerp(cam.position, mouseTarget.position, MovementSpeed);
            
            Quaternion targetRotation = mouseTarget.rotation * Quaternion.Euler(-mousePos.y * MouseSensitivity, mousePos.x * MouseSensitivity, 0f);
            cam.rotation = Quaternion.Lerp(cam.rotation, targetRotation, Speed);
            
            _prevMousePos = mousePos;
        }

        private void OnEnable() {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
            cam = Camera.main.transform;
            _input.Interaction.GoBack.started += GoBack;
        }

        private void OnDisable() {
            _input.Interaction.GoBack.started -= GoBack;
        }

        private void GoBack(InputAction.CallbackContext _) {
            GameManager.Player.GetComponent<CameraController>().enabled = true;
            GameManager.Player.GetComponent<MovementController>().enabled = true;
            enabled = false;
        }
    }
}