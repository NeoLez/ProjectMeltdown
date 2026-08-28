using TMPro;
using UnityEngine;

namespace Root
{
    public class StationDistanceUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;
        [SerializeField] private Train train;
        [SerializeField] private MapGeneration mapGeneration;

        private void Update()
        {
            MapSection station = mapGeneration.GetNextStation();

            if (station == null || mapGeneration.IsTrainInStation())
            {
                text.text = "";
                return;
            }

            float distance = Vector3.Distance(
                train.trainPosition.position,
                station.transform.position);

            text.text = $"Estacion a: {distance:0}m";
        }
    }
}