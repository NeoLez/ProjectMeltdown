using System;
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

        private void Awake() {
            train.AlertSystem.OnEventChanged += ChangeStuff;
        }

        private void OnDestroy() {
            train.AlertSystem.OnEventChanged -= ChangeStuff;
        }

        private void ChangeStuff() {
            image.overrideSprite = train.AlertSystem.currentAlert.arrow;
            label.text = string.Format(format, Mathf.RoundToInt(train.AlertSystem.currentAlert.maxSpeed));
        }
    }
}
