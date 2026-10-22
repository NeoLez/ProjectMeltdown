using System;
using UnityEngine;

namespace Root
{
    public abstract class DialogueEffectSO : ScriptableObject
    {
        public abstract void Execute(DialogueExecutionContext context);

        public abstract bool CanExecute(DialogueExecutionContext context = null);

        public virtual void ExecuteBefore(DialogueExecutionContext context = null) { }

        public Action OnEffectExcuted;

        public string newDescription;
    }

    public class DialogueExecutionContext
    {
        public GameObject Instigator { get; } // El Jugador
        public InteractBehaviour Target { get; }
        public NPCInteraction NPC { get; }

        public DialogueExecutionContext(GameObject instigator, InteractBehaviour target)
        {
            Instigator = instigator;
            Target = target;
            NPC = target != null ? target.GetComponent<NPCInteraction>() : null;
        }
    }
}
