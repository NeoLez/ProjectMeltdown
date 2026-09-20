using System;
using Root;
using UnityEngine;

namespace Timers {
    [RequireComponent(typeof(BoxCollider))]
    public class BoundingBox : MonoBehaviour {
        private Matrix4x4 _worldToLocalMatrix;
        private Vector3 _cachedCenter;
        private Vector3 _cachedHalfSize;
        [SerializeField] private CompositeBoundingBox composite;
        public event Action<BoundingBoxTracker> OnObjectAdded;
        public event Action<BoundingBoxTracker> OnObjectExited;

        private void Awake() {
            var col = GetComponent<BoxCollider>();
            _worldToLocalMatrix = col.transform.worldToLocalMatrix;
            _cachedCenter = col.center;
            _cachedHalfSize = col.size * 0.5f;
        }

        public MapSection GetMapSection() {
            return composite.GetMapSection();
        }
        
        private bool _compositeBoundingBoxSet;
        public void SetCompositeBoundingBox(CompositeBoundingBox compositeBoundingBox) {
            if (_compositeBoundingBoxSet) return;
            _compositeBoundingBoxSet = true;
            this.composite = compositeBoundingBox;
        }

        public void AddObject(BoundingBoxTracker tracker) {
            OnObjectAdded?.Invoke(tracker);
        }

        public void RemoveObject(BoundingBoxTracker tracker) {
            OnObjectExited?.Invoke(tracker);
        }
        
        public bool IsPointInside(Vector3 point)
        {
            Vector3 localPoint = _worldToLocalMatrix.MultiplyPoint3x4(point);
            
            localPoint -= _cachedCenter;
            
            return Mathf.Abs(localPoint.x) <= _cachedHalfSize.x &&
                   Mathf.Abs(localPoint.y) <= _cachedHalfSize.y &&
                   Mathf.Abs(localPoint.z) <= _cachedHalfSize.z;
        }

        private void OnValidate() {
            if (gameObject.layer != LayerMask.NameToLayer("BoundingBox"))
                gameObject.layer = LayerMask.NameToLayer("BoundingBox");
            GetComponent<BoxCollider>().isTrigger = true;
        }
    }
}