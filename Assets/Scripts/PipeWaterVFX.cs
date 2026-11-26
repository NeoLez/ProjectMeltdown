using UnityEngine;
using UnityEngine.VFX;

namespace Root
{
    public class PipeWaterVFX : MonoBehaviour
    {
        [SerializeField] private WaterMovable _water;
        [SerializeField] private Renderer _waterSurface;
        [SerializeField] private float _surfaceOffset;

        private VisualEffect _vfx;
        private bool _playing;

        private void Awake()
        {
            _vfx = GetComponent<VisualEffect>();
            _vfx.initialEventName = "OnStop";
        }

        private void OnEnable()
        {
            _water.OnRisingChanged += HandleRisingChanged;
            ForceOff();
        }

        private void Start()
        {
            if (!_playing) ForceOff();
        }

        private void OnDisable()
        {
            _water.OnRisingChanged -= HandleRisingChanged;
            ForceOff();
        }

        private void LateUpdate()
        {
            if (_playing) UpdateFallLength();
        }

        private void ForceOff()
        {
            _playing = false;
            _vfx.Stop();
        }

        private void HandleRisingChanged(bool rising)
        {
            _playing = rising;

            if (rising)
            {
                UpdateFallLength();
                _vfx.Play();
            }
            else _vfx.Stop();
        }

        private void UpdateFallLength()
        {
            if (_waterSurface == null) return;

            float surfaceY = _waterSurface.bounds.max.y + _surfaceOffset;
            float length = (transform.position.y - surfaceY) / transform.lossyScale.y;
            _vfx.SetFloat("LongitudCaida", Mathf.Max(0.1f, length));
        }
    }
}