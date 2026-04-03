using UnityEngine;

namespace Root {
    public class TrainMovingPlatform : MovingPlatform {
        [SerializeField] private Train train;
        
        public override Vector3 GetSpeed() {
            return train.GetSpeed();
        }
    }
}