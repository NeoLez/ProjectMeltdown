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
        private CreaturePathfind _creaturePathfind;
        void Start()
        {
            _creaturePathfind = GetComponentInChildren<CreaturePathfind>();
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
            _creaturePathfind.InterruptByDmg();
        }

        public override void Death()
        {
            animator.Play(deathAnimation);

            GetComponent<BloodSplatter>()?.SpawnBlood(transform.position);

            Instantiate(_particles, transform.position, Quaternion.identity, transform);

            if (_idleLoop != null) _idleLoop.Stop();
            if (_soundDeath != null) GameManager.AudioSystem.PlaySoundPositional(_soundDeath, transform.position, GameManager.AudioSystem.VFX);

            _creaturePathfind.enabled = false;

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
