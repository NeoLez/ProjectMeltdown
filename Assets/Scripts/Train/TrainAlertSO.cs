using UnityEngine;

namespace Root {
    [CreateAssetMenu(fileName = "TrainAlert", menuName = "SO/TrainAlert")]
    public class TrainAlertSO :  ScriptableObject {
        public Sprite arrow;
        public int maxSpeed;
    }
}