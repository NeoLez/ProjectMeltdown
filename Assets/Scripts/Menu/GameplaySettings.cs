using Root;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class GameplaySettings : MonoBehaviour
{
    [Header("Brightness")]
    [SerializeField] private Slider _brightnessSlider;

    private void Start()
    {
        if (_brightnessSlider == null || LoadURPBrightness.Instance == null) return; 

        _brightnessSlider.SetValueWithoutNotify(LoadURPBrightness.Instance.SavedBrightness);
        _brightnessSlider.onValueChanged.AddListener(SetBrightness);
    }

    public void SetBrightness(float value)
    {
        if (LoadURPBrightness.Instance == null) return; 

        LoadURPBrightness.Instance.SetBrightness(value); 
    }

    private void OnDestroy()
    {
        if (_brightnessSlider != null)
            _brightnessSlider.onValueChanged.RemoveListener(SetBrightness);
    }
}