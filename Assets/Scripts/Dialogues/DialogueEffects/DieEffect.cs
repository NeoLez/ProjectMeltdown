using UnityEngine;

namespace Root
{
    [CreateAssetMenu(menuName = "Dialogue/Advanced/Effects/Player Death")]
    public class DieEffect : DialogueEffectSO
    {
        public override bool CanExecute(DialogueExecutionContext context = null)
        {
            return true;
        }

        public override void Execute(DialogueExecutionContext context)
        {
            HealthControl health = context.Instigator.GetComponent<HealthControl>();

            health.InstantDeath();
        }

    }
}
