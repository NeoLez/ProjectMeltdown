using UnityEngine;

namespace Root
{
    public class WaterDrop : MonoBehaviour
    {
        [Header("Fall")]
        [SerializeField] private float _fallSpeed = 3f;
        [SerializeField] private float _lifeTime = 5f;

        [Header("Damage")]
        [SerializeField] private float _damage = 5f;

        [Header("Ground")]
        [SerializeField] private LayerMask _groundLayer;

        [Header("Audio")]
        [SerializeField] private AudioClip _splashClip;
        [SerializeField] private float _splashVolume = 1f;
        [SerializeField] private float _splashMaxDistance = 15f;

        private void Start()
        {
            Destroy(gameObject, _lifeTime);
        }

        private void Update()
        {
            transform.position += Vector3.down * _fallSpeed * Time.deltaTime;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<HealthControl>(out HealthControl health))
            {
                health.TakeDamage(_damage);
                PlaySplash();
                Destroy(gameObject);
                return;
            }

            if (IsInLayerMask(other.gameObject.layer, _groundLayer))
            {
                PlaySplash();
                Destroy(gameObject);
            }
        }

        private bool IsInLayerMask(int layer, LayerMask mask)
        {
            return (mask.value & (1 << layer)) != 0;
        }

        private void PlaySplash()
        {
            if (_splashClip != null)
            {
                GameManager.AudioSystem?.PlaySoundPositional(
                    _splashClip,
                    transform.position,
                    GameManager.AudioSystem.VFX,
                    _splashVolume,
                    _splashMaxDistance
                );
            }
        }
    }
}