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

        private ColorAdjustments _colorAdjustments;
        private float DefaultBrightness = 0.1f;
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
            if (!_volume.profile.TryGet(out _colorAdjustments))
            {
                Debug.LogError("The Volume Profile does not contain a Color Adjustments override");
                return;
            }

            float savedBrightness = PlayerPrefs.GetFloat(BrightnessKey, DefaultBrightness);
            SetBrightness(savedBrightness);
        }

        public void SetBrightness(float value)
        {
            _colorAdjustments.postExposure.value = value;
        }


    }
}
