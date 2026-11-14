using UnityEngine;

public static class VolumeSettings
{
    public const string MasterKey = "MasterVolume";
    public const string MusicKey = "MusicVolume";
    public const string SfxKey = "SFXVolume";
    public const float Default = 1f;
    public const float MinSlider = 0.0001f;

    public static float Get(string key) => PlayerPrefs.GetFloat(key, Default);

    public static void Set(string key, float value)
    {
        Apply(key, value);
        PlayerPrefs.SetFloat(key, value);
        PlayerPrefs.Save();
    }

    public static void ApplyAll()
    {
        Apply(MasterKey, Get(MasterKey));
        Apply(MusicKey, Get(MusicKey));
        Apply(SfxKey, Get(SfxKey));
    }

    private static void Apply(string key, float value)
    {
        if (GameManager.AudioSystem == null || GameManager.AudioSystem.GeneralMixer == null) return;
        GameManager.AudioSystem.GeneralMixer.SetFloat(key, Mathf.Log10(Mathf.Max(value, MinSlider)) * 20f);
    }
}