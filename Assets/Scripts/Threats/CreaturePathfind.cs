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
        [SerializeField] private float viewDistance;
        [SerializeField] private LayerMask raycastLayerMask;
        private Transform _target;
        private readonly Timer _navmeshUpdateTimer = new();
        private readonly Timer _attackTimer = new();
        Animator _animator;
        
        private void Awake() {
            _agent = GetComponent<NavMeshAgent>();
            _animator = GetComponentInChildren<Animator>();
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
            if (_navmeshUpdateTimer.IsCompleted() && IsPlayerInView()) {
                if (_target == null) _target = GameManager.Player.transform;
                _navmeshUpdateTimer.Reset(navmeshUpdateFrequency);
                _agent.SetDestination(_target.position);
            }
        }

        private void OnCollisionEnter(Collision collision) {
            if (collision.gameObject == GameManager.Player.gameObject && _attackTimer.IsCompleted()) {
                _animator.Play("Attack");
                collision.gameObject.GetComponent<HealthControl>().TakeDamage(attackDamage);
                _attackTimer.Reset(attackCooldown);
            }
        }

        private bool IsPlayerInView() {
            if (!Physics.Raycast(transform.position, GameManager.Player.transform.position - transform.position,
                    out var hit, viewDistance, raycastLayerMask))
                return false;
            return hit.collider.gameObject == GameManager.Player.gameObject;
        }
    }
}
