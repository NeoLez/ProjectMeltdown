using PrimeTween;
using UnityEngine;

namespace Root
{
    [RequireComponent(typeof(Collider))]
    public class PipeValve : InteractableNormalCamera
    {
        public bool Open { get; private set; }
        [SerializeField] private float rotationAngle;
        [SerializeField] private float rotationTime;
        [SerializeField] private Transform visuals;
        [SerializeField] private Pipe pipe;

        private bool _isAnimating;
        public override void Interact()
        {
            if (_isAnimating) return;
            _isAnimating = true;
            Open = !Open;

            var targetRotation = Quaternion.Euler(new Vector3(0, 0, Open ? -rotationAngle : rotationAngle));
            Tween.LocalRotation(transform, visuals.localRotation, targetRotation, rotationTime).OnComplete(() => _isAnimating = false);
        }
    }
}