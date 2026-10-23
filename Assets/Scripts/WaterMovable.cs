using UnityEngine;

namespace Root
{
    public class WaterMovable : MonoBehaviour
    {
        [SerializeField] private float _minimumHeight;
        [SerializeField] private float _movementRate;
        [SerializeField] private PipeValve _pipeValve;

        private float _currentOffset;
        private Vector3 _startPosition;

        private void Awake()
        {
            _startPosition = transform.localPosition;
        }

        private void Update()
        {
            if (_pipeValve.Open)
            {
                _currentOffset += _movementRate * Time.deltaTime;
                if(_currentOffset > _minimumHeight) _currentOffset = _minimumHeight;
                transform.localPosition = _startPosition + Vector3.down * _currentOffset;
            }
        }
    }
}