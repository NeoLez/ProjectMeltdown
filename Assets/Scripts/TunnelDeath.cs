using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static Root.MapPointsGen;

namespace Root
{
    public class TunnelDeath : MonoBehaviour
    {
        [SerializeField] private Collider trigger;
        [SerializeField] private Image blurEffect;
        [SerializeField] private float distanceToStartTimer;
        [SerializeField] private float fadeInMultiplier;
        [SerializeField] private float fadeOutMultiplier;
        [SerializeField] private float fadeInDuration;
        [SerializeField] private float fadeOutDuration;
        [SerializeField] private AudioSource creepySound;
        [SerializeField] private float creepySoundMaxVolume;

        private float _currentTimer;
        private Coroutine _timerCoroutine;
        private bool _playerLeftTrain;
        private float _alpha;
        private int _playerLayer;
        private enum FadeState { None, FadingIn, FadingOut }
        private FadeState _currentFadeState = FadeState.None;

        private void Awake()
        {
            _playerLayer = LayerMask.NameToLayer("Player");
        }

        private void Start()
        {
            _currentTimer = 0;
            SetBlurAlpha(0);

            if (blurEffect != null) blurEffect.raycastTarget = false;

            GameManager.AudioSystem.AssignOutputMixerGroup(creepySound, GameManager.AudioSystem.VFX);
        }

        private void Update()
        {
            if (_playerLeftTrain)
            {
                float distance = Vector3.Distance(GameManager.Player.transform.position, GameManager.Train.pivot.position);

                if (distance > distanceToStartTimer)
                {
                    if (_currentFadeState != FadeState.FadingIn)
                    {
                        StartFade(FadeInRoutine(), FadeState.FadingIn);
                    }
                }
                else
                {
                    if (_currentFadeState != FadeState.FadingOut && _currentTimer > 0f)
                    {
                        StartFade(FadeOutRoutine(), FadeState.FadingOut);
                    }
                }
            }
        }

        private void StartFade(IEnumerator routine, FadeState newState)
        {
            if (_timerCoroutine != null)
            {
                StopCoroutine(_timerCoroutine);
            }

            _currentFadeState = newState;
            _timerCoroutine = StartCoroutine(routine);
        }

        IEnumerator FadeInRoutine()
        {
            float elapsedTime = 0;
            while (elapsedTime < fadeInDuration)
            {
                elapsedTime += fadeInMultiplier * Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / fadeInDuration);
                _currentTimer = elapsedTime;
                //alpha = elapsedTime;
                //_currentTimer += timerMultiplier * Time.deltaTime;
                SetBlurAlpha(t);
                SetTunnelSound(0, creepySoundMaxVolume, t);
                yield return null;
            }

            KillPlayer();
        }

        IEnumerator FadeOutRoutine()
        {
            float elapsedTime = 0;
            float startVolume = creepySound.volume;
            while (elapsedTime < fadeOutDuration)
            {
                elapsedTime += fadeOutMultiplier * Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / fadeOutDuration);
                _currentTimer = elapsedTime;
                //_currentTimer -= timerMultiplier * Time.deltaTime;
                SetBlurAlpha(Mathf.Lerp(_alpha, 0, t));
                SetTunnelSound(startVolume, 0, t);
                yield return null;
            }

            _currentTimer = 0f;
            _alpha = 0;
            SetBlurAlpha(0f);
            creepySound.volume = 0;
            _currentFadeState = FadeState.None;
        }

        private void SetBlurAlpha(float value)
        {
            _alpha = value;
            Color color = blurEffect.color;
            color.a = _alpha;
            blurEffect.color = color;
        }

        private void SetTunnelSound(float initialValue, float finalValue, float time)
        {
            creepySound.volume = Mathf.Lerp(initialValue, finalValue, time);
        }

        private void OnTriggerEnter(Collider collision)
        {
            if (collision.gameObject.layer == _playerLayer && collision.gameObject == GameManager.Player.gameObject)
            {
                StartFade(FadeOutRoutine(), FadeState.FadingOut);

                _playerLeftTrain = false;
            }
        }

        private void OnTriggerExit(Collider collision)
        {
            if (!GameManager.Train.IsStopped()) return;

            if (collision.gameObject.layer == _playerLayer && collision.gameObject == GameManager.Player.gameObject)
            {
                _playerLeftTrain = true;
            }
        }

        private void KillPlayer()
        {
            if (creepySound != null) creepySound.Stop();
            //Debug.Log("mori");
            GameManager.Player.gameObject.GetComponent<HealthControl>().InstantDeath();
        }
    }
}
