using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Root
{
    public class LoadURPBrightness : MonoBehaviour
    {
        [SerializeField] Volume _volume;

        public static LoadURPBrightness Instance { get; private set; }
        public const string BrightnessKey = "Brightness";
        public string Key => BrightnessKey;
        public float SavedBrightness => PlayerPrefs.GetFloat(BrightnessKey, DefaultBrightness); 
        private ColorAdjustments _colorAdjustments;
        private float DefaultBrightness = 0.5f;
        private const float MinBrightness = -1f; 
        private const float MaxBrightness = 1f; 
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            if (Instance != this) return; 

            if (!_volume.profile.TryGet(out _colorAdjustments))
            {
                Debug.LogError("The Volume Profile does not contain a Color Adjustments override");
                return;
            }

            SetBrightness(SavedBrightness); 
        }

        public void SetBrightness(float value)
        {
            if (_colorAdjustments == null) return; 

            _colorAdjustments.active = true; 
            _colorAdjustments.postExposure.overrideState = true; 
            _colorAdjustments.postExposure.value = Mathf.Lerp(MinBrightness, MaxBrightness, value); 
            PlayerPrefs.SetFloat(BrightnessKey, value); 
            PlayerPrefs.Save(); 
        }


    }
}