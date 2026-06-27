using UnityEngine;
using Random = UnityEngine.Random;

namespace Root {
    public class KitSpawner : MonoBehaviour {
        public ItemGenerationPoolSO[] pools;
        public Transform[] spawnPoints;
        
        private void Start() {
            Debug.Log("Spawning Kit " + GameManager.VeryUglyKitNumber);
            SpawnItems(pools[GameManager.VeryUglyKitNumber]);
        }

        private void SpawnItems(ItemGenerationPoolSO pool) {
            int Spawn = 0;
            foreach (var item in pool.items) {
                Debug.Log(item.name);
                SpawnItem(item, spawnPoints[Spawn]);
                Spawn = (Spawn + 1) % pool.items.Count;
            }
        }

        private void SpawnItem(GameObject item, Transform spawnPoint) {
            var obj = Instantiate(item);
            obj.transform.parent = transform.parent;
            obj.transform.position = spawnPoint.position;
            obj.transform.rotation = Quaternion.Euler(Vector3.up * Random.Range(0f, 360f));
            obj.transform.GetChild(0).transform.position = spawnPoint.position;
        }
    }
}