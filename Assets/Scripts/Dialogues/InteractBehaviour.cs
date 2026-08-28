using UnityEngine;

namespace Root
{
    public abstract class InteractBehaviour : MonoBehaviour
    {
        public Transform Pivot;
        public Transform PlayerPivot;
        public DialogueSO Dialogue;
        public bool hasBeenTriggeredOnce;

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

        public bool HasDialogueEnded()
        {
            return !Dialogue.CanRepeatDialogue && hasBeenTriggeredOnce;
        }
        public abstract void ExecuteDialogue();

        public virtual void StartedExecutingDialogue() { }

        public virtual void FinishedExecutingDialogue() { }
    }
}
