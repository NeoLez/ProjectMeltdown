using UnityEngine;

namespace Root.Enemy
{
    public class TanglerTentacleLatch : InteractableNormalCamera
    {
        public TanglerTentacle tangler;
        [SerializeField] GameObject _particles;
        public int ChainLatchNumber;
        bool _once = true;

        private AudioClip _soundLatch;

        public void SetSounds(AudioClip latch, bool playLatch)
        {
            _soundLatch = latch;
            if (playLatch && _soundLatch != null)
                GameManager.AudioSystem.PlaySoundPositional(_soundLatch, transform.position, GameManager.AudioSystem.VFX);
        }

        public override void Interact()
        {
            tangler.Cut(ChainLatchNumber);
        }

        public void DestroyInTime(float time)
        {
            Invoke(nameof(DestroyThingy), time);
        }

        public void DestroyThingy()
        {
            Instantiate(_particles, transform.position, transform.rotation);
            Destroy(gameObject);
        }

        private void OnTriggerEnter(Collider other) {
            
            if (_once && other.gameObject.layer == LayerMask.NameToLayer("Player") && other.TryGetComponent(out HealthControl health))
            {
                health.TakeDamage(35f);
                Interact();
                _once = false;
            }
        }
    }
}