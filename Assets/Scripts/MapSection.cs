using System;
using System.Collections.Generic;
using UnityEngine;

namespace Root {
    public class MapSection : MonoBehaviour {
        public List<TrainPathWaypoint> Waypoints;
        public event Action OnTrainCompleted;
        public Transform end;

        private void Awake() {
            Waypoints[^1].OnTrainReached += () => {
                OnTrainCompleted?.Invoke();
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