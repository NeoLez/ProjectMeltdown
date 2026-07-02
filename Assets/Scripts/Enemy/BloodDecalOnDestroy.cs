using UnityEngine;

namespace Root.Enemy
{
    public class BloodSplatter : MonoBehaviour
    {
        [Header("Decals")]
        [SerializeField] private GameObject[] bloodDecals;

        [Header("Spawn")]
        [SerializeField] private int splatterCount = 12;
        [SerializeField] private float maxDistance = 2f;
        [SerializeField] private LayerMask surfaceMask;

        [Header("Random")]
        [SerializeField] private Vector2 randomScale = new(0.8f, 1.3f);

        private bool _spawned;

        public void SpawnBlood(Vector3 origin)
        {
            if (_spawned)
                return;

            _spawned = true;

            for (int i = 0; i < splatterCount; i++)
            {
                ShootBloodRay(origin);
            }
        }

        private void ShootBloodRay(Vector3 origin)
        {
            Vector3 direction = (Random.onUnitSphere + Vector3.down * 0.4f).normalized;

            float distance = Random.Range(maxDistance * 0.3f, maxDistance);

            if (!Physics.Raycast(origin, direction, out RaycastHit hit, distance, surfaceMask))
                return;

            GameObject prefab = bloodDecals[Random.Range(0, bloodDecals.Length)];

            Quaternion rotation = Quaternion.LookRotation(-hit.normal);
            rotation *= Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));

            GameObject decal = Instantiate(
                prefab,
                hit.point + hit.normal * 0.02f,
                rotation);

            float t = distance / maxDistance;
            float scale = Random.Range(randomScale.x, randomScale.y) * Mathf.Lerp(1.2f, 0.8f, t);

            decal.transform.localScale *= scale;
        }
    }
}