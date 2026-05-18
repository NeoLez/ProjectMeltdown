using UnityEngine;

namespace Root
{
    public class BrakeRepairSlot : MonoBehaviour
    {
        [SerializeField] private TrainBrakeController brakeController; 

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent<Liquid>(out var liquid)) return;

            brakeController.Repair(liquid.rechargeAmount);
            Destroy(other.gameObject);
        }
    }
}