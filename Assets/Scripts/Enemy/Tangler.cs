using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Root.Enemy {
    public class Tangler : InteractableNormalCamera {
        [SerializeField] float _maxRadius = 10f;
        [SerializeField] float _minRadius = 2f;
        [SerializeField] GameObject visuals;
        [SerializeField] TanglerTentacle tentaclePrefab;
        [SerializeField] int maxTentacleQuantity = 3;
        bool _maxTangle = false;
        [SerializeField] LayerMask layerMask;

        private Dictionary<int, TanglerTentacle> _tentacles = new();
        private int _tentacleAmount;
        private int _tentacleID;

        private bool _isDead;
        
        [SerializeField] float tentacleSpawnCooldown = 15f;
        private float _lastTentacleSpawnTime;

        private void Awake() {
            _lastTentacleSpawnTime = Time.time;
        }

        private void Update() {
            if (Time.time >= _lastTentacleSpawnTime + tentacleSpawnCooldown) {
                AttemptTentacleSpawn();
            }
        }

        private void AttemptTentacleSpawn() {
            if (!CanSpawn()) return;

            Vector3 origin = transform.position;
            Vector3 randomDirection = Random.insideUnitSphere.normalized;

            if (Physics.Raycast(origin, randomDirection, out var hit, _maxRadius, layerMask) && hit.distance >= _minRadius) {
                Debug.DrawLine(origin, hit.point, Color.green, 1f);
                
                var tentacle = Instantiate(tentaclePrefab, transform.position, Quaternion.identity, transform);
                _tentacles[_tentacleID] = tentacle;
                _lastTentacleSpawnTime = Time.time;
                _tentacleAmount++;
                _tentacleID++;
                
                tentacle.onCut += HandleOnTentacleCut;
                tentacle.Spawn(transform.position, hit.distance, randomDirection);
            }
            else {
                Debug.DrawRay(origin, randomDirection * _maxRadius, Color.red, 1f);
            }
        }

        public bool CanSpawn() {
            return !_isDead && (_tentacleAmount < maxTentacleQuantity);
        }

        public override void Interact() {
            if (_isDead) return;
            foreach (var tentacle in _tentacles) {
                if (tentacle.Value.IsDead()) continue;
                tentacle.Value.Cut(0);
            }

            _isDead = true;
            visuals.SetActive(false);
            Invoke(nameof(Destroy), 5f);
        }

        private void HandleOnTentacleCut() {
            _lastTentacleSpawnTime = Time.time;
            _tentacleAmount--;
        }
    }
}