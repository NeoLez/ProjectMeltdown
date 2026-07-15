using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioSystem
{
    public AudioSource NonPositionAudioSource;
    public AudioSource MusicAudioSource;
    private List<AudioSource> currentlyLoopingSounds = new();
    public AudioMixerGroup Music;
    public AudioMixerGroup VFX;
    public AudioMixer GeneralMixer;

    private List<AudioSource> pausedSources = new();

    public void PauseAll()
    {
        pausedSources.Clear();
        AudioSource[] all = Object.FindObjectsOfType<AudioSource>();
        foreach (AudioSource s in all)
        {
            if (s.isPlaying)
            {
                s.Pause();
                pausedSources.Add(s);
            }
        }
    }

    public void ResumeAll()
    {
        foreach (AudioSource s in pausedSources)
        {
            if (s != null)
                s.UnPause();
        }
        pausedSources.Clear();
    }

    public void PlayMusic(AudioClip audioClip, AudioMixerGroup mixerGroup, float volume = 1)
    {
        AudioSource audioSource = MusicAudioSource;
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.loop = true;
        audioSource.outputAudioMixerGroup = mixerGroup;

        audioSource.Play();
    }

    public void PlaySound(AudioClip audioClip, AudioMixerGroup mixerGroup, float volume = 1) 
    {
        AudioSource audioSource = NonPositionAudioSource;
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.outputAudioMixerGroup = mixerGroup;

        NonPositionAudioSource?.PlayOneShot(audioClip, volume);
    }

    public void PlaySoundPositional(AudioClip audioClip, Vector3 position, AudioMixerGroup mixerGroup, float volume = 1, float maxDistance = 50f) 
    {
        GameObject source = new GameObject("PositionalAudioSource");
        source.transform.position = position;

        AudioSource audioSource = source.AddComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.spatialBlend = 1f;
        audioSource.maxDistance = maxDistance;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.outputAudioMixerGroup = mixerGroup;

        audioSource.Play();
        Object.Destroy(source, audioClip.length);
    }

    public AudioSource PlaySoundLooping(AudioClip audioClip, Vector3 position, AudioMixerGroup mixerGroup, float volume = 1)
    {
        GameObject go = new GameObject();
        go.transform.position = position;
        AudioSource audioSource = go.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1;
        audioSource.loop = true;
        audioSource.volume = volume;
        audioSource.clip = audioClip;
        audioSource.outputAudioMixerGroup = mixerGroup;

        audioSource.Play();
        return audioSource;
    }

    public AudioSource PlaySoundLooping(AudioClip audioClip, AudioMixerGroup mixerGroup, float volume = 1)
    {
        GameObject go = new GameObject();
        AudioSource audioSource = go.AddComponent<AudioSource>();
        audioSource.spatialBlend = 0;
        audioSource.loop = true;
        audioSource.volume = volume;
        audioSource.clip = audioClip; 
        audioSource.outputAudioMixerGroup = mixerGroup;

        audioSource.Play();
        return audioSource;
    }
}