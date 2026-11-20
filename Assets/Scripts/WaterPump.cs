using System;
using UnityEngine;
using System.Collections;

namespace Root
{
    public class WaterPump : InteractableNormalCamera
    {
        [SerializeField] private Pipe pipe;
        [SerializeField] private bool alwaysOn;
        [SerializeField] private float pumpTime;
        [SerializeField] private WaterMovable water;
        [SerializeField] private AudioSource activePump;
        [SerializeField] private Transform shakeTarget; 
        [SerializeField] private float shakeMagnitude = 0.01f;
        [SerializeField] private AudioClip _soundFail;
        [SerializeField] private float failShakeDuration = 0.3f; 
        private Coroutine _shakeRoutine; 
        private Vector3 _shakeBasePosition; 
        public event Action OnActivate;
        private bool _locked; 

        private void Start()
        {
            if(activePump!=null) GameManager.AudioSystem.AssignOutputMixerGroup(activePump, GameManager.AudioSystem.VFX);
        }

        private void Update()
        {
            if(alwaysOn && !pipe.HasFluid() && !_locked && !water.IsLow) 
                SendWater();
        }

        public override void Interact()
        {
            if (_locked || alwaysOn || pipe.HasFluid()) return; 

            if (water.IsLow || !SendWater())
                PlayFailFeedback();
        }

        private bool SendWater() 
        {
            if (pipe.HasFluid()) return true; 
            var outputAmount = pipe.FindConsumers(null, true);
            if (outputAmount == 0)
            {
                return false;
            };

            pipe.SendSignal(null, true);
            OnActivate?.Invoke();
            _locked = true; 
            CancelInvoke(nameof(StopShake)); 
            StartShake();
            if (activePump != null) StopAudio(false);
            Invoke(nameof(FlushFluid), pumpTime);
            return true; 
        }

        public void StopAudio(bool state)
        {
            if (state)
            {
                if (activePump != null) activePump.Stop();
                StopShake();
                _locked = false;
            }
            else activePump.Play();
        }

        private void FlushFluid()
        {
            pipe.SendSignal(null, false);
        }

        private void StartShake() 
        {
            if (shakeTarget == null || _shakeRoutine != null) return; 

            _shakeBasePosition = shakeTarget.localPosition; 
            _shakeRoutine = StartCoroutine(ShakeRoutine()); 
        }

        private void StopShake() 
        {
            if (_shakeRoutine == null) return; 

            StopCoroutine(_shakeRoutine); 
            _shakeRoutine = null;
            shakeTarget.localPosition = _shakeBasePosition; 
        }
        private void PlayFailFeedback()
        {
            _locked = true; 
            float lockTime = failShakeDuration; 

            if (_soundFail != null)
            {
                GameManager.AudioSystem.PlaySoundPositional(_soundFail, transform.position, GameManager.AudioSystem.VFX);
                lockTime = Mathf.Max(lockTime, _soundFail.length); 
            }

            if (_shakeRoutine == null) 
            {
                StartShake(); 
                Invoke(nameof(StopShake), failShakeDuration); 
            }

            Invoke(nameof(UnlockInteraction), lockTime); 
        }
        private IEnumerator ShakeRoutine() 
        {
            while (true) 
            {
                shakeTarget.localPosition = _shakeBasePosition + UnityEngine.Random.insideUnitSphere * shakeMagnitude; 
                yield return null;
            }
        }
        private void UnlockInteraction() 
        {
            _locked = false; 
        }
    }
}