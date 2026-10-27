using Root.Enemy;
using UnityEngine;

namespace Root
{
    public class KillableZombie : EnemyClass
    {
        [SerializeField] private float _health = 3;

        [SerializeField] GameObject _visuals;
        [SerializeField] GameObject _particles;

        [Header("Sounds")]
        [SerializeField] private AudioClip _soundDeath;
        [SerializeField] private AudioClip _soundIdle;

        [Header("Animation")]
        [SerializeField] private Animator animator;
        [SerializeField] private string deathAnimation = "death";

        private AudioSource _idleLoop;
        private CreaturePathfind _creaturePathfind;
        void Start()
        {
            _creaturePathfind = GetComponentInChildren<CreaturePathfind>();
        }

      /*  public void Interact()
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

                Instantiate(_particlesHit, transform.position, transform.rotation);
                if (_soundDamage != null) GameManager.AudioSystem.PlaySoundPositional(_soundDamage, transform.position, GameManager.AudioSystem.VFX);
                _health--;
            }
        }*/

       /* public void TakeDamage(float multiplier)
        {
            if (_health <= 0.0f)
            {
                Death();
                return;
            }
            _health *= multiplier;
            //_health--;
            Mathf.Max(0, _health--);
            _creaturePathfind.InterruptByDmg();
        }*/

        public override void TakeDamage(float damage, float multiplier)
        {
            if (_health <= 0)
            {
                Death();
                return;
            }
            _health -= damage * multiplier;
            Mathf.Max(0, _health--);
            _creaturePathfind.InterruptByDmg();
        }

        public override void Death()
        {
            animator.Play(deathAnimation, -1, 0f);

            GetComponent<BloodSplatter>()?.SpawnBlood(transform.position);

            Instantiate(_particles, transform.position, Quaternion.identity, transform);

            if (_idleLoop != null) _idleLoop.Stop();
            if (_soundDeath != null) GameManager.AudioSystem.PlaySoundPositional(_soundDeath, transform.position, GameManager.AudioSystem.VFX);
        }

        public void OnAnimationEnded()
        {
            _visuals.SetActive(false);
            Destroy(_visuals);
        }
    }
}
