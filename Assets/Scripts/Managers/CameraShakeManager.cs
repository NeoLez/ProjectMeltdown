using UnityEngine;
using Random = UnityEngine.Random;

namespace Root.Managers {
    public class CameraShakeManager : MonoBehaviour {
        public static CameraShakeManager Instance { get; private set; }
        private void Awake() {
            Instance = this;
        }

        private float _shakeIntensity;
        private float _targetShakeIntensity;
        private float _shakeTime;
        private readonly float _shakeIntensityLerp = 0.05f;
        public Vector3 GetShakeOffset()
        {
            return new Vector3(
                Random.Range(-1f, 1f) * _shakeIntensity,
                Random.Range(-1f, 1f) * _shakeIntensity,
                0f
            );
        }

        public void Shake(float intensity, float time) {
            _targetShakeIntensity = intensity;
            _shakeTime = time + Time.time;
        }

        private void Update() {
            if (_shakeTime < Time.time) {
                _shakeIntensity = Mathf.Lerp(_shakeIntensity, 0, _shakeIntensityLerp);
                return;
            }
            _shakeIntensity = Mathf.Lerp(_shakeIntensity, _targetShakeIntensity, _shakeIntensityLerp);
        }
    }
}