using UnityEngine;

namespace Root {
    [CreateAssetMenu(fileName = "TrainAlert", menuName = "SOs/TrainAlert")]
    public class TrainAlertSO :  ScriptableObject {
        public Sprite arrow;
        public int maxSpeed;
    }
}