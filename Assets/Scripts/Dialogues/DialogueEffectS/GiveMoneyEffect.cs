using UnityEngine;

namespace Root
{
    [CreateAssetMenu(menuName = "Dialogue/Advanced/Effects/Give Money")]
    public class GiveMoneyEffect : DialogueEffectSO
    {
        public override void Execute(DialogueExecutionContext context)
        {
            int requestAmount = context.NPC.gameObject.GetComponent<NpcToll>().AmountToRequest;

            if (EconomyManager.Instance.GetMoney() < requestAmount)
            {
                Debug.Log("No tienes dinero suficiente para depositar");
                return;
            }
            EconomyManager.Instance.SpendMoney(requestAmount);
        }
    }
}
