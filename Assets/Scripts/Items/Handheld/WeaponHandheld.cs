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
        private MeleeWeaponItemSo ItemSo => (MeleeWeaponItemSo)state.ItemSo;
        
        public override void Use(InputAction.CallbackContext ctx)
        {
            base.Use(ctx);
            StartCoroutine(PerformHit());
        }

        private IEnumerator PerformHit()
        {
            if (!_cooldownTimer.IsCompleted())
            {
                var cooldownTimeLeft = _cooldownTimer.GetTimeLeft();
                if (cooldownTimeLeft <= ItemSo.InputBufferTime)
                {
                    yield return new WaitForSeconds(cooldownTimeLeft);
                }
                else
                {
                    yield break;
                }
            }
            _cooldownTimer.Reset(ItemSo.Cooldown);
            
            PlayAnimation();
            yield return new WaitForSeconds(ItemSo.HitTime);
            Damage();
        }

        private void PlayAnimation()
        {
            animator.SetFloat(AttackSpeedHash, attackClip.length / ItemSo.Cooldown);
            animator.Play(AttackStateHash, 0, 0f);
        }

        private void Damage()
        {
            var cam = GameManager.CameraPivot.transform;
            if (!Physics.Raycast(cam.position, cam.forward, out RaycastHit hit, ItemSo.Range, layerMask))
                return;

            if (hit.collider.gameObject.TryGetComponent<EnemyHitBox>(out var component))
            {
                component.Hit(ItemSo.Damage);
            }
        }
    }
}