using UnityEngine;

namespace Root
{
    public class DeliveryObjectiveChecker : MonoBehaviour
    {
        [SerializeField] private DeliveryPostSpawner deliveryPost;

        private void Awake()
        {
            PackagesSystemController.Instance.OnDeliveryStationReached += EnableDeliveryDock;
        }

        private void EnableDeliveryDock()
        {
            deliveryPost.enabled = true;
        }

        private void OnDestroy()
        {
            PackagesSystemController.Instance.OnDeliveryStationReached -= EnableDeliveryDock;
        }
    }
}
