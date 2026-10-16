using System;
using System.Collections.Generic;
using UnityEngine;

namespace Root {
    public class TrainAlertSystem : MonoBehaviour {
        private List<Tuple<TrainAlertSO, TrainPathWaypoint>> alerts = new();
        public Tuple<TrainAlertSO, TrainPathWaypoint> currentAlert;
        public event Action OnEventChanged;

        public void AddAlert(TrainAlertSO alert, TrainPathWaypoint waypoint) {
            alerts.Add(new (alert, waypoint));
            if(alerts.Count == 1) SetAlert();
        }

        private void SetAlert() {
            currentAlert = alerts[0];
            OnEventChanged?.Invoke();
        }
        
        public void SetNextAlert() {
            if (alerts.Count == 0) {
                return;
            }
            
            alerts.RemoveAt(0);
            if (alerts.Count >= 1) {
                SetAlert();
            }
        }
    }
}