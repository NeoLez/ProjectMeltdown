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

        private PlayerInputActions _input;
        private InteractBehaviour _interactableEntity;

        private bool _hasTriggeredOnce;

        private void Awake()
        {
            _input = GameManager.Input;
            _input.Interaction.Enable();
            _input.Interaction.NPC.performed += TriggerNarrative;          
        }

        private void Start()
        {
            DialogueManager.Instance.OnDialogueEnded += ResetDialogue;
        }

        private void OnDestroy()
        {
            _input.Interaction.NPC.performed -= TriggerNarrative;
            DialogueManager.Instance.OnDialogueEnded -= ResetDialogue;
        }

        private void FixedUpdate()
        {
            if (_hasTriggeredOnce) return;
            if(Physics.Raycast(transform.position, transform.forward, out RaycastHit raycastHit, maxDistance))
            {
                var currentInteractable = raycastHit.collider.GetComponent<InteractBehaviour>();

                if (currentInteractable == _interactableEntity) return;

                _interactableEntity = currentInteractable;
            }
            else if (_interactableEntity != null)
            {
                _interactableEntity = null;
            }

        }

        void TriggerNarrative(InputAction.CallbackContext _)
        {
            if(_interactableEntity!= null) 
            {
                _hasTriggeredOnce = true;
                _interactableEntity.ExecuteDialogue();
            }
        }

        void ResetDialogue()
        {
            _hasTriggeredOnce = false;
            _interactableEntity = null;
        }

    }
}
