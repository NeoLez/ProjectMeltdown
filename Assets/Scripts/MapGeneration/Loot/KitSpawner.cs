using UnityEngine;

namespace Root {
    [RequireComponent(typeof(Inventory))]
    public class KitSpawner : MonoBehaviour {
        public ItemGenerationPoolSo[] pools;
        public int[] startingMoney;
        
        private void Start() {
            SpawnItems(pools[GameManager.VeryUglyKitNumber]);
            EconomyManager.Instance.AddMoney(startingMoney[GameManager.VeryUglyKitNumber]);
        }

        private void SpawnItems(ItemGenerationPoolSo pool) {
            var inv = GetComponent<Inventory>();
            foreach (var item in pool.items) {
                inv.InsertItem(item.DefaultItemState.Clone());
            }
        }
    }
}