using System;
using UnityEngine;

namespace Root {
    public class TrainPathWaypoint : MonoBehaviour {
        public float maxSpeed;
        public event Action OnTrainReached;

        public void TrainReached() {
            OnTrainReached?.Invoke();
        }
        
        private void OnDrawGizmos() {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, 1f);
        }
    }
}