using System;
using UnityEngine;

namespace Root {
    [Serializable]
    public class VisualContainer : MonoBehaviour {
        [SerializeField] public GameObject visuals;
        public Transform originCenter;
        public Transform goal;
        private bool MatchesRealPosition = true;
        public event Action OnMatchRealPosition;
        public event Action OnChangeToVirtualPosition;

        private void Start() {
            originCenter = GameManager.Train.transform;
        }

        private void LateUpdate() {
            if (visuals == null) return;
            
            if (goal == null) {
                if (!MatchesRealPosition) {
                    MatchesRealPosition = true;
                    OnMatchRealPosition?.Invoke();
                }
                visuals.transform.position = transform.position;
                visuals.transform.rotation = transform.rotation;
            }
            else {
                if (MatchesRealPosition) {
                    MatchesRealPosition = false;
                    OnChangeToVirtualPosition?.Invoke(); 
                }

                Vector3 localPos = originCenter.InverseTransformPoint(transform.position);
                Vector3 worldPos = goal.TransformPoint(localPos);
                
                Quaternion localRot = Quaternion.Inverse(originCenter.rotation) * transform.rotation;
                Quaternion worldRot = goal.rotation * localRot;

                visuals.transform.SetPositionAndRotation(worldPos, worldRot);
            }
        }

        private void OnDestroy() {
            if (visuals != null) Destroy(visuals);
        }
    }
}