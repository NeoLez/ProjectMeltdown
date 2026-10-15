using UnityEngine;

namespace Root
{
    [CreateAssetMenu(menuName = "Dialogue/Advanced/Effects/Give Money")]
    public class GiveMoneyEffect : DialogueEffectSO
    {
        public override bool CanExecute(DialogueExecutionContext context = null)
        {
            int money = context.NPC.gameObject.GetComponent<NpcToll>().AmountToRequest;
            return EconomyManager.Instance.GetMoney() >= money;
        }

        public override void Execute(DialogueExecutionContext context)
        {
            int money = context.NPC.gameObject.GetComponent<NpcToll>().AmountToRequest;
            EconomyManager.Instance.SpendMoney(money);
        }
    }
}
