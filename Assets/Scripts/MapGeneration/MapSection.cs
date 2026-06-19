using System;
using System.Collections.Generic;
using UnityEngine;

namespace Root {
    public class MapSection : MonoBehaviour {
        public bool isStation;
        [SerializeField] protected List<TrainPathWaypoint> _waypoints = new();

        public List<TrainPathWaypoint> GetWaypoints() {
            return _waypoints;
        }

        public event Action<bool> OnTrainCompleted;
        public TrainAlertSO alert;
        public Transform end;
        public bool shouldConsumeAlert;

        public void Initialize() {
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
    }
}