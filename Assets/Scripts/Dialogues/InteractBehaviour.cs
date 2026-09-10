using System;
using UnityEngine;

namespace Root
{
    public abstract class InteractBehaviour : MonoBehaviour
    {
        public Transform Pivot;
        public Transform PlayerPivot;
        public DialogueSO Dialogue;
        public bool hasBeenTriggeredOnce;

        public Action OnInteractionEnded;
        public Action OnInteractionStarted;

        protected virtual void Awake()
        {
            if (HasDialogue())
            {
                Dialogue.OnDialogueStarted += StartedExecutingDialogue;
                Dialogue.OnDialogueEnded += FinishedExecutingDialogue;
            }
        }

        protected virtual void OnDestroy()
        {
            if (HasDialogue())
            {
                Dialogue.OnDialogueStarted -= StartedExecutingDialogue;
                Dialogue.OnDialogueEnded -= FinishedExecutingDialogue;
            }

        }

        public bool HasDialogue()
        {
            return Dialogue;
        }

        public Transform CheckPivot()
        {
            if (Pivot == null)
            {
                Debug.LogWarning("Asignar pivot");
                return null;
            }

            return Pivot;
        }

        public Transform CheckPosPivot()
        {
            if (PlayerPivot == null)
            {
                Debug.LogWarning("Asignar pivot");
                return null;
            }

            return PlayerPivot;
        }

        public bool HasDialoguePermenantlyEnded()
        {
            if (Dialogue == null) return false;

            return !Dialogue.CanRepeatDialogue && hasBeenTriggeredOnce;
        }

        public bool HasDialogueEnded()
        {
            return Dialogue.CanRepeatDialogue;
        }

        public abstract void ExecuteDialogue();

        public virtual void StartedExecutingDialogue() {

            OnInteractionStarted?.Invoke();
        }

        public virtual void FinishedExecutingDialogue() {

            OnInteractionEnded?.Invoke();
        }
    }
}
