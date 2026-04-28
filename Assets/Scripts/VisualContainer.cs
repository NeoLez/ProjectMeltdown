using UnityEngine;

namespace Root {
    public class VisualContainer : MonoBehaviour {
        [SerializeField] private GameObject visuals;
        public Transform originCenter;
        public Transform goal;

        private void Start() {
            originCenter = GameManager.Train.transform;
        }

        private void Update() {
            if (goal == null) {
                visuals.transform.position = transform.position;
                visuals.transform.rotation = transform.rotation;
            }
            else {
                visuals.transform.position = goal.TransformPoint(originCenter.InverseTransformPoint(transform.position));
                visuals.transform.forward = goal.TransformDirection(originCenter.InverseTransformDirection(transform.forward));
            }
        }
    }
}