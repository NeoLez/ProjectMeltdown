using UnityEngine;

namespace Root
{
    [RequireComponent(typeof (Collider))]
    public class EnemyHitBox : InteractableNormalCamera
    {
        [Min(0.1f)]
        [SerializeField] private float damagePerPartMultiplier;
        [SerializeField] private EnemyClass enemyHealth;

        [Header ("Visuals")]
        [SerializeField] private Animator animator;
        [SerializeField] private AudioClip damageSound;

        [SerializeField] private GameObject particlesHit;

        private int partAnimation = Animator.StringToHash("sss"); //Insertar nombre de la animacion aca. Agregar mas si ahcen falta

        public override void Interact()
        {
            OnEnemyHit(1, damagePerPartMultiplier);
            TriggerVisuals();
        }

        public void OnEnemyHit(float damage, float multiplier)
        {
            enemyHealth.TakeDamage(damage,multiplier);
        }

        public void TriggerVisuals()
        {
            animator.Play(partAnimation, -1, 0f);

            Instantiate(particlesHit, transform.position, transform.rotation);
            if (damageSound != null) GameManager.AudioSystem.PlaySoundPositional(damageSound, transform.position, GameManager.AudioSystem.VFX);
        }

    }
}
