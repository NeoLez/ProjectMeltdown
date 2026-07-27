using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Root
{
    public class NarrativeInteraction : MonoBehaviour
    {
        //mostrar algun feedback 
        [SerializeField] float maxDistance;
        [SerializeField] LayerMask interactableEntityLayer;

        [SerializeField] private GameObject crosshair;
        [SerializeField] private List<Sprite> _crosshairSprite; 

        private Image _crosshairImage;
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
            _crosshairImage = crosshair.GetComponent<Image>();

            DialogueManager.Instance.OnDialogueEnded += ResetDialogue;
        }

        private void Update()
        {
            if (_hasTriggeredOnce) return;

            if (TryFindInteractableNPC(out _))
            {
                NotificationManager.Instance.ShowNotification("Pulsa E para conversar");

                _crosshairImage.sprite = _crosshairSprite[1];
            }
            else
            {
                _crosshairImage.sprite = _crosshairSprite[0];
                NotificationManager.Instance.ShowNotification("");
            }
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

            DialogueWithNPC();
        }

        private void DialogueWithNPC()
        {
            if (_interactableEntity != null)
            {
                _hasTriggeredOnce = true;
                _interactableEntity.ExecuteDialogue();
            }
        }

        private bool TryFindInteractableNPC(out InteractBehaviour interactable)
        {
            interactable = null;
            return Physics.Raycast(transform.position, transform.forward, out RaycastHit raycastHit, maxDistance, interactableEntityLayer) && raycastHit.collider.gameObject.TryGetComponent(out interactable);
        }

        private void ResetDialogue()
        {
            //_hasTriggeredOnce = false;
            _interactableEntity = null;
        }

        private void OnDestroy()
        {
            _input.Interaction.NPC.performed -= HandleNarrativeInteraction;
            DialogueManager.Instance.OnDialogueEnded -= ResetDialogue;
        }

    }
}
