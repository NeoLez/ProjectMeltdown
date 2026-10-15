using UnityEngine;

namespace Root {
    [CreateAssetMenu(menuName = "Dialogue/Advanced/Effects/TriggerAnimation")]
    public class TriggerNPCAnimationEffectSo : DialogueEffectSO {
        
        [SerializeField] private string animationName;

        public override bool CanExecute(DialogueExecutionContext context = null)
        {
            return true;
        }

        public override void Execute(DialogueExecutionContext context) {
            context.NPC.gameObject.GetComponent<NpcToll>().animator.Play(animationName);
        }
    }
}