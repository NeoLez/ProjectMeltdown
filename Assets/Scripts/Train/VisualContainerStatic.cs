using UnityEngine;

namespace Root {
    public class VisualContainerStatic : MonoBehaviour {
        [SerializeField] public GameObject visuals;

        public void SetToPosition(Transform goal) {
            if (visuals == null) return;

            visuals.transform.parent = goal;

        }
        
        private void OnDestroy() {
            Destroy(visuals);
        }
    }
}