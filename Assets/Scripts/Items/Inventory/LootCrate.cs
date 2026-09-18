using System.Collections.Generic;
using Timers;
using UnityEngine;

namespace Root {
    [RequireComponent(typeof(Inventory))]
    public class LootCrate : MonoBehaviour {
        [SerializeField] private ItemGenerationPoolSo pool;
        [SerializeField] private int minAmountOfItemsToGenerate;
        [SerializeField] private int maxAmountOfItemsToGenerate;
        private void Start() {
            //TODO: Since this uses global positions it could be unstable in the long run due to small changes in the rebase calculation. Maybe switch to local position respective to the MapSection?
            var seed = SeedUtils.Combine(new [] {GameManager.seed, (int)(transform.position.x * 10), (int)(transform.position.y * 10), (int)(transform.position.z * 10)});
            var random = new System.Random(seed);
            List<ItemSo> itemsToGenerate = new();
            for (int i = 0; i < random.Next(minAmountOfItemsToGenerate, maxAmountOfItemsToGenerate); i++) {
                itemsToGenerate.Add(pool.GetRandom(random));
            }
            
            var inv = GetComponent<Inventory>();
            itemsToGenerate.Sort((item1, item2) => {
                var size1 = GetItemSize(item1.InventorySize);
                var size2 = GetItemSize(item2.InventorySize);
                if (size1 > size2)
                    return -1;
                if (size1 < size2)
                    return 1;
                return 0;
            });

            foreach (var item in itemsToGenerate) {
                inv.InsertItem(item.DefaultItemState);
            }
        }

        private int GetItemSize(Vector2Int v) {
            return v.x * v.y;
        }
    }
}