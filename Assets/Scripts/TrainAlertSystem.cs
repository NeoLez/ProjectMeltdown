using UnityEngine;

namespace Root {
    public class TrainAlertSystem : MonoBehaviour {
        public TrainAlertSO alert;
        public void SetAlert(TrainAlertSO alert) {
            this.alert = alert;
            Debug.Log(alert.name);
            Debug.Log(alert.maxSpeed);
        }
    }
}