using System.Collections;
using UnityEngine;

namespace Root
{
    public class AnimationTriggers : MonoBehaviour
    {
        private bool _isShaking;
        private float shakeDuration = 2f;
        private float shakeMagnitude = 0.06f;
        [SerializeField] GameObject Objectt;
        private void TriggerInteraction()
        {
            StartCoroutine(ShakeFeedback(Objectt));
        }
        private IEnumerator ShakeFeedback(GameObject Obj)
        {
            _isShaking = true;
            Vector3 basePosition = Obj.transform.localPosition;
            float elapsed = 0f;

            while (elapsed < shakeDuration)
            {
                elapsed += Time.deltaTime;
                float damper = 1f - Mathf.Clamp01(elapsed / shakeDuration);
                Obj.transform.localPosition = basePosition + Random.insideUnitSphere * shakeMagnitude * damper;
                yield return null;
            }

            Obj.transform.localPosition = basePosition;
            _isShaking = false;
        }
    }
}
