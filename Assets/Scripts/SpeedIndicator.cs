using TMPro;
using UnityEngine;

namespace Root
{
    public class SpeedIndicator : MonoBehaviour
    {
        [SerializeField] private Train train;
        [SerializeField] private TMP_Text label;
        [SerializeField] private string format = "{0} km/h";

        private void Update()
        {
            float speed = train.IsStopped() ? 0 : train.GetCurrentMaxSpeed();
            label.text = string.Format(format, Mathf.RoundToInt(speed));
        }
    }
}
