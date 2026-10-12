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
        [SerializeField] private float DelayBeforeDmg;
        [SerializeField] private LayerMask raycastLayerMask;
        private Transform _target;
        private readonly Timer _navmeshUpdateTimer = new();
        private readonly Timer _attackTimer = new();
        Animator _animator;
        private bool _doDamage = false;
        private HealthControl _pj;
        [SerializeField] private float _delay;
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

        private void Update()
        {
            if (_navmeshUpdateTimer.IsCompleted() && IsPlayerInView())
            {
                if (_target == null) _target = GameManager.Player.transform;
                _navmeshUpdateTimer.Reset(navmeshUpdateFrequency);
                _agent.SetDestination(_target.position);
                _animator.SetBool("walking", true);
            }
            else if (!IsPlayerInView())
            {
                _animator.SetBool("walking", false);
                return;
            }
            
            if (_pj != null && _attackTimer.IsCompleted()) 
            {
                
                _animator.Play("Attack");
                _attackTimer.Reset(attackCooldown);
            }
            _delay -= 1f * Time.deltaTime;
            if (_doDamage && _delay <= 0) { _pj.TakeDamage(attackDamage); }
            if (_delay <= 0) { _delay = DelayBeforeDmg;  }
        } 

        private void OnTriggerEnter(Collider collision)
        {
            if (collision.gameObject == GameManager.Player.gameObject)
            {
                _delay = DelayBeforeDmg;
                _pj = collision.gameObject.GetComponent<HealthControl>();
                _doDamage = true;
            }
        }
        private void OnTriggerExit(Collider collision)
        {
            if (collision.gameObject == GameManager.Player.gameObject)
            {
                _doDamage = false;
            _pj = null;}
        }
        private bool IsPlayerInView() {
            if (!Physics.Raycast(transform.position, GameManager.Player.transform.position - transform.position,
                    out var hit, viewDistance, raycastLayerMask))
                return false;
            return hit.collider.gameObject == GameManager.Player.gameObject;
        }
    }
}
