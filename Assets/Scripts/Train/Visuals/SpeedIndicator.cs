using System;
using Timers;
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
            MakeVisible();
            flickerTimer.Reset(flickerSpeed);
            image.overrideSprite = train.AlertSystem.currentAlert.Item1.arrow;
            label.text = string.Format(format, Mathf.RoundToInt(train.AlertSystem.currentAlert.Item1.maxSpeed));
        }
        
        
        [SerializeField] private float flickerDistance;
        [SerializeField] private float flickerSpeed;
        private bool _isVisible = true;
        private Timer flickerTimer = new();
        private void Update() {
            var dist = Vector3.Distance(train.AlertSystem.currentAlert.Item2.transform.position, train.trainPosition.position);
            if (dist < flickerDistance) {
                if (flickerTimer.IsCompleted()) {
                    if (_isVisible) {
                        MakeInvisible();
                    }
                    else {
                        MakeVisible();
                    }
                    flickerTimer.AddTime(flickerSpeed);
                }
            }
            else {
                flickerTimer.Reset(flickerSpeed);
            }
        }
        
        private void MakeInvisible() {
            _isVisible = false;
            image.enabled = false;
            label.enabled = false;
        }

        private void MakeVisible() {
            _isVisible = true;
            image.enabled = true;
            label.enabled = true;
        }
    }
}
