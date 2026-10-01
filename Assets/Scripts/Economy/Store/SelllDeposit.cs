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
            var package = state.itemState as PackageItemState;

            if (state.itemState != package) return;

            if(!_sellableItems.Contains(state))
            {
                _sellableItems.Add(state);

                sellablePrice = package.price;
                RefreshSumAmount(sellablePrice, false);
                OnItemUpdated?.Invoke();
            }
        }

        public void ItemDiscarted(InventoryItem state)
        {
            var package = state.itemState as PackageItemState;
            if (_sellableItems.Contains(state))
            {
                _sellableItems.Remove(state);

                RefreshSumAmount(package.price, true);
                OnItemUpdated?.Invoke();
            }
        }

        public bool AreUnconfirmedItems()
        {
            return _sellableItems.Count > 0;
        }

    }
}
