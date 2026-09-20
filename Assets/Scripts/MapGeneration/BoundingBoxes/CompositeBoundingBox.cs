using System.Collections.Generic;
using System.Linq;
using Root;
using UnityEngine;

namespace Timers {
    public class CompositeBoundingBox : MonoBehaviour {
        [SerializeField] private List<BoundingBox> boundingBoxes = new();
        [SerializeField] private MapSection mapSection;
        private HashSet<BoundingBoxTracker> _objects = new();

        private void Awake() {
            foreach (var boundingBox in boundingBoxes) {
                boundingBox.OnObjectAdded += tracker => _objects.Add(tracker);
                boundingBox.OnObjectExited += tracker => _objects.Remove(tracker);
            }
        }

        public IEnumerable<BoundingBoxTracker> GetObjects() {
            return _objects;
        }

        public MapSection GetMapSection() {
            return mapSection;
        }

        private void OnValidate() {
            if(boundingBoxes.Count != boundingBoxes.ToHashSet().Count)
                Debug.LogWarning("Duplicate bounding box references. You should remove the duplicates.", this);
            foreach(var boundingBox in boundingBoxes)
                boundingBox.SetCompositeBoundingBox(this);
        }
    }
}