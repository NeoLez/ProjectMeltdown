using UnityEngine;
using UnityEngine.Audio;

public class AudioSystemInitializer : MonoBehaviour
{
    private static AudioSystemInitializer instance;

    [SerializeField] private AudioSource nonPositionalSource;
    [SerializeField] private AudioSource MusicSource;
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioMixerGroup musicMixerGroup;
    [SerializeField] private AudioMixerGroup sfxMixerGroup;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject); 

        GameManager.AudioSystem.NonPositionAudioSource = nonPositionalSource;
        GameManager.AudioSystem.MusicAudioSource = MusicSource;
        GameManager.AudioSystem.GeneralMixer = audioMixer;
        GameManager.AudioSystem.Music = musicMixerGroup;
        GameManager.AudioSystem.VFX = sfxMixerGroup;
    }

    private void Start()
    {
        if (instance != this) return; 
        VolumeSettings.ApplyAll();
    }
}