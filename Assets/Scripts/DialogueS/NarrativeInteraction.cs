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
        private bool isInteracting;

        private void Awake()
        {
            _input = GameManager.Input;
            _input.Interaction.Enable();
            _input.Interaction.NPC.performed += HandleNarrativeInteraction;
        }

        private void Update()
        {
            if (isInteracting)
            {
                interactionPanel.enabled = false;
                return;
            }

            if (TryFindInteractableNPC(out var currentInteractable))
            {
                interactionPanel.enabled = true;
            }
            else
            {
                interactionPanel.enabled = false;
            }
        }

        private void HandleNarrativeInteraction(InputAction.CallbackContext _)
        {
            if (TryFindInteractableNPC(out var currentInteractable))
            {
                if (currentInteractable.CheckPivot() && currentInteractable.CheckPosPivot())
                {
                    if (currentInteractable.HasDialogueEnded())
                    {
                        //isInteracting = false; //fijarme la prox si tiene dialogo hago que siga interactuando 
                        return;
                    }

                    _npcLookingPivot = currentInteractable.Pivot;
                    _npcPositionPivot = currentInteractable.PlayerPivot;

                    GameManager.Player.GetComponent<MovementController>().CenterPlayerDialogueInteraction(cameraPivot, _npcPositionPivot.position);
                    GameManager.Player.GetComponent<CameraController>().FocusCamera(_npcLookingPivot);

                    isInteracting = true;
                }

                currentInteractable.ExecuteDialogue();
            }
        }

        private bool TryFindInteractableNPC(out InteractBehaviour interactable) //evitar poder interactuar con otros cuando estoy ya con uno
        {
            interactable = null;
            return Physics.Raycast(cameraPivot.position, cameraPivot.forward, out RaycastHit raycastHit, maxDistance, interactableEntityLayer) && raycastHit.collider.gameObject.TryGetComponent(out interactable);
        }

        private void OnDestroy()
        {
            _input.Interaction.NPC.performed -= HandleNarrativeInteraction;
        }

    }
}
