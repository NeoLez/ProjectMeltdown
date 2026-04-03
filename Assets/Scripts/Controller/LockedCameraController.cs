using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Root.Controller {
    public class LockedCameraController : MonoBehaviour {
        [SerializeField] private float MouseSensitivity;
        [SerializeField] private float Speed;
        [SerializeField] private Transform cam;
        [SerializeField] private Transform cameraTarget;
        [SerializeField] public float interactDistance = 2;

        private PlayerInputActions _input;
        private Vector2 _prevMousePos = Vector2.zero;

        private void Awake() {
            _input = GameManager.Input;
            
            _input.Interaction.Interact.started += HandleInteraction;
            _input.Interaction.Interact.canceled += HandleInteraction;
        }

        private void Update() {
            Vector2 mousePos = GameManager.Input.CameraMovement.MousePosition.ReadValue<Vector2>();
            
            mousePos.y -= Screen.height / 2;
            mousePos.x -= Screen.width / 2;
            
            cam.position = cameraTarget.position;
            
            Quaternion targetRotation = cameraTarget.rotation * Quaternion.Euler(-mousePos.y * MouseSensitivity, mousePos.x * MouseSensitivity, 0f);
            cam.rotation = Quaternion.Lerp(cam.rotation, targetRotation, Speed);
            
            _prevMousePos = mousePos;
            
            HandleInteractionObjectSelection();
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
            _input.Movement.Enable();
            enabled = false;
        }
        
        private Interactable _selectedInteractable;
        private void HandleInteractionObjectSelection() {
            Ray ray = Camera.main.ScreenPointToRay(_input.CameraMovement.MousePosition.ReadValue<Vector2>());
            if (!Physics.Raycast(ray, out var hit, interactDistance) ||
                !hit.collider.gameObject.TryGetComponent<Interactable>(out var component)) {
                return;
            }

            if (_selectedInteractable == component) return;
            _selectedInteractable = component;
            _selectedInteractable.Select(true);
        }

        private void DeselectItem() {
            if (_selectedInteractable == null) return;
        
            _selectedInteractable.Select(false);
            _selectedInteractable.Interact(false);
        }
    
        private void HandleInteraction(InputAction.CallbackContext ctx) {
            if (_selectedInteractable == null) return;
        
            _selectedInteractable.Interact(ctx.started);
        }
    }
}