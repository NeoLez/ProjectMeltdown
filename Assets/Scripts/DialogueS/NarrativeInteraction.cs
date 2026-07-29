using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace Root
{
    public class NarrativeInteraction : MonoBehaviour
    {
        [SerializeField] private Transform cameraPivot;
        [SerializeField] float maxDistance;
        [SerializeField] LayerMask interactableEntityLayer;

        private PlayerInputActions _input;
        private Transform _npcLookingPivot;

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
                if (currentInteractable.CheckPivot())   //preguntar si el dialogo termino o se puede repertir, que vuelva a generar la interaccion
                {
                    if (currentInteractable.HasDialogueEnded()) return;

                    _npcLookingPivot = currentInteractable.Pivot;

                    GameManager.Player.GetComponent<CameraController>().FocusCamera(_npcLookingPivot);
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
