using PrimeTween;
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Root
{
    public abstract class StoreBell : InteractableNormalCamera
    {
        public bool IsStoreOpened { get; set; }
        public bool CanCloseStore {  get; set; }    

        [SerializeField] protected StoreManager storeManager;
        [SerializeField] private MerchantInteraction dialogue;

        [Header("Animation")]
        [SerializeField] private AudioClip bellRing;
        [SerializeField] private Vector3 desiredHeight;
        [SerializeField] private Transform pivot;
        [SerializeField] private float duration = 1f;

        protected Action OnBellRanged;
        private bool _isAnimating;
        public override void Interact()
        {
            if (_isAnimating) return;

            if (!IsStoreOpened && !dialogue.IsDialoguing)
            {
                OnBellRanged?.Invoke();
            }
            GameManager.AudioSystem.PlaySoundPositional(bellRing, transform.position, GameManager.AudioSystem.VFX);

            PressBell();
        }

        private void PressBell()
        {
            _isAnimating = true;

            Tween.LocalPosition(
            target: pivot,
            startValue: pivot.localPosition,
            endValue: pivot.localPosition + desiredHeight,
            duration: duration,
            ease: Ease.OutQuad,
            cycles: 2,
            cycleMode: CycleMode.Yoyo).OnComplete(() => { _isAnimating = false; });
        }

    }
}
