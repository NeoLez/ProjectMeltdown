using System;
using System.Collections.Generic;
using Timers;
using UnityEngine;

namespace Root {
    public class MapSection : MonoBehaviour {
        public bool isStation;
        private MapPointsGen.Node _node;
        private int _id;
        [SerializeField] protected List<TrainPathWaypoint> _waypoints = new();

        public List<TrainPathWaypoint> GetWaypoints() {
            return _waypoints;
        }

        public event Action<bool> OnTrainCompleted;
        public TrainAlertSO alert;
        public Transform end;
        public bool shouldConsumeAlert;

        public void Initialize(MapPointsGen.Node node, int id) {
            _node = node;
            _id = id;
            _waypoints[^1].OnTrainReached += () => {
                OnTrainCompleted?.Invoke(shouldConsumeAlert);
            };
        }

        public int GetMapSectionSeed() {
            return SeedUtils.Combine(new[] { GameManager.seed, _node.height, _node.dist, _id });
        }

        public void Remove() {
            foreach (var w in _waypoints) {
                Destroy(w);
            }
            Destroy(gameObject);
        }
    }
}