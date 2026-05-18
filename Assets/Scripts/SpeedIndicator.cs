using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Root
{
    public class SpeedIndicator : MonoBehaviour
    {
        [SerializeField] private Train train;
        [SerializeField] private TMP_Text label;
        [SerializeField] private Image image;
        [SerializeField] private string format = "{0}";

        private void Update()
        {
            if (train.AlertSystem.alert == null) return;

            image.overrideSprite = train.AlertSystem.alert.arrow;
            label.text = string.Format(format, Mathf.RoundToInt(train.AlertSystem.alert.maxSpeed));
        }
    }
}
