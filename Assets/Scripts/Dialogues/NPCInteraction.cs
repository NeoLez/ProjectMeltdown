using NaughtyAttributes;
using UnityEngine;

namespace Root
{
    public class NPCInteraction : InteractBehaviour
    {
        public Transform instancePivot;
        [field: Required, SerializeField] public MapSection Section { get; private set; }
        [SerializeField] private MissionObjectiveSO currentMission;

        public MissionObjectiveSO Mission => currentMission;

        public override void ExecuteDialogue()
        {
            if (!gameObject.activeInHierarchy) return;

            if (hasBeenTriggeredOnce) return;

            TriggerDialogue();
        }

        void TriggerDialogue()
        {
            if (Dialogue != null)
            {
                DialogueManager.Instance.StartConversation(Dialogue, this);
            }
        }

        public override void FinishedExecutingDialogue()
        {
            if (!Dialogue.CanRepeatDialogue) hasBeenTriggeredOnce = true;

            base.FinishedExecutingDialogue();
        }

    }
}
