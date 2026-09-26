using System.Collections.Generic;
using UnityEngine;

namespace Root
{
    public class TrainLightingController : MonoBehaviour
    {
        [SerializeField] private TrainPowerFeed powerFeed;

        [SerializeField] private Transform lightsRoot;
        [SerializeField] private int emissiveMaterialIndex = 0;

        [SerializeField] private AnimationCurve activationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        [SerializeField] private float smoothSpeed = 2f;

        [SerializeField, Range(0f, 1f)] private float instabilityThreshold = 0.25f;
        [SerializeField] private bool canFlicker = true;
        [SerializeField, Range(0f, 1f)] private float flickerMinMultiplier = 0.3f;
        [SerializeField, Range(0f, 1f)] private float flickerMaxMultiplier = 1f;
        [SerializeField] private float flickerMinInterval = 0.05f;
        [SerializeField] private float flickerMaxInterval = 0.2f;

        private static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");

        private List<Light> _lights = new List<Light>();
        private List<Renderer> _emissiveRenderers = new List<Renderer>();
        private float[] _baseLightIntensities;
        private Color[] _baseEmissionColors;
        private MaterialPropertyBlock _mpb;

        private float _currentPercent;
        private float _currentIntensity01;
        private float _flickerMultiplier = 1f;
        private float _nextFlickerTime;

        private void Awake()
        {
            CollectFromRoot();

            _mpb = new MaterialPropertyBlock();

            _baseLightIntensities = new float[_lights.Count];
            for (int i = 0; i < _lights.Count; i++)
                if (_lights[i] != null)
                    _baseLightIntensities[i] = _lights[i].intensity;

            _baseEmissionColors = new Color[_emissiveRenderers.Count];
            for (int i = 0; i < _emissiveRenderers.Count; i++)
            {
                Renderer rend = _emissiveRenderers[i];
                if (rend == null) continue;

                Material[] mats = rend.sharedMaterials;
                if (emissiveMaterialIndex < 0 || emissiveMaterialIndex >= mats.Length) continue;

                _baseEmissionColors[i] = mats[emissiveMaterialIndex].GetColor(EmissionColorId);
            }
        }

        private void CollectFromRoot()
        {
            if (lightsRoot == null) return;

            foreach (Light l in lightsRoot.GetComponentsInChildren<Light>(true))
                _lights.Add(l);

            foreach (Renderer r in lightsRoot.GetComponentsInChildren<Renderer>(true))
            {
                Material[] mats = r.sharedMaterials;
                if (emissiveMaterialIndex < 0 || emissiveMaterialIndex >= mats.Length) continue;
                Material m = mats[emissiveMaterialIndex];
                if (m == null || !m.HasProperty(EmissionColorId)) continue;

                _emissiveRenderers.Add(r);
            }
        }

        private void Start()
        {
            powerFeed.OnPowerPercentChanged += HandlePercentChanged;
            HandlePercentChanged(powerFeed.CurrentPercent01);
        }

        private void OnDestroy()
        {
            if (powerFeed != null)
                powerFeed.OnPowerPercentChanged -= HandlePercentChanged;
        }

        private void HandlePercentChanged(float percent)
        {
            _currentPercent = percent;
        }

        private void Update()
        {
            bool unstable = _currentPercent > 0f && _currentPercent <= instabilityThreshold;

            float target = activationCurve.Evaluate(_currentPercent);
            _currentIntensity01 = Mathf.Lerp(_currentIntensity01, target, smoothSpeed * Time.deltaTime);

            UpdateFlicker(unstable);

            float finalIntensity = _currentIntensity01 * _flickerMultiplier;
            ApplyLights(finalIntensity);
        }

        private void UpdateFlicker(bool unstable)
        {
            if (!canFlicker || !unstable || _currentIntensity01 <= 0.01f)
            {
                _flickerMultiplier = 1f;
                return;
            }

            if (Time.time >= _nextFlickerTime)
            {
                _flickerMultiplier = Random.Range(flickerMinMultiplier, flickerMaxMultiplier);
                _nextFlickerTime = Time.time + Random.Range(flickerMinInterval, flickerMaxInterval);
            }
        }

        private void ApplyLights(float t)
        {
            for (int i = 0; i < _lights.Count; i++)
            {
                if (_lights[i] == null) continue;
                _lights[i].intensity = _baseLightIntensities[i] * t;
            }

            for (int i = 0; i < _emissiveRenderers.Count; i++)
            {
                Renderer rend = _emissiveRenderers[i];
                if (rend == null) continue;

                rend.GetPropertyBlock(_mpb, emissiveMaterialIndex);
                _mpb.SetColor(EmissionColorId, _baseEmissionColors[i] * t);
                rend.SetPropertyBlock(_mpb, emissiveMaterialIndex);
            }
        }
    }
}