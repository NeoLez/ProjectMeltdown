using UnityEngine;

namespace Root
{
    public class BrakeRepairSlot : MonoBehaviour
    {
        [SerializeField] private TrainBrakeController brakeController; 

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent<BrakeFluid>(out var fluid)) return;

            brakeController.Repair(fluid.repairAmount);
            Destroy(other.gameObject);
        }

        public bool TryUseBrakeFluid(BrakeFluid fluid)
        {
            brakeController.Repair(fluid.repairAmount);

            Destroy(fluid.gameObject);

            return true;
        }
    }
}