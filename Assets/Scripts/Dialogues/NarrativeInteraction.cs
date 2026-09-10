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
            if (!TryFindInteractableNPC(out var currentInteractable))
            {
                _currentInterractable = null;
                interactionPanel.enabled = false;
                return;
            }

            _currentInterractable = currentInteractable;

            ShowCanvas();
        }

        private void ShowCanvas()
        {
            bool canInteract = !_isInteracting && !_currentInterractable.HasDialoguePermenantlyEnded();
            interactionPanel.enabled = canInteract;
        }

        private void HandleNarrativeInteraction(InputAction.CallbackContext _)
        {
            if (TryFindInteractableNPC(out var currentInteractable))
            {
                if (!currentInteractable.HasDialogue()) return;

                if (currentInteractable.HasDialoguePermenantlyEnded()) return;

                if (currentInteractable.CheckPivot() && currentInteractable.CheckPosPivot())
                {
                    _npcLookingPivot = currentInteractable.Pivot;
                    _npcPositionPivot = currentInteractable.PlayerPivot;

                    GameManager.Player.GetComponent<MovementController>().CenterPlayerDialogueInteraction(cameraPivot, _npcPositionPivot.position);
                    GameManager.Player.GetComponent<CameraController>().FocusCamera(_npcLookingPivot);
                }

                currentInteractable.OnInteractionStarted += StartInteraction;
                currentInteractable.OnInteractionEnded += EndInteraction;

                currentInteractable.ExecuteDialogue();
            }
        }

        private bool TryFindInteractableNPC(out InteractBehaviour interactable)
        {
            interactable = null;
            return Physics.Raycast(cameraPivot.position, cameraPivot.forward, out RaycastHit raycastHit, maxDistance, interactableEntityLayer) &&
                raycastHit.collider.gameObject.TryGetComponent(out interactable);
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
