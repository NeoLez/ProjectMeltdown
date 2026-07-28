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
        [SerializeField] private Transform cam;
        [SerializeField] float maxDistance;
        [SerializeField] LayerMask interactableEntityLayer;

        private PlayerInputActions _input;

        private void Awake()
        {
            _input = GameManager.Input;
            _input.Interaction.Enable();
            _input.Interaction.NPC.performed += HandleNarrativeInteraction;          
        }


        private void HandleNarrativeInteraction(InputAction.CallbackContext _)
        {
            if (TryFindInteractableNPC(out var currentInteractable)) 
            {
                currentInteractable.ExecuteDialogue();
            }
        }

        private bool TryFindInteractableNPC(out InteractBehaviour interactable) //evitar poder interactuar con otros cuando estoy ya con uno
        {
            interactable = null;
            return Physics.Raycast(cam.position, cam.forward, out RaycastHit raycastHit, maxDistance, interactableEntityLayer) && raycastHit.collider.gameObject.TryGetComponent(out interactable);
        }


        private void OnDestroy()
        {
            _input.Interaction.NPC.performed -= HandleNarrativeInteraction;
        }

    }
}
