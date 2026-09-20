using System;
using Root;
using UnityEngine;

namespace Timers {
    [RequireComponent(typeof(Rigidbody), typeof(Collider))]
    public class BoundingBoxTracker : MonoBehaviour {
        private int _boundingBoxLayer;
        [SerializeField] private BoundingBox _currentBoundingBox;
        private MapSection _currentMapSection;
        public event Action OnMapSectionRemoved;

        private void Awake() {
            _boundingBoxLayer = LayerMask.NameToLayer("BoundingBox");
        }

        private void OnTriggerEnter(Collider other) {
            if (other.gameObject.layer != _boundingBoxLayer || 
                !other.TryGetComponent(out BoundingBox boundingBox) ||
                boundingBox == _currentBoundingBox) return;

            if (_currentBoundingBox != null) {
                _currentBoundingBox.RemoveObject(this);
                _currentMapSection.OnMapSectionRemoved -= FireMapSectionRemoved;
            }

            _currentBoundingBox = boundingBox;
            _currentBoundingBox.AddObject(this);
            _currentMapSection = _currentBoundingBox.GetMapSection();
            _currentMapSection.OnMapSectionRemoved += FireMapSectionRemoved;
            transform.parent = _currentMapSection.transform;
        }

        private void OnTriggerExit(Collider other) {
            if (other.gameObject.layer != _boundingBoxLayer || 
                !other.TryGetComponent(out BoundingBox boundingBox) ||
                boundingBox != _currentBoundingBox) return;

            transform.parent = null;
            _currentBoundingBox.RemoveObject(this);
            _currentBoundingBox = null;
            _currentMapSection.OnMapSectionRemoved -= FireMapSectionRemoved;
            _currentMapSection = null;
        }

        private void FireMapSectionRemoved() {
            OnMapSectionRemoved?.Invoke();
        }

        private void OnDisable() {
            if (_currentMapSection == null) return;
            _currentMapSection.OnMapSectionRemoved -= FireMapSectionRemoved;
            _currentMapSection = null;
            _currentBoundingBox = null;
        }
    }
}