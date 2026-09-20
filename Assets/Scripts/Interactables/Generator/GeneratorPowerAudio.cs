using UnityEngine;

namespace Root
{
    public class GeneratorPowerAudio : MonoBehaviour
    {
        [SerializeField] private GeneratorPowerFeed powerFeed;
        [SerializeField] private StationLightingController lighting;
        [SerializeField] private AudioSource buzzSource;
        [SerializeField] private AudioSource[] powerOnSources;
        [SerializeField] private AudioClip soundDepleted;
        [SerializeField] private AudioClip soundBatteryRemoved;
        [SerializeField] private AudioClip soundPowerRestored;

        private AudioSource _audioSource;
        private bool _unstable;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _audioSource.playOnAwake = false;
            _audioSource.spatialBlend = 0f;
        }

        private void Start()
        {
            powerFeed.OnDepleted += HandleDepleted;
            powerFeed.OnBatteryRemoved += HandleBatteryRemoved;
            powerFeed.OnPowerRestored += HandlePowerRestored;
            powerFeed.OnPowerPercentChanged += HandlePercentChanged;
        }

        private void OnDestroy()
        {
            if (powerFeed == null) return;
            powerFeed.OnDepleted -= HandleDepleted;
            powerFeed.OnBatteryRemoved -= HandleBatteryRemoved;
            powerFeed.OnPowerRestored -= HandlePowerRestored;
            powerFeed.OnPowerPercentChanged -= HandlePercentChanged;
        }

        private void HandleDepleted()
        {
            StopPowerOn();

            if (soundDepleted != null)
                _audioSource.PlayOneShot(soundDepleted);
        }

        private void HandleBatteryRemoved()
        {
            StopPowerOn();

            if (soundBatteryRemoved != null)
                _audioSource.PlayOneShot(soundBatteryRemoved);
        }

        private void HandlePowerRestored()
        {
            PlayPowerOn(); 

            if (soundPowerRestored != null)
                _audioSource.PlayOneShot(soundPowerRestored);
        }

        private void PlayPowerOn()
        {
            if (powerOnSources == null) return;
            foreach (AudioSource s in powerOnSources)
                if (s != null) s.Play();
        }

        private void StopPowerOn()
        {
            if (powerOnSources == null) return;
            foreach (AudioSource s in powerOnSources)
                if (s != null) s.Stop();
        }

        private void HandlePercentChanged(float percent)
        {
            if (lighting == null || buzzSource == null) return;

            bool unstable = percent > 0f && percent <= lighting.InstabilityThreshold;
            if (unstable == _unstable) return;
            _unstable = unstable;

            if (unstable) buzzSource.Play();
            else buzzSource.Stop();
        }
    }
}