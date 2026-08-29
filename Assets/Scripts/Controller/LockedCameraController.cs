using UnityEngine;
using UnityEngine.InputSystem;

namespace Root.Controller {
    public class LockedCameraController : MonoBehaviour {
        [SerializeField] private float lerpSensitivity;
        [SerializeField] private float MouseSensitivity;
        [SerializeField] private float Speed;
        [SerializeField] private CameraPivot cam;
        [SerializeField] private Transform cameraTarget;
        [SerializeField] public float interactDistance = 2;

        private PlayerInputActions _input;
        private Vector2 _prevMousePos = Vector2.zero;

        private void Awake() {
            _input = GameManager.Input;
            _input.Interaction.Interact.started += HandleInteraction;
            _input.Interaction.Interact.canceled += HandleInteraction;
        }

        private void OnDestroy() {
            _input.Interaction.Interact.started -= HandleInteraction;
            _input.Interaction.Interact.canceled -= HandleInteraction;
        }

        private void HandleInteraction(InputAction.CallbackContext ctx)
        {
            if (!isActiveAndEnabled) return;
            if (ctx.started) HandleInteractionObjectSelection();
            
            if (ctx.started)
            {
                if (_selectedInteractable == null) return;
                _selectedInteractable.StartInteraction();
            }
            else {
                _selectedInteractable?.EndInteraction();
                _selectedInteractable = null;
            }
        }

        private void Update() {
            Vector2 mousePos = GameManager.Input.CameraMovement.MousePosition.ReadValue<Vector2>();
            
            mousePos.y -= Screen.height / 2;
            mousePos.x -= Screen.width / 2;

            mousePos.y /= Screen.height / MouseSensitivity;
            mousePos.x /= Screen.width / MouseSensitivity;
            
            cam.transform.position = cameraTarget.position;
            
            Quaternion targetRotation = cameraTarget.rotation * Quaternion.Euler(-mousePos.y * lerpSensitivity, mousePos.x * lerpSensitivity, 0f);
            cam.transform.rotation = Quaternion.Lerp(cam.transform.rotation, targetRotation, Speed * Time.deltaTime);
            
            _prevMousePos = mousePos;
        }

        private void OnEnable() {
            MouseHandler.RequestControl(CursorLockMode.Confined, true, this);
            _input.Interaction.GoBack.started += GoBack;
        }

        private void OnDisable() {
            _selectedInteractable?.EndInteraction();
            MouseHandler.RelinquishControl(this);
            _input.Interaction.GoBack.started -= GoBack;
        }

        private void GoBack(InputAction.CallbackContext _) {
            GameManager.Player.GetComponent<CameraController>().enabled = true;
            _input.Movement.Enable();
            enabled = false;
        }
        
        private Interactable _selectedInteractable;
        private void HandleInteractionObjectSelection() {
            Ray ray = cam.ScreenPointToRay(_input.CameraMovement.MousePosition.ReadValue<Vector2>()/GameManager.GetResolutionRatio());
            if (!Physics.Raycast(ray, out var hit, interactDistance) ||
                !hit.collider.gameObject.TryGetComponent<Interactable>(out var component)) {
                _selectedInteractable = null;
                return;
            }
            _selectedInteractable = component;
        }
        
        public void SyncToRotation(Quaternion targetWorldRotation)
        {
            cam.transform.rotation = targetWorldRotation;
        }
    }
}