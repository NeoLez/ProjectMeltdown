using System;
using System.Collections.Generic;
using UnityEngine;

namespace Root {
    public class MapSection : MonoBehaviour {
        public List<TrainPathWaypoint> Waypoints;
        public event Action<bool> OnTrainCompleted;
        public TrainAlertSO alert;
        public Transform end;
        public bool shouldConsumeAlert;

        private void Awake() {
            Waypoints[^1].OnTrainReached += () => {
                OnTrainCompleted?.Invoke(shouldConsumeAlert);
            };
        }

        public void Remove() {
            foreach (var w in Waypoints) {
                Destroy(w);
            }
            Destroy(gameObject);
        }
    }
}