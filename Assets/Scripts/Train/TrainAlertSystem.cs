using System;
using System.Collections.Generic;
using UnityEngine;

namespace Root {
    public class TrainAlertSystem : MonoBehaviour {
        [SerializeField] private List<TrainAlertSO> alerts;
        public TrainAlertSO currentAlert;
        public event Action OnEventChanged;

        private bool firstAdded = true;
        public void AddAlert(TrainAlertSO alert) {
            Debug.Log("Adding alert " + alert.name);
            alerts.Add(alert);
            if(alerts.Count == 1) SetAlert();
        }

        private void SetAlert() {
            currentAlert = alerts[0];
            OnEventChanged?.Invoke();
        }
        
        public void SetNextAlert() {
            if (alerts.Count == 0) {
                Debug.Log("No alerts!");
                return;
            }

            if (alerts.Count == 1) {
                return;
            }
            Debug.Log("Removing Alert" + alerts[0].name);
            alerts.RemoveAt(0);
            SetAlert();
        }
        
        
    }
}