using System.Collections.Generic;
using UnityEngine;

namespace Root {
    public class ItemInsideArea : MonoBehaviour {
        public HashSet<VisualContainer> _containers = new();

        private void OnTriggerEnter(Collider other) {
            if (other.TryGetComponent(out VisualContainer container)) {
                _containers.Add(container);
                //Debug.Log("Added "+container.transform.parent.name);
            }
        }

        private void OnTriggerExit(Collider other) {
            if (other.TryGetComponent(out VisualContainer container)) {
                _containers.Remove(container);
                //Debug.Log("Removed "+container.transform.parent.name);
            }
        }
    }
}