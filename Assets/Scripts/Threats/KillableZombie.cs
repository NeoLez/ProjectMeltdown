using Root.Enemy;
using UnityEngine;
using static UnityEngine.ParticleSystem;

namespace Root
{
    public class KillableZombie : InteractableNormalCamera
    {
        [SerializeField] private int _health = 3;
        private bool _isDead;
        Animator _animator;
        [SerializeField] GameObject _visuals;
        [SerializeField] GameObject _particles;
        [SerializeField] GameObject _particlesHit;

        [Header("Sounds")]
        [SerializeField] private AudioClip _soundDamage;
        [SerializeField] private AudioClip _soundDeath;
        [SerializeField] private AudioClip _soundIdle;
        [SerializeField] private AudioClip _soundLatch;
        [SerializeField] private AudioClip _soundDestroy;
        
        private AudioSource _idleLoop;
        private CreaturePathfind CreaturePathfind;
        void Start()
        {
            _animator = GetComponentInChildren<Animator>();
            CreaturePathfind = GetComponentInChildren<CreaturePathfind>();
        }

        public override void Interact()
        {
            if (_isDead) return;
            if (_health <= 0)
            {               
                _isDead = true;
                
                GetComponent<BloodSplatter>()?.SpawnBlood(transform.position);
                _visuals.SetActive(false);
                Destroy(_visuals);
                Instantiate(_particles, transform.position, Quaternion.identity, transform);

                if (_idleLoop != null) _idleLoop.Stop();
                if (_soundDeath != null) GameManager.AudioSystem.PlaySoundPositional(_soundDeath, transform.position, GameManager.AudioSystem.VFX);

                Invoke(nameof(Destroy), 5f);
            }
            else
            {
                CreaturePathfind.InterruptByDmg();
                _animator.Play("Damage", -1, 0f);
                Instantiate(_particlesHit, transform.position, transform.rotation);
                if (_soundDamage != null) GameManager.AudioSystem.PlaySoundPositional(_soundDamage, transform.position, GameManager.AudioSystem.VFX);
                _health--;
            }
        }
    }
}
