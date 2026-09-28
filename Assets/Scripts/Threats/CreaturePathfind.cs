using System;
using Timers;
using UnityEngine;
using UnityEngine.AI;

namespace Root
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class CreaturePathfind : MonoBehaviour {
        private NavMeshAgent _agent;
        [SerializeField] private float navmeshUpdateFrequency;
        [SerializeField] private float attackCooldown;
        [SerializeField] private float attackDamage;
        private Transform _target;
        private readonly Timer _navmeshUpdateTimer = new();
        private readonly Timer _attackTimer = new();
        
        private void Awake() {
            _agent = GetComponent<NavMeshAgent>();
            if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 2.0f, NavMesh.AllAreas))
            {
                _agent.Warp(hit.position);
            }
            else
            {
                Debug.LogError($"{gameObject.name} spawned too far away from a valid NavMesh!", this);
            }
        }

        private void Update() {
            if (_navmeshUpdateTimer.IsCompleted()) {
                if (_target == null) _target = GameManager.Player.transform;
                _navmeshUpdateTimer.Reset(navmeshUpdateFrequency);
                _agent.SetDestination(_target.position);
            }
        }

        private void OnCollisionEnter(Collision collision) {
            if (collision.gameObject == GameManager.Player.gameObject && _attackTimer.IsCompleted()) {
                collision.gameObject.GetComponent<HealthControl>().TakeDamage(attackDamage);
                _attackTimer.Reset(attackCooldown);
            }
        }
    }
}
