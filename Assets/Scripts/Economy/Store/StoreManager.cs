using System.Collections.Generic;
using UnityEngine;

namespace Root
{
    public class StoreManager : MonoBehaviour
    {
        [SerializeField] private List<StoreItemData> items;
        [SerializeField] private List<StoreSpawnPoint> spawnPoints;

        private void Start()
        {
            GenerateStock();
        }

        private void GenerateStock()
        {
            foreach (var point in spawnPoints)
            {
                StoreItemData item = GetRandomItem();

                int price =
                    Random.Range(item.minPrice,
                                 item.maxPrice + 1);

                GameObject obj =
                    Instantiate(item.prefab,
                        point.transform.position,
                        point.transform.rotation);

                StoreItemDisplay display =
                    obj.GetComponentInChildren<StoreItemDisplay>();

                if (display != null)
                    display.Initialize(item, price);
            }
        }

        private StoreItemData GetRandomItem()
        {
            int totalWeight = 0;

            foreach (var item in items)
                totalWeight += item.weight;

            int randomWeight =
                Random.Range(0, totalWeight);

            foreach (var item in items)
            {
                randomWeight -= item.weight;

                if (randomWeight < 0)
                    return item;
            }

            return items[0];
        }
    }
}