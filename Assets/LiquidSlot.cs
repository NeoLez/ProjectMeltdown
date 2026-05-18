using UnityEngine;

namespace Root
{
    public class LiquidSlot : MonoBehaviour
    {
        [SerializeField] private BatterySlot batterySlot; 

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent<Liquid>(out var liquid)) return;

            var battery = batterySlot.GetBattery();
            if (battery == null) return;

           
            battery.energy = Mathf.Clamp(battery.energy + liquid.rechargeAmount * battery.maxEnergy, 0, battery.maxEnergy);
            Destroy(other.gameObject);
        }
    }
}