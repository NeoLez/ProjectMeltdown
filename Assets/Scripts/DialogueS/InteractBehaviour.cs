using UnityEngine;

namespace Root
{
    public abstract class InteractBehaviour : MonoBehaviour
    {
        public DialogueSO dialogue;

        public abstract void ExecuteDialogue();

        public virtual void StartedExecutingDialogue() { }

        public virtual void FinishedExecutingDialogue() { }
    }
}
