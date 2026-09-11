using Timers;
using UnityEngine;

namespace Root
{
    public class SpawnObj : MonoBehaviour
    {
        [SerializeField] private ItemGenerationPoolSo pool;
        void Start() {
            var seed = SeedUtils.Combine(new [] {GameManager.seed, (int)(transform.position.x * 10), (int)(transform.position.y * 10), (int)(transform.position.z * 10)});
            System.Random random = new (seed);
            var obj = pool.GetRandom(random).CreatePhysicalItem();
            obj.transform.position = transform.position;
            obj.transform.rotation = Quaternion.Euler(Vector3.up * Random.Range(0f, 360f));
            obj.transform.parent = transform.parent;
            obj.transform.GetChild(0).transform.position = transform.position;
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position, 0.3f);
        }
    }
}
