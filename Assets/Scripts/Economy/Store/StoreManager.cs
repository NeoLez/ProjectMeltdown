using NUnit.Framework.Interfaces;
using Root.Factories;
using Root.Managers;
using System;
using System.Collections.Generic;
using Timers;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Localization.SmartFormat.Core.Parsing;
using UnityEngine.UIElements;

namespace Root
{
    public class StoreManager : MonoBehaviour
    {
        [SerializeField] private bool isSingleItemSpawn;
        [SerializeField] private MapSection mapSection;

        [Header("Buy")]
        [SerializeField] private StoreItemData forcedSpawnItem;
        [SerializeField] private StoreItemPoolSO storeItemPool;
        [SerializeField] private int minAmountOfItemsToGenerate;
        [SerializeField] private int maxAmountOfItemsToGenerate;

        [Header("UI")]
        [SerializeField] private RectTransform scrollView;
        [SerializeField] private GameObject storeItemCanvasPrefab;

        [Header("Tutorial Courtesy Item")]
        [SerializeField] private StoreItemData initialItemSpawn;

        private List<StoreItemDisplay> itemsCreated = new();
        public bool CanSpawnMultipleItems => !isSingleItemSpawn;

        [Header("Sell")]
        [SerializeField] private SelllDeposit deposit;
        [SerializeField] private Transform dropPivot;
        [SerializeField] private StoreMenu menu;

        private System.Random _random;
        private Inventory _playerInventory;
        private Action OnRegenerateStock;

        private void OnEnable()
        {
            itemsCreated.Clear();
        }

        private void Start()
        {
            _playerInventory = GameManager.Player.GetComponent<Inventory>();

            _random = new System.Random(mapSection.GetMapSectionSeed() + SeedUtils.TextToSeed("Shop"));
            GenerateStoreItems();
        }


        private void GenerateStoreItems()
        {
            if (isSingleItemSpawn)
            {
                GenerateCourtesyItem();
            }
            else
            {
                if (AreAllItemsGenerated()) return;

                GenerateStock();
            }
        }

        private bool _forcedSpawn;
        private void GenerateStock()
        {
            for (int i = 0; i < _random.Next(minAmountOfItemsToGenerate, maxAmountOfItemsToGenerate); i++)
            {
                StoreItemData item;
                if (forcedSpawnItem != null && !_forcedSpawn)
                {
                    item = forcedSpawnItem;
                    _forcedSpawn = true;
                }
                else
                    item = storeItemPool.GetEntry(_random);


                int price = _random.Next(item.minPrice, item.maxPrice + 1);

                GameObject obj = Instantiate(storeItemCanvasPrefab, scrollView.transform);
                obj.GetComponent<StoreItemDisplay>().SetNotPurchased();
                obj.GetComponent<StoreItemDisplay>().OnPurchased += (i, itemData) =>
                {
                    //Debug.Log("Purchased item " + item.item.name);
                    InsertBoughtItem(itemData);
                    //itemsCreated.Remove(i);
                    menu.RefreshWalletAmount();
                    RefreshStoreItems();
                };
                itemsCreated.Add(obj.GetComponent<StoreItemDisplay>());

                StoreItemDisplay display = obj.GetComponentInChildren<StoreItemDisplay>();

                if (display != null)
                    display.Initialize(item, price);
            }
        }

        private void GenerateCourtesyItem()
        {
            StoreItemData item;
            if (forcedSpawnItem != null && !_forcedSpawn)
            {
                item = forcedSpawnItem;
                _forcedSpawn = true;
            }
            else
                item = initialItemSpawn;

            GameObject obj = Instantiate(storeItemCanvasPrefab, scrollView.transform);
            obj.GetComponent<StoreItemDisplay>().SetNotPurchased();
            obj.GetComponent<StoreItemDisplay>().OnPurchased += (i, itemData) =>
            {
                InsertBoughtItem(itemData);
            };
            obj.GetComponent<StoreItemDisplay>().OnSingleItemBought += () =>
            {
                isSingleItemSpawn = false;
                OnRegenerateStock += ClearStoreItems;
            };
            itemsCreated.Add(obj.GetComponent<StoreItemDisplay>());

            StoreItemDisplay display = obj.GetComponentInChildren<StoreItemDisplay>();

            if (display != null)
                display.Initialize(item, 0);

        }

        public void ShowStore()
        {
            if (!isSingleItemSpawn)
            {
                 foreach (var item in itemsCreated)
                 {
                     item.RefreshItems();
                 }
            }
        }

        public void HideStore()
        {
            if (!isSingleItemSpawn)
            {
                OnRegenerateStock?.Invoke();
                OnRegenerateStock -= ClearStoreItems;
            }
        }

        private void RefreshStoreItems()
        {
            foreach (var item in itemsCreated)
            {
                item.RefreshItems();
            }
        }

        private void ClearStoreItems()
        {
            foreach (var item in itemsCreated)
            {
                Destroy(item.gameObject);
            }
            itemsCreated.Clear();

            GenerateStoreItems();
        }

        private ItemSpawnContext CreateItemSpawnContext() {
            return new ItemSpawnContext(mapSection, _random);
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
                    var bill = tuple.Item1.CreatePhysicalItem(CreateItemSpawnContext());
                    bill.transform.position = dropPivot.transform.position;
                }
            }
        }


        public void InsertBoughtItem(StoreItemData item)
        {
            _playerInventory.InsertItem(item.item.CreateState(new ItemSpawnContext(mapSection, _random)));
        }

    }
}