using System;
using UnityEngine;

namespace Root
{
    public static class MouseSensitivitySettings
    {
        private const string Key = "MouseSensitivity";
        public const float Min = 0.01f;
        public const float Max = 0.1f;
        public const float Default = 0.05f;
        public static float ToSlider(float sensitivity) => Mathf.InverseLerp(Min, Max, sensitivity); 
        public static float FromSlider(float t) => Mathf.Lerp(Min, Max, t);
        public static event Action<float> OnChanged;

        public static float Value
        {
            get => PlayerPrefs.GetFloat(Key, Default);
            set
            {
                float clamped = Mathf.Clamp(value, Min, Max);
                PlayerPrefs.SetFloat(Key, clamped);
                OnChanged?.Invoke(clamped);
            }
        }
    }
}