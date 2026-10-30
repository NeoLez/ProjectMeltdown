using UnityEngine;

namespace Root
{
    [CreateAssetMenu(menuName = "Dialogue/Advanced/Effects/Give Money")]
    public class GiveMoneyEffect : DialogueEffectSO
    {
        public float discountPerPackage = 0.1f;
        public float allPackagesDeliveredDiscount = 0.5f;

        private float _currentMoney;

        public float CurrentMoney => _currentMoney;

        public override bool CanExecute(DialogueExecutionContext context = null)
        {
            MissionsManager missions = MissionsManager.Instance;

            _currentMoney = context.NPC.gameObject.GetComponent<NpcToll>().AmountToRequest;

            if(missions.AreMissionsActive())
            {
                //si no entregaste ninguno, que suba por la cantidad que tenes en este caso 3
                if (missions.PackagesToDeliver() == missions.UndeliveredPackages())
                {
                    _currentMoney *= missions.UndeliveredPackages();
                    //Debug.Log(currentMoney);
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
                }
                else
                {
                    _currentMoney *= allPackagesDeliveredDiscount;
                    //Debug.Log(currentMoney);
                }
            }

            OnEffectExcuted?.Invoke();

            return EconomyManager.Instance.GetMoney() >= _currentMoney;
        }

        public override void Execute(DialogueExecutionContext context)
        {
            //float money = context.NPC.gameObject.GetComponent<NpcToll>().AmountToRequest;
            EconomyManager.Instance.SpendMoney((int)_currentMoney);
        }
    }
}
