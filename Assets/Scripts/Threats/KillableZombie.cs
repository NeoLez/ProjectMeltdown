using Root.Enemy;
using UnityEngine;

namespace Root
{
    public class KillableZombie : EnemyClass
    {
        [SerializeField] private float _health = 3;

        [SerializeField] private GameObject _visuals;
        [SerializeField] private GameObject _particles;
        [SerializeField] private Collider[] hitBoxes;

        [Header("Sounds")]
        [SerializeField] private AudioClip _soundDeath;
        [SerializeField] private AudioClip _soundIdle;

        [Header("Animation")]
        [SerializeField] private Animator animator;
        [SerializeField] private string deathAnimation = "death";

        private AudioSource _idleLoop;
        private ZombieStateMachine _stateMachine;
        void Start()
        {
            _stateMachine = GetComponentInChildren<ZombieStateMachine>();
        }

        public override void TakeDamage(float damage, float multiplier)
        {
            if (_health <= 0)
            {
                Death();
                return;
            }
            _health -= damage * multiplier;
            Mathf.Max(0, _health--);
            //TODO-Add damage delay
        }

        public override void Death()
        {
            animator.SetBool(deathAnimation, true);

            GetComponent<BloodSplatter>()?.SpawnBlood(transform.position);

            Instantiate(_particles, transform.position, Quaternion.identity, transform);

            if (_idleLoop != null) _idleLoop.Stop();
            if (_soundDeath != null) GameManager.AudioSystem.PlaySoundPositional(_soundDeath, transform.position, GameManager.AudioSystem.VFX);

            _stateMachine.StopAllCoroutines();
            if(_stateMachine.Agent.enabled) _stateMachine.Agent.enabled = false;

            _stateMachine.enabled = false;

            for (int i = 0; i < hitBoxes.Length; i++)
            {
                hitBoxes[i].enabled = false;
            }
        }

        public void OnAnimationEnded()
        {
            _visuals.SetActive(false);
            Destroy(_visuals);
        }
    }
}
