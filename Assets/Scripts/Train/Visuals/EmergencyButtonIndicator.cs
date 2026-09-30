using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Root
{
    public class EmergencyButtonIndicator : MonoBehaviour
    {
        [SerializeField] private RawImage discIcon;
        [SerializeField] DiscSlot discSlot;
        [SerializeField] private Color emptyColor = Color.white;
        [SerializeField] private float blinkInterval = 0.5f;
        [SerializeField] private Color[] usageColors = { Color.red, Color.yellow, Color.green };

        private Coroutine _blinkRoutine;

        private void Update()
        {
            BrakeDiscItem disc = discSlot.GetBrakeDisc();

            if (disc == null)
            {
                if (_blinkRoutine == null)
                    _blinkRoutine = StartCoroutine(BlinkRoutine());
                return;
            }

            if (_blinkRoutine != null)
            {
                StopCoroutine(_blinkRoutine);
                _blinkRoutine = null;
            }

            UpdateDiscColor(disc);
        }

        private void UpdateDiscColor(BrakeDiscItem disc)
        {
            if (usageColors.Length == 0) return;

            int index = Mathf.Clamp(disc.GetDiscUsage(), 0, usageColors.Length - 1);
            Color color = usageColors[index];
            color.a = 1f;
            discIcon.color = color;
        }

        private IEnumerator BlinkRoutine()
        {
            bool visible = true;
            while (true)
            {
                Color color = emptyColor;
                color.a = visible ? 1f : 0f;
                discIcon.color = color;
                visible = !visible;
                yield return new WaitForSeconds(blinkInterval);
            }
        }
    }
}