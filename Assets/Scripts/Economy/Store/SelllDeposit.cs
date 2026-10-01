using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Root
{
    public class SelllDeposit : MonoBehaviour
    {
        [SerializeField] private StoreManager storeManager;
        [SerializeField] private Inventory inventory;
        [SerializeField] private SellDisplay inventorySellDisplay;

        private int _currentPackageSum;
        private List<InventoryItem> _sellableItems = new();

        public Action OnPriceUpdated;
        public Action OnItemUpdated;

        void Start()
        {
            inventory.OnItemInserted += ItemAdded;
            inventory.OnItemDiscarted += ItemDiscarted;
        }

        public void LoadDisplay()
        {
            inventorySellDisplay.LoadInventory(inventory, true);
            inventorySellDisplay.gameObject.SetActive(true);

            OnItemUpdated?.Invoke();
            OnPriceUpdated?.Invoke();
        }

        public int GetCurrentAmount()
        {
            return _currentPackageSum;
        }

        private void RefreshSumAmount(int amount, bool canDecrement)
        {
            if(canDecrement)
            {
                _currentPackageSum -= amount;
            }
            else
            {
                _currentPackageSum += amount;
            }
            OnPriceUpdated?.Invoke();
        }

        public void ConfirmSell()
        {
            if(_sellableItems.Count > 0)
            {
                _sellableItems.Clear();
                storeManager.GiveMoney();
                _currentPackageSum = 0;
                OnPriceUpdated?.Invoke();
                OnItemUpdated?.Invoke();
                inventorySellDisplay.LoadInventory(inventory, false);
            }
        }

        private void ItemAdded(InventoryItem state)
        {  
            int sellablePrice = 0;

            if(!_sellableItems.Contains(state))
            {
                if(state.itemState is PackageItemState)
                    sellablePrice = ((PackageItemState)state.itemState).price;
                else {
                    var storeItemData = state.itemState.ItemSo.StoreItemData;
                    if (storeItemData == null) {
                        sellablePrice = 0;
                    } else
                        sellablePrice = storeItemData.minPrice;
                }

                _sellableItems.Add(state);
                RefreshSumAmount(sellablePrice, false);
                OnItemUpdated?.Invoke();
            }
        }

        public void ItemDiscarted(InventoryItem state)
        {
            if (_sellableItems.Contains(state)) {
                int sellablePrice = 0;
                if(state.itemState is PackageItemState)
                    sellablePrice = ((PackageItemState)state.itemState).price;
                else {
                    var storeItemData = state.itemState.ItemSo.StoreItemData;
                    if (storeItemData == null) {
                        sellablePrice = 0;
                    } else
                        sellablePrice = storeItemData.minPrice;
                }
                _sellableItems.Remove(state);

                RefreshSumAmount(sellablePrice, true);
                OnItemUpdated?.Invoke();
            }
        }

        public bool AreUnconfirmedItems()
        {
            return _sellableItems.Count > 0;
        }

        private void OnDestroy()
        {
            inventory.OnItemInserted -= ItemAdded;
            inventory.OnItemDiscarted -= ItemDiscarted;
        }

    }
}
