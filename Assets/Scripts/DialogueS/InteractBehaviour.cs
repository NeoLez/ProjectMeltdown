using UnityEngine;

namespace Root
{
    public abstract class InteractBehaviour : MonoBehaviour
    {
        public DialogueSO Dialogue;
        //public bool CanMaintaingInteraction;
        public bool hasBeenTriggeredOnce;
        public abstract void ExecuteDialogue();

        public virtual void StartedExecutingDialogue() { }

        public virtual void FinishedExecutingDialogue() { }
    }
}
