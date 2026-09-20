using System.Collections.Generic;
using UnityEngine;

namespace Root
{
    public class StationLightingController : MonoBehaviour
    {
        [System.Serializable]
        public class LightZone
        {
            public string zoneName = "Zona";

            [Header("Luces/Renders")]
            public List<GameObject> roots = new List<GameObject>();
            public int emissiveMaterialIndex = 0;

            [System.NonSerialized] public List<Light> lights = new List<Light>();
            [System.NonSerialized] public List<Renderer> emissiveRenderers = new List<Renderer>();

            [Header("Curva de intensidad")]
            public AnimationCurve activationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
            public float smoothSpeed = 2f;

            [Header("Inestabilidad (parpadeo bajo el umbral)")]
            public bool canFlicker = true;
            [Range(0f, 1f)] public float flickerMinMultiplier = 0.3f;
            [Range(0f, 1f)] public float flickerMaxMultiplier = 1f;
            public float flickerMinInterval = 0.05f;
            public float flickerMaxInterval = 0.2f;

            [HideInInspector] public float currentIntensity01;
            [HideInInspector] public float flickerMultiplier = 1f;
            [HideInInspector] public float nextFlickerTime;
            [HideInInspector] public float[] baseLightIntensities;
            [HideInInspector] public Color[] baseEmissionColors;
            [HideInInspector] public MaterialPropertyBlock mpb;
        }
        public float InstabilityThreshold => instabilityThreshold;
        [SerializeField] private GeneratorPowerFeed powerFeed;
        [SerializeField] private List<LightZone> zones = new List<LightZone>();

        [Header("Porcentaje")]
        [SerializeField, Range(0f, 1f)] private float instabilityThreshold = 0.25f;

        private static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");

        private float _currentPercent;

        private void Awake()
        {
            foreach (LightZone zone in zones)
                CacheZone(zone);
        }

        private void CollectFromRoots(LightZone zone)
        {
            foreach (GameObject root in zone.roots)
            {
                if (root == null) continue;

                foreach (Light l in root.GetComponentsInChildren<Light>(true))
                    if (!zone.lights.Contains(l)) zone.lights.Add(l);

                foreach (Renderer r in root.GetComponentsInChildren<Renderer>(true))
                {
                    if (zone.emissiveRenderers.Contains(r)) continue;

                    Material[] mats = r.sharedMaterials;
                    if (zone.emissiveMaterialIndex < 0 || zone.emissiveMaterialIndex >= mats.Length) continue;
                    Material m = mats[zone.emissiveMaterialIndex];
                    if (m == null || !m.HasProperty(EmissionColorId)) continue;

                    zone.emissiveRenderers.Add(r);
                }
            }
        }

        private void CacheZone(LightZone zone)
        {
            CollectFromRoots(zone);

            zone.mpb = new MaterialPropertyBlock();

            zone.baseLightIntensities = new float[zone.lights.Count];
            for (int i = 0; i < zone.lights.Count; i++)
                if (zone.lights[i] != null)
                    zone.baseLightIntensities[i] = zone.lights[i].intensity;

            zone.baseEmissionColors = new Color[zone.emissiveRenderers.Count];
            for (int i = 0; i < zone.emissiveRenderers.Count; i++)
            {
                Renderer rend = zone.emissiveRenderers[i];
                if (rend == null) continue;

                Material[] mats = rend.sharedMaterials;
                if (zone.emissiveMaterialIndex < 0 || zone.emissiveMaterialIndex >= mats.Length) continue;

                zone.baseEmissionColors[i] = mats[zone.emissiveMaterialIndex].GetColor(EmissionColorId);
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

            foreach (LightZone zone in zones)
            {
                float target = zone.activationCurve.Evaluate(_currentPercent);
                zone.currentIntensity01 = Mathf.Lerp(zone.currentIntensity01, target, zone.smoothSpeed * Time.deltaTime);

                UpdateFlicker(zone, unstable);

                float finalIntensity = zone.currentIntensity01 * zone.flickerMultiplier;
                ApplyZone(zone, finalIntensity);
            }
        }

        private void UpdateFlicker(LightZone zone, bool unstable)
        {
            if (!zone.canFlicker || !unstable || zone.currentIntensity01 <= 0.01f)
            {
                zone.flickerMultiplier = 1f;
                return;
            }

            if (Time.time >= zone.nextFlickerTime)
            {
                zone.flickerMultiplier = Random.Range(zone.flickerMinMultiplier, zone.flickerMaxMultiplier);
                zone.nextFlickerTime = Time.time + Random.Range(zone.flickerMinInterval, zone.flickerMaxInterval);
            }
        }

        private void ApplyZone(LightZone zone, float t)
        {
            for (int i = 0; i < zone.lights.Count; i++)
            {
                if (zone.lights[i] == null) continue;
                zone.lights[i].intensity = zone.baseLightIntensities[i] * t;
            }

            for (int i = 0; i < zone.emissiveRenderers.Count; i++)
            {
                Renderer rend = zone.emissiveRenderers[i];
                if (rend == null) continue;

                rend.GetPropertyBlock(zone.mpb, zone.emissiveMaterialIndex);
                zone.mpb.SetColor(EmissionColorId, zone.baseEmissionColors[i] * t);
                rend.SetPropertyBlock(zone.mpb, zone.emissiveMaterialIndex);
            }
        }
    }
}