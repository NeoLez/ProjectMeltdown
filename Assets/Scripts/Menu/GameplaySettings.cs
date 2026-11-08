using Root;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class GameplaySettings : MonoBehaviour
{
    [Header("Brightness")]
    [SerializeField] private Volume _volume;
    [SerializeField] private Slider _brightnessSlider;

    [SerializeField] private float DefaultBrightness = 0.1f;
    [SerializeField] private float MinBrightness = -10;
    [SerializeField] private float MaxBrightness = 3;

    private ColorAdjustments _colorAdjustments;
    private void Start()
    {
        if(_volume == null)
        {
            _volume = LoadURPBrightness.Instance.GetComponent<Volume>();
        }

        if(!_volume.profile.TryGet(out _colorAdjustments))
        {            
            Debug.LogError("The Volume Profile does not contain a Color Adjustments override");
            return;
        }

        float savedBrightness = PlayerPrefs.GetFloat(LoadURPBrightness.Instance.Key, DefaultBrightness);


        if (_brightnessSlider != null)
        {
            _brightnessSlider.SetValueWithoutNotify(savedBrightness);
            _brightnessSlider.onValueChanged.AddListener(SetBrightness);
        }
        SetBrightness(savedBrightness);
    }

    public void SetBrightness(float value)
    {
        _colorAdjustments.postExposure.value = Mathf.Lerp(MinBrightness, MaxBrightness, value);

        PlayerPrefs.SetFloat(LoadURPBrightness.Instance.Key, value);
        PlayerPrefs.Save();
    }

    private void OnDestroy()
    {
        if (_brightnessSlider != null)
            _brightnessSlider.onValueChanged.RemoveListener(SetBrightness); 
    }
}
