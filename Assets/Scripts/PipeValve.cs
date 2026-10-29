using System;
using System.Collections.Generic;
using PrimeTween;
using UnityEngine;

namespace Root
{
    [RequireComponent(typeof(Pipe))]
    public class PipeValve : InteractableNormalCamera
    {
        [Serializable]
        private struct ConnectionInfo
        {
            public float angle;
            public List<Pipe> connections;
        }
        [SerializeField] private float rotationTime;
        [SerializeField] private Transform visuals;
        private Pipe _pipe;
        [SerializeField] private List<ConnectionInfo> connections;
        private int _currentIndex = -1;

        private void Awake()
        {
            _pipe = GetComponent<Pipe>();
            SetConnection();
        }

        private bool _isAnimating;
        public override void Interact()
        {
            if (_isAnimating || _pipe.HasFluid()) return;
            _isAnimating = true;
            SetConnection();

            var targetRotation = Quaternion.Euler(new Vector3(0, 0, connections[_currentIndex].angle));
            Tween.LocalRotation(visuals.transform, visuals.localRotation, targetRotation, rotationTime).OnComplete(() => _isAnimating = false);
        }

        private void SetConnection()
        {
            _currentIndex = (_currentIndex + 1) % connections.Count;
            _pipe.connections = connections[_currentIndex].connections;
        }
    }
}