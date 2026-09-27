using Timers;
using UnityEngine;
using UnityEngine.AI;

namespace Root
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class CreaturePathfind : MonoBehaviour {
        private NavMeshAgent _agent;
        [SerializeField] private float navmeshUpdateFrequency;
        private Transform _target;
        private Timer _navmeshUpdateTimer;
        
        private void Awake() {
            _agent = GetComponent<NavMeshAgent>();
            _navmeshUpdateTimer = new Timer();
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
    }
}
