using NaughtyAttributes;
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
        [SerializeField] private bool triggerVisuals = true;
        [SerializeField, ShowIf("triggerVisuals")] private Animator animator;
        [SerializeField, ShowIf("triggerVisuals")] private AudioClip damageSound;

        [SerializeField, ShowIf("triggerVisuals")] private GameObject particlesHit;

        [SerializeField, ShowIf("triggerVisuals")] private string partAnimation = "damage"; 
        

        public override void Interact()
        {
            OnEnemyHit(1 * damagePerPartMultiplier);
            TriggerVisuals();
        }
        
        public void Hit(float damage)
        {
            OnEnemyHit(damage * damagePerPartMultiplier);
            if (triggerVisuals)
                TriggerVisuals();
        }
        
        public void OnEnemyHit(float damage)
        {
            enemyHealth.TakeDamage(damage);
        }

        public void TriggerVisuals()
        {
            animator.Play(partAnimation, -1, 0f);

            Instantiate(particlesHit, transform.position, transform.rotation);
            if (damageSound != null) GameManager.AudioSystem.PlaySoundPositional(damageSound, transform.position, GameManager.AudioSystem.VFX);
        }
    }
}
