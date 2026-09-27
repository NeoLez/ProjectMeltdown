using Root.Controller;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Root
{
    public class NarrativeInteraction : MonoBehaviour
    {
        //[SerializeField] Transform playerPos;
        [SerializeField] private Transform cameraPivot;
        [SerializeField] float maxDistance;
        [SerializeField] LayerMask interactableEntityLayer;
        [SerializeField] private Canvas interactionPanel;

        private PlayerInputActions _input;
        private Transform _npcLookingPivot;
        private Transform _npcPositionPivot;
        private Vector3 _newPlayerPosition;
        private bool _isInteracting;
        private InteractBehaviour _currentInterractable;

        private void Awake()
        {
            _input = GameManager.Input;
            _input.Interaction.Enable();
            _input.Interaction.NPC.performed += HandleNarrativeInteraction;
        }

        private void Update()
        {
            if (TryFindInteractableNPC(out var currentInteractable) && !_isInteracting)
            {
                _currentInterractable = currentInteractable;
            }
            else
            {
                if(!_isInteracting) _currentInterractable = null;

                interactionPanel.enabled = false;
                return;
            }
            ShowCanvas();
        }

        private void ShowCanvas()
        {
            bool canInteract = !_isInteracting && !_currentInterractable.HasDialoguePermenantlyEnded();
            interactionPanel.enabled = canInteract;
        }

        bool _hasEnteredOnce;
        private void HandleNarrativeInteraction(InputAction.CallbackContext _)
        {
            if(_currentInterractable != null)
            {
                if (!_currentInterractable.HasDialogue()) return;

                if (_currentInterractable.HasDialoguePermenantlyEnded()) return;

                if (_currentInterractable.CheckPivot() && _currentInterractable.CheckPosPivot() && !_hasEnteredOnce)
                {
                    SetLookAndPositionPivots(_currentInterractable);

                    GameManager.Player.GetComponent<MovementController>().CenterPlayerDialogueInteraction(cameraPivot, _newPlayerPosition);
                    GameManager.Player.GetComponent<CameraController>().FocusCamera(_npcLookingPivot);

                    _hasEnteredOnce = true;
                }
                else
                {
                    GameManager.Player.GetComponent<CameraController>().FocusCamera(null);
                }

                _currentInterractable.OnInteractionStarted += StartInteraction;
                _currentInterractable.OnInteractionEnded += EndInteraction;

                _currentInterractable.ExecuteDialogue();
            }
        }

        private bool TryFindInteractableNPC(out InteractBehaviour interactable)
        {
            interactable = null;
            return Physics.Raycast(cameraPivot.position, cameraPivot.forward, out RaycastHit raycastHit, maxDistance, interactableEntityLayer) &&
                raycastHit.collider.gameObject.TryGetComponent(out interactable);
        }

        private void SetLookAndPositionPivots(InteractBehaviour currentInteractable)
        {
            _npcLookingPivot = currentInteractable ? currentInteractable.Pivot : null;
            _npcPositionPivot = currentInteractable ? currentInteractable.PlayerPivot : null;

            _newPlayerPosition = new Vector3(_npcPositionPivot.position.x, transform.position.y, _npcPositionPivot.position.z);
        }

        private void StartInteraction()
        {
            _isInteracting = true;
            if (_currentInterractable != null)
            {
                _currentInterractable.OnInteractionStarted -= StartInteraction;
            }

        }
        private void EndInteraction()
        {
            _isInteracting = false;
            if(_currentInterractable.CheckPivot()) GameManager.Player.GetComponent<CameraController>().FocusCamera(_npcLookingPivot);
            if (_currentInterractable != null)
            {
                _currentInterractable.OnInteractionEnded -= EndInteraction;
            }
        }

        private void OnDestroy()
        {
            _input.Interaction.NPC.performed -= HandleNarrativeInteraction;
        }

    }
}
