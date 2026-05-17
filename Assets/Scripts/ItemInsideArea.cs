using System.Collections.Generic;
using UnityEngine;

namespace Root {
    public class ItemInsideArea : MonoBehaviour {
        public HashSet<VisualContainer> _containers = new();

        private void OnTriggerEnter(Collider other) {
            if (other.TryGetComponent(out VisualContainer container)) {
                Debug.Log(other.name);
                _containers.Add(container);
            }
        }

        private void OnTriggerExit(Collider other) {
            if (other.TryGetComponent(out VisualContainer container)) {
                Debug.Log("Removed " +other.name);
                _containers.Remove(container);
            }
        }
    }
}