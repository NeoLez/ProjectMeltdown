using System;
using System.Collections.Generic;
using UnityEngine;

namespace Root {
    public class MapSection : MonoBehaviour {
        [SerializeField] protected List<TrainPathWaypoint> _waypoints = new();

        public List<TrainPathWaypoint> GetWaypoints() {
            SetAlert();
            return _waypoints;
        }

        public event Action<bool> OnTrainCompleted;
        public TrainAlertSO alert;
        public Transform end;
        public bool shouldConsumeAlert;

        private void SetAlert() {
            _waypoints[^1].OnTrainReached += () => {
                OnTrainCompleted?.Invoke(shouldConsumeAlert);
            };
        }

        public void Remove() {
            foreach (var w in _waypoints) {
                Destroy(w);
            }
            Destroy(gameObject);
        }

        public bool HasAlert() {
            return alert != null;
        }
    }
}