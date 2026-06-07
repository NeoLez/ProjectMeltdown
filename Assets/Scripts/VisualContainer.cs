using System;
using UnityEngine;

namespace Root {
    [Serializable]
    public class VisualContainer : MonoBehaviour {
        [SerializeField] public GameObject visuals;
        public Transform originCenter;
        public Transform goal;
        Animator anim;

        private void Start() {
            originCenter = GameManager.Train.transform;
            if (visuals != null) { anim = visuals.GetComponentInChildren<Animator>(); }            
        }

        private void Update() {
            if (visuals == null) return;
            if (goal == null) {
                visuals.transform.position = transform.position;
                visuals.transform.rotation = transform.rotation;
            }
            else {
                Vector3 localPos = originCenter.InverseTransformPoint(transform.position);
                Vector3 worldPos = goal.TransformPoint(localPos);

                
                Quaternion localRot = Quaternion.Inverse(originCenter.rotation) * transform.rotation;
                Quaternion worldRot = goal.rotation * localRot;

                visuals.transform.SetPositionAndRotation(worldPos, worldRot);
                Debug.DrawRay(visuals.transform.position, visuals.transform.forward, Color.green);
                Debug.DrawRay(transform.position, transform.forward, Color.red);
            }
        }

        private void OnDestroy() {
            Destroy(visuals);
        }

        public void PlayAnimation(bool an)
        {
            anim.enabled = an;
        }
    }
}