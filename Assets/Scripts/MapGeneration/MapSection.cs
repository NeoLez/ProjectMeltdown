using System;
using System.Collections.Generic;
using Timers;
using UnityEngine;

namespace Root {
    public class MapSection : MonoBehaviour {
        public bool isStation;
        public MapPointsGen.Node Node { get; private set; }
        private int _id;
        [SerializeField] protected List<TrainPathWaypoint> _waypoints = new();
        [SerializeReference] CompositeBoundingBox boundingBox;
        public event Action OnMapSectionRemoved;

        public List<TrainPathWaypoint> GetWaypoints() {
            return _waypoints;
        }

        public event Action<bool> OnTrainCompleted;
        public TrainAlertSO alert;
        public Transform end;
        public bool shouldConsumeAlert;

        public void Initialize(MapPointsGen.Node node, int id) {
            Node = node;
            _id = id;
            _waypoints[^1].OnTrainReached += () => {
                OnTrainCompleted?.Invoke(shouldConsumeAlert);
            };
        }

        public int GetMapSectionSeed() {
            return SeedUtils.Combine(new[] { GameManager.seed, Node.height, Node.dist, _id });
        }

        public void Remove() {
            OnMapSectionRemoved?.Invoke();
            foreach (var w in _waypoints) {
                Destroy(w);
            }
            Destroy(gameObject);
        }
    }
}