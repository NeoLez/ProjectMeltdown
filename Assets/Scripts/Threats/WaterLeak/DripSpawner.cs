using UnityEngine;

namespace Root
{
    public class DripSpawner : MonoBehaviour
    {
        [Header("Prefab")]
        [SerializeField] private GameObject _dropPrefab;

        [Header("Timing")]
        [SerializeField] private float _minInterval = 2f;
        [SerializeField] private float _maxInterval = 5f;

        [Header("Spawn Point")]
        [SerializeField] private Transform _spawnPoint;

        private float _timer;
        private float _nextDropTime;

        private void Start()
        {
            SetNextDropTime();
        }

        private void Update()
        {
            _timer += Time.deltaTime;

            if (_timer >= _nextDropTime)
            {
                SpawnDrop();
                _timer = 0f;
                SetNextDropTime();
            }
        }

        private void SetNextDropTime()
        {
            _nextDropTime = Random.Range(_minInterval, _maxInterval);
        }

        private void SpawnDrop()
        {
            if (_dropPrefab == null) return;

            Vector3 pos = _spawnPoint != null ? _spawnPoint.position : transform.position;
            Instantiate(_dropPrefab, pos, Quaternion.identity);
        }

        private void OnDrawGizmosSelected()
        {
            Vector3 pos = _spawnPoint != null ? _spawnPoint.position : transform.position;
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(pos, 0.15f);
            Gizmos.DrawLine(pos, pos + Vector3.down * 6.5f);
        }
    }
}