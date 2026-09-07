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
            Dialogue.OnDialogueStarted += StartedExecutingDialogue;
            Dialogue.OnDialogueEnded += FinishedExecutingDialogue;
        }

        protected virtual void OnDestroy()
        {
            Dialogue.OnDialogueStarted -= StartedExecutingDialogue;
            Dialogue.OnDialogueEnded -= FinishedExecutingDialogue;
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
