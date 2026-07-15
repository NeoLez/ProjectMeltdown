using UnityEngine;
using UnityEngine.Audio;

public class AudioSystemInitializer : MonoBehaviour
{
    [SerializeField] private AudioSource nonPositionalSource;
    [SerializeField] private AudioSource MusicSource;

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioMixerGroup musicMixerGroup;
    [SerializeField] private AudioMixerGroup sfxMixerGroup;

    private void Awake()
    {
        GameManager.AudioSystem.NonPositionAudioSource = nonPositionalSource;
        GameManager.AudioSystem.MusicAudioSource = MusicSource;

        GameManager.AudioSystem.GeneralMixer = audioMixer;
        GameManager.AudioSystem.Music = musicMixerGroup;
        GameManager.AudioSystem.VFX = sfxMixerGroup;

    }
}