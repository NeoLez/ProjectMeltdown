using UnityEngine;

namespace Root {
    [CreateAssetMenu(menuName = "Dialogue/Advanced/Effects/TriggerAnimation")]
    public class TriggerNPCAnimationEffectSo : DialogueEffectSO {
        [SerializeField] private string animationName;
        public override void Execute(DialogueExecutionContext context) {
            context.NPC.gameObject.GetComponent<NpcToll>().animator.Play(animationName);
        }
    }
}