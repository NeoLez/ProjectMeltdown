using UnityEngine;
using UnityEngine.Audio;

namespace Root
{
    public class AudioForObjects : MonoBehaviour
    {
        [SerializeField] AudioClip sound;

        AudioSource _currentSource;

        void Start()
        {
            _currentSource = GetComponent<AudioSource>();
            if (_currentSource != null)
            {
                GameManager.AudioSystem.AssignOutputMixerGroup(_currentSource, GameManager.AudioSystem.VFX);
            }
            else
            {
                AudioSource newAudioSource = gameObject.AddComponent<AudioSource>(); //hacer parametros configurables!
                _currentSource = newAudioSource;
                _currentSource.clip = sound;
                _currentSource.loop = true;
                _currentSource.spatialBlend = 1;

                GameManager.AudioSystem.AssignOutputMixerGroup(_currentSource, GameManager.AudioSystem.VFX);
                _currentSource.Play();
            }
        }
    }
}
