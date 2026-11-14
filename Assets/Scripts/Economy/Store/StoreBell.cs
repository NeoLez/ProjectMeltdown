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

        [Header("Animation")]
        [SerializeField] private AudioClip bellRing;
        [SerializeField] private Vector3 desiredHeight;
        [SerializeField] private Transform pivot;
        [SerializeField] private float duration = 1f;

        protected Action OnBellRanged;

        public override void Interact()
        {
            if (!IsStoreOpened)
            {
                OnBellRanged?.Invoke();
            }
            GameManager.AudioSystem.PlaySoundPositional(bellRing, transform.position, GameManager.AudioSystem.VFX);

            PressBell();
        }

        private void PressBell()
        {
            Tween.LocalPosition(
            target: pivot,
            startValue: pivot.localPosition,
            endValue: pivot.localPosition + desiredHeight,
            duration: duration,
            ease: Ease.OutQuad,
            cycles: 2,
            cycleMode: CycleMode.Yoyo);
        }

    }
}
