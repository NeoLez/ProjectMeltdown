using UnityEngine;

namespace Root
{
    [CreateAssetMenu(menuName = "Dialogue/Advanced/Effects/Give Money")]
    public class GiveMoneyEffect : DialogueEffectSO
    {
        public float discountPerPackage = 0.1f;
        public float allPackagesDeliveredDiscount = 0.5f;

        [SerializeField] private string[] labels = {$"(x{3}) ", $"(-{50}%) "};
        private float _currentMoney;

        public override void ExecuteBefore(DialogueExecutionContext context = null)
        {
            _currentMoney = context.NPC.gameObject.GetComponent<NpcToll>().AmountToRequest;

            MissionsManager missions = MissionsManager.Instance;

            //si no entregaste ninguno, que suba por la cantidad que tenes en este caso 3
            if (missions.PackagesToDeliver() == missions.UndeliveredPackages())
            {
                _currentMoney *= missions.UndeliveredPackages();
                //Debug.Log(currentMoney);

                newDescription = string.Format(labels[0], _currentMoney.ToString());
            }
            else if (missions.PackagesDelivered() < missions.PackagesToDeliver()) //sino que te descuente un porcentaje por los pquetes entregados,
            {
                int totalDelivered = missions.PackagesDelivered();
                int amountToDeliver = missions.PackagesToDeliver();
                totalDelivered = Mathf.Clamp(totalDelivered, 0, amountToDeliver);

                int remainingPackages = amountToDeliver - totalDelivered;

                float succesPercentage = ((float)totalDelivered / amountToDeliver);

                float totalToPay = remainingPackages * succesPercentage;
                _currentMoney *= totalToPay;
                //Debug.Log(currentMoney);
                newDescription = _currentMoney.ToString();
            }
            else
            {
                _currentMoney *= allPackagesDeliveredDiscount;
                //Debug.Log(currentMoney);

                newDescription = string.Format(labels[1], _currentMoney.ToString());
            }
            //newDescription = currentMoney.ToString();
            base.ExecuteBefore();
        }

        public override bool CanExecute(DialogueExecutionContext context = null)
        {
           /* //newDescription = context.Target.Dialogue.DialogueChoices;
            float money = context.NPC.gameObject.GetComponent<NpcToll>().AmountToRequest;

            MissionsManager missions = MissionsManager.Instance;

            //si no entregaste ninguno, que suba por la cantidad que tenes en este caso 3
            if (missions.PackagesToDeliver() == missions.UndeliveredPackages())
            {
                money *= missions.UndeliveredPackages();
                Debug.Log(money);
            }
            else if (missions.PackagesDelivered() < missions.PackagesToDeliver()) //sino que te descuente un porcentaje por los pquetes entregados,
            {
                if (missions.PackagesDelivered() <= 0) return false;

                int totalDelivered = missions.PackagesDelivered();
                int amountToDeliver = missions.PackagesToDeliver();
                totalDelivered = Mathf.Clamp(totalDelivered, 0, amountToDeliver);

                int remainingPackages = amountToDeliver - totalDelivered;

                float succesPercentage = ((float)totalDelivered / amountToDeliver) * 100f;
                Debug.Log($"Eficiencia de entrega: {succesPercentage:F2}% ({totalDelivered}/{amountToDeliver})");

                float totalToPay = remainingPackages * penalizationCostForEveryPackage;
                Debug.Log(money);

            }
            else // si entregaste todos, te descuenta la mitad?
            {
                money *= allPackagesDeliveredDiscount;

                Debug.Log(money);
            }
            newDescription = money.ToString();*/
            return EconomyManager.Instance.GetMoney() >= _currentMoney;
        }

        public override void Execute(DialogueExecutionContext context)
        {
            float money = context.NPC.gameObject.GetComponent<NpcToll>().AmountToRequest;
            EconomyManager.Instance.SpendMoney((int)money);


            OnEffectExcuted?.Invoke();
        }
    }
}
