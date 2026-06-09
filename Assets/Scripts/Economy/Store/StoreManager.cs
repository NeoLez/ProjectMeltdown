using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Root
{
    public class StoreManager : MonoBehaviour
    {
        [SerializeField] private List<StoreItemData> items;
        [SerializeField] private List<StoreSpawnPoint> spawnPoints;
        [SerializeField] private List<Transform> priceCanvasSpawnPoint;
        [SerializeField] private GameObject priceCanvasPrefab;
        private List<MerchantHand> merchantHands = new();
        private List<StoreItemDisplay> itemsCreated = new();

        [SerializeField] private MerchantHand merchantHandPrefab;
        private void Start()
        {
            foreach (var spawnPoint in spawnPoints) {
                var hand = Instantiate(merchantHandPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation, transform);
                merchantHands.Add(hand);
            }
            GenerateStock();
        }

        private void GenerateStock()
        {
            for (int i = 0; i < spawnPoints.Count; i++)
            {
                MerchantHand hand = merchantHands[i];
                Transform priceCanvasSpawn = priceCanvasSpawnPoint[i];
                StoreItemData item = GetRandomItem();
                

                int price = Random.Range(item.minPrice, item.maxPrice + 1);

                GameObject obj = Instantiate(item.prefab);
                obj.GetComponent<StoreItemDisplay>()._storeHand = hand;
                obj.GetComponent<StoreItemDisplay>()._purchased = false;
                obj.GetComponent<StoreItemDisplay>().OnPurchased += (boughtHand, i) => {
                    Debug.Log("Purchased item " + item.itemName);
                    boughtHand.HideHand();
                    itemsCreated.Remove(i);
                    merchantHands.Remove(boughtHand);
                };
                itemsCreated.Add(obj.GetComponent<StoreItemDisplay>());
                var objBehaviour = obj.transform.GetChild(0);
                objBehaviour.GetComponent<Rigidbody>().isKinematic = true;

                GameObject canvasObj = Instantiate(priceCanvasPrefab,
                    priceCanvasSpawn.transform.position,
                    priceCanvasSpawn.rotation
                    );

                PriceCanvas priceCanvas = canvasObj.GetComponent<PriceCanvas>();

                if (priceCanvas != null)
                    priceCanvas.Initialize(price);

                StoreItemDisplay display = obj.GetComponentInChildren<StoreItemDisplay>();

                if (display != null)
                    display.Initialize(item, price, priceCanvas);
            }
        }

        private void OnDestroy() {
            for (int i = itemsCreated.Count - 1; i >= 0; i--) {
                Destroy(itemsCreated[i].gameObject);
            }
        }

        public void ShowItems() {
            Debug.Log("a");
            foreach (var hand in merchantHands) {
                Debug.Log("b");
                hand.ShowHand();
            }
        }

        public void HideItems() {
            foreach (var hand in merchantHands) {
                hand.HideHand();
            }
        }

        private StoreItemData GetRandomItem()
        {
            int totalWeight = 0;

            foreach (var item in items)
                totalWeight += item.weight;

            int randomWeight = Random.Range(0, totalWeight);

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