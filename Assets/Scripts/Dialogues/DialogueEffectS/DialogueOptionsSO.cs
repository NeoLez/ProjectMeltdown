using UnityEngine;

namespace Root
{
    [CreateAssetMenu(fileName = "GiveMissionEffect", menuName = "Dialogue/Advanced/Effects/Give Mission")]
    public class DialogueOptionsSO : DialogueEffectSO
    {
        public override void Execute(DialogueExecutionContext context)
        {
            PackagesSystemController.Instance.EnablePackageGeneration(context.NPC, context.NPC.instancePivot, context.NPC.Mission.AmountOfPackages);
        }

    }

}
