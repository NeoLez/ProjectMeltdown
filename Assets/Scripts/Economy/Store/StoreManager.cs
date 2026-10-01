using Root.Managers;
using System;
using System.Collections.Generic;
using Timers;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.SmartFormat.Core.Parsing;

namespace Root
{
    public class StoreManager : MonoBehaviour
    {
        [SerializeField] private bool isSingleItemSpawn;
        [Header("Buy")]
        [SerializeField] private StoreItemData forcedSpawnItem;
        [SerializeField] private StoreItemPoolSO storeItemPool;
        [SerializeField] private List<StoreSpawnPoint> spawnPoints;
        [SerializeField] private List<Transform> priceCanvasSpawnPoint;
        [SerializeField] private GameObject priceCanvasPrefab;
        [SerializeField] private MapSection mapSection;
        private List<MerchantHand> merchantHands = new();
        private List<StoreItemDisplay> itemsCreated = new();

        [SerializeField] private MerchantHand merchantHandPrefab;

        [Header("Tutorial Courtesy Item")]
        [SerializeField] private StoreItemData initialItemSpawn;
        [SerializeField] private StoreSpawnPoint initialItemSpawnPoint;
        [SerializeField] private Transform singlePriceSpawnPoint;
        private List<MerchantHand> initialMerchantHands = new();
        public bool CanSpawnMultipleItems => !isSingleItemSpawn;

        [Header("Sell")]
        [SerializeField] private SelllDeposit deposit;
        [SerializeField] private Transform dropPivot;
        [SerializeField] private MerchantInteraction merchantDialogue;

        private System.Random _random;
        
        private void Start()
        {
            if(isSingleItemSpawn)
            {
                var hand = Instantiate(merchantHandPrefab, initialItemSpawnPoint.transform.position, initialItemSpawnPoint.transform.rotation, transform);
                initialMerchantHands.Add(hand);

                GenerateCourtesyItem();
            }

            _random  = new System.Random(mapSection.GetMapSectionSeed() + SeedUtils.TextToSeed("Shop"));
        }

        public void GenerateStoreItems()
        {
            if (isSingleItemSpawn)
            {
                var hand = Instantiate(merchantHandPrefab, initialItemSpawnPoint.transform.position, initialItemSpawnPoint.transform.rotation, transform);
                initialMerchantHands.Add(hand);

                GenerateCourtesyItem();
            }
            else
            {
                if (AreAllItemsGenerated()) return;

                foreach (var spawnPoint in spawnPoints)
                {
                    var hand = Instantiate(merchantHandPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation, transform);
                    merchantHands.Add(hand);
                }
                GenerateStock();
            }
        }

        private bool _forcedSpawn;
        private void GenerateStock()
        {
            for (int i = 0; i < spawnPoints.Count; i++)
            {
                MerchantHand hand = merchantHands[i];
                Transform priceCanvasSpawn = priceCanvasSpawnPoint[i];

                StoreItemData item;
                if (forcedSpawnItem != null && !_forcedSpawn)
                {
                    item = forcedSpawnItem;
                    _forcedSpawn = true;
                }
                else
                    item = storeItemPool.GetEntry(_random);


                int price = _random.Next(item.minPrice, item.maxPrice + 1);

                GameObject obj = item.item.CreatePhysicalItem().gameObject;
                obj.GetComponent<StoreItemDisplay>()._storeHand = hand;
                obj.GetComponent<StoreItemDisplay>().SetNotPurchased();
                obj.GetComponent<StoreItemDisplay>().OnPurchased += (boughtHand, i) =>
                {
                    Debug.Log("Purchased item " + item.item.name);
                    boughtHand.HideHand();
                    itemsCreated.Remove(i);
                    merchantHands.Remove(boughtHand);
                };
                itemsCreated.Add(obj.GetComponent<StoreItemDisplay>());
                var objBehaviour = obj.transform;
                objBehaviour.GetComponent<Rigidbody>().isKinematic = true;

                GameObject canvasObj = Instantiate(priceCanvasPrefab,
                    priceCanvasSpawn.transform.position,
                    priceCanvasSpawn.rotation
                    );

                PriceCanvas priceCanvas = canvasObj.GetComponent<PriceCanvas>();

                if (priceCanvas != null)
                    priceCanvas.Initialize(price, obj.GetComponent<StoreItemDisplay>());

                StoreItemDisplay display = obj.GetComponentInChildren<StoreItemDisplay>();

                if (display != null)
                    display.Initialize(item, price, priceCanvas);
            }
        }

        private void GenerateCourtesyItem()
        {
            MerchantHand hand = initialMerchantHands[0];
            Transform priceCanvasSpawn = singlePriceSpawnPoint;

            StoreItemData item;
            if (forcedSpawnItem != null && !_forcedSpawn)
            {
                item = forcedSpawnItem;
                _forcedSpawn = true;
            }
            else
                item = initialItemSpawn;

            GameObject obj = item.item.CreatePhysicalItem().gameObject;
            obj.GetComponent<StoreItemDisplay>()._storeHand = hand;
            obj.GetComponent<StoreItemDisplay>().SetNotPurchased();
            obj.GetComponent<StoreItemDisplay>().OnPurchased += (boughtHand, i) =>
            {
                boughtHand.HideHand();
                initialMerchantHands.Remove(boughtHand);
                itemsCreated.Remove(i);
            };
            obj.GetComponent<StoreItemDisplay>().OnSingleItemBought += () =>
            {
                merchantDialogue.ForceEnableInteraction();
                isSingleItemSpawn = false;
            };
            itemsCreated.Add(obj.GetComponent<StoreItemDisplay>());
            obj.GetComponent<Rigidbody>().isKinematic = true;

            GameObject canvasObj = Instantiate(priceCanvasPrefab,
                priceCanvasSpawn.transform.position,
                priceCanvasSpawn.rotation
                );

            PriceCanvas priceCanvas = canvasObj.GetComponent<PriceCanvas>();

            if (priceCanvas != null)
                priceCanvas.Initialize(0, obj.GetComponent<StoreItemDisplay>());

            StoreItemDisplay display = obj.GetComponentInChildren<StoreItemDisplay>();

            if (display != null)
                display.Initialize(item, 0, priceCanvas);

        }

        public bool AreAllItemsGenerated()
        {
            return itemsCreated.Count > 0;
        }

        public void GiveMoney()
        {
            int money = deposit.GetCurrentAmount();
            SpawnBills(MoneyManager.Instance.NumberToBills(money));
        }

        private void SpawnBills(List<ValueTuple<BillItemSo, int>> bills)
        {
            foreach (var tuple in bills)
            {
                for (int i = 0; i < tuple.Item2; i++)
                {
                    var bill = tuple.Item1.CreatePhysicalItem();
                    bill.transform.position = dropPivot.transform.position;
                }
            }
        }

        public void ShowItems()
        {
            if (isSingleItemSpawn)
            {
                foreach (var hand in initialMerchantHands)
                {
                    hand.ShowHand();
                }
                foreach (var item in itemsCreated)
                {
                    item.OnShowPrice?.Invoke();
                }
            }
            else
            {
                foreach (var hand in merchantHands)
                {
                    hand.ShowHand();
                }
                foreach (var item in itemsCreated)
                {
                    item.OnShowPrice?.Invoke();
                }
            }
        }

        public void HideItems()
        {
            if (isSingleItemSpawn)
            {
                foreach (var hand in initialMerchantHands)
                {
                    hand.HideHand();
                }
                foreach (var item in itemsCreated)
                {
                    item.OnHidePrice?.Invoke();
                }
            }
            else
            {
                foreach (var hand in merchantHands)
                {
                    hand.HideHand();
                }

                foreach (var item in itemsCreated)
                {
                    item.OnHidePrice?.Invoke();
                }
            }
        }
    }
}