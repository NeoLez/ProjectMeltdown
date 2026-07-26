using NUnit.Framework;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Root
{
    public class NarrativeInteraction : MonoBehaviour
    {
        //mostrar algun feedback 
        [SerializeField] float maxDistance;
        [SerializeField] LayerMask interactableEntityLayer;

        private PlayerInputActions _input;
        private InteractBehaviour _interactableEntity;

        private bool _hasTriggeredOnce;

        private void Awake()
        {
            _input = GameManager.Input;
            _input.Interaction.Enable();
            _input.Interaction.NPC.performed += HandleNarrativeInteraction;          
        }

        private void Start()
        {
            DialogueManager.Instance.OnDialogueEnded += ResetDialogue;
        }

        private void OnDestroy()
        {
            _input.Interaction.NPC.performed -= HandleNarrativeInteraction;
            DialogueManager.Instance.OnDialogueEnded -= ResetDialogue;
        }


        private void HandleNarrativeInteraction(InputAction.CallbackContext _)
        {
            if (!_hasTriggeredOnce)
            {
                if (TryFindInteractableNPC(out var currentInteractable))
                {
                    if (currentInteractable == _interactableEntity) return;

                    _interactableEntity = currentInteractable;

                }
                else if (_interactableEntity != null)
                {
                    _interactableEntity = null;
                }

            }
        }

        private bool TryFindInteractableNPC(out InteractBehaviour interactable)
        {
            interactable = null;
            return Physics.Raycast(transform.position, transform.forward, out RaycastHit raycastHit, maxDistance, interactableEntityLayer) && raycastHit.collider.gameObject.TryGetComponent(out interactable);
        }

        private void ResetDialogue()
        {
            _hasTriggeredOnce = false;
            _interactableEntity = null;
        }

    }
}
