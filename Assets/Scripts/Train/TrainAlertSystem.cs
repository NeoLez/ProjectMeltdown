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
            if (firstAdded) {
                firstAdded = false;
                currentAlert = alert;
                OnEventChanged?.Invoke();
                return;
            }
            alerts.Add(alert);
        }

        private void SetAlert() {
            currentAlert = alerts[0];
            OnEventChanged?.Invoke();
        }

        public void SetNextAlert() {
            if (alerts.Count == 0) {
                firstAdded = true;
                return;
            }
            SetAlert();
            alerts.RemoveAt(0);
        }
        
        
    }
}