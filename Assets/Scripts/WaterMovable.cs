using UnityEngine;
using UnityEngine.Assertions;

namespace Root
{
    public class WaterMovable : MonoBehaviour
    {
        [SerializeField] private float _minimumHeight;
        [SerializeField] private float _movementRate;
        [SerializeField] private Pipe pipe;

        private float _currentOffset;
        private Vector3 _startPosition;
        private Vector3 currentState;

        private void Awake()
        {
            _startPosition = transform.localPosition;
        }

        private void Update()
        {
            if (pipe.HasFluid())
            {
                _currentOffset += _movementRate * Time.deltaTime;
                if(_currentOffset > _minimumHeight) _currentOffset = _minimumHeight;
                transform.localPosition = _startPosition + Vector3.down * _currentOffset;
            }
        }

        private void OnValidate()
        {
            Assert.IsNotNull(pipe);
            Assert.IsTrue(pipe.IsConsumer());
        }
    }
}