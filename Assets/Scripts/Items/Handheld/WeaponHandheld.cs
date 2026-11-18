using System.Collections;
using Timers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Root.Handheld
{
    public class WeaponHandheld : HandheldItem
    {
        [SerializeField] Animator animator;
        [SerializeField] AnimationClip attackClip;
        [SerializeField] private LayerMask layerMask;
        private Timer _cooldownTimer = new();
        
        static readonly int AttackSpeedHash = Animator.StringToHash("AttackSpeed");
        static readonly int AttackStateHash = Animator.StringToHash("Attack"); 
        private MeleeWeaponItemSo State => (MeleeWeaponItemSo)state.ItemSo;
        
        public override void Use(InputAction.CallbackContext ctx)
        {
            base.Use(ctx);
            StartCoroutine(PerformHit());
        }

        private IEnumerator PerformHit()
        {
            if (!_cooldownTimer.IsCompleted())
                yield break;
            _cooldownTimer.Reset(State.Cooldown);
            
            PlayAnimation();
            yield return new WaitForSeconds(State.HitTime);
            Damage();
        }

        private void PlayAnimation()
        {
            animator.SetFloat(AttackSpeedHash, attackClip.length / State.Cooldown);
            animator.Play(AttackStateHash, 0, 0f);
        }

        private void Damage()
        {
            var cam = GameManager.CameraPivot.transform;
            if (!Physics.Raycast(cam.position, cam.forward, out RaycastHit hit, State.Range, layerMask))
                return;

            if (hit.collider.gameObject.TryGetComponent<EnemyHitBox>(out var component))
            {
                component.Hit(State.Damage);
            }
        }
    }
}