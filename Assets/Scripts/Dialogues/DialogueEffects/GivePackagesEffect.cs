using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Root
{
    [CreateAssetMenu(menuName = "Dialogue/Advanced/Effects/Give Packages")]
    public class GivePackagesEffect : DialogueEffectSO
    {
        private List<ItemState> _itemsToErease = new();
        private List<PackageItemGenerationDataSo> _packageItemData = new();
        private PackageRequestType _types;
        private ItemGroup _requestPackages;

        public override void Execute(DialogueExecutionContext context)
        {
            Inventory inventory = context.Instigator.gameObject.GetComponent<Inventory>();

            CheckRequestType(_types, _requestPackages, inventory);

            OnEffectExcuted?.Invoke();
        }

        private void CheckRequestType(PackageRequestType type, ItemGroup requestPackages, Inventory inventory)
        {
            switch (type)
            {
                case PackageRequestType.Amount:
                    DeliverPackagesAmount(requestPackages, inventory);
                    break;
                case PackageRequestType.TypeOfPackages:
                    DeliverPackagesType(_packageItemData, inventory);
                    break;
                default:
                    Debug.LogWarning($"Please choose a type of PackageRequest for {name}");
                    break;
            }
        }

        private void DeliverPackagesAmount(ItemGroup requestPackages, Inventory inventory)
        {
            if (_itemsToErease.Count >= requestPackages.Items.Count)
            {
                RemovePackagesFromInventory(inventory, _itemsToErease);
            }

        }

        private void DeliverPackagesType(List<PackageItemGenerationDataSo> requestPackages, Inventory inventory)
        {
            List<PackageItemGenerationDataSo> currentRequest = new(requestPackages);

            if (_itemsToErease.Count >= currentRequest.Count)
            {
                RemovePackagesFromInventory(inventory, _itemsToErease);
            }
        }

        private void RemovePackagesFromInventory(Inventory inventory, List<ItemState> items)
        {
            foreach (var item in inventory.GetItems())
            {
                foreach (var itemSo in items)
                {
                    var itemState = itemSo as PackageItemState;
                    if (item.itemState == itemState)
                    {
                        inventory.RemoveItem(item);
                    }
                }
            }
        }


        private void OnDestroy()
        {
            _itemsToErease.Clear();
            _packageItemData.Clear();
        }

        public override bool CanExecute(DialogueExecutionContext context = null)
        {
            Inventory inventory = context.Instigator.gameObject.GetComponent<Inventory>();
            _requestPackages = context.NPC.gameObject.GetComponent<NpcToll>().PackagesToRequest;
            _packageItemData = context.NPC.gameObject.GetComponent<NpcToll>().Packages;
            _types = context.NPC.gameObject.GetComponent<NpcToll>().RequestType;

            bool canActivateEffect =false;
            _itemsToErease.Clear();

            switch (_types)
            {
                case PackageRequestType.Amount:
                    if (inventory.ContainsItemType(_requestPackages))
                    {
                        foreach (var item in inventory.GetItems())
                        {
                            if (!_itemsToErease.Contains(item.itemState) && _itemsToErease.Count < _requestPackages.Items.Count)
                            {
                                _itemsToErease.Add(item.itemState);
                            }
                        }
                    }
                    canActivateEffect = _itemsToErease.Count >= _requestPackages.Items.Count; 
                    break;

                case PackageRequestType.TypeOfPackages:
                    List<PackageItemGenerationDataSo> currentRequest = new(_packageItemData);

                    foreach (var item in inventory.GetItems())
                    {
                        if (item.itemState is PackageItemState itemState)
                        {
                            for (int i = 0; i < _packageItemData.Count; i++)
                            {
                                if (itemState.typeOfPackage == _packageItemData[i].TypeOfPackage)
                                {
                                    if (!_itemsToErease.Contains(itemState))
                                    {
                                        _itemsToErease.Add(itemState);
                                        _packageItemData.RemoveAt(i);
                                    }
                                }

                            }
                        }
                    }
                    canActivateEffect = _itemsToErease.Count >= currentRequest.Count;
                    break;

                default:
                    Debug.LogWarning($"Please choose a type of PackageRequest for {name}");
                    break;
            }

            return canActivateEffect;
        }
    }
}
