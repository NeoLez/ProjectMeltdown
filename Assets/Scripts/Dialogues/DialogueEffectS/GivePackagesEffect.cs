using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Root
{
    [CreateAssetMenu(menuName = "Dialogue/Advanced/Effects/Give Packages")]
    public class GivePackagesEffect : DialogueEffectSO
    {
        List<ItemState> _itemsToErease = new();
        List<PackageItemGenerationDataSo> _packageItemData = new();
        public override void Execute(DialogueExecutionContext context)
        {
            PackageRequestType types = context.NPC.gameObject.GetComponent<NpcToll>().RequestType;
            ItemGroup requestPackages = context.NPC.gameObject.GetComponent<NpcToll>().PackagesToRequest;
            _packageItemData = context.NPC.gameObject.GetComponent<NpcToll>().Packages;

            Inventory inventory = context.Instigator.gameObject.GetComponent<Inventory>();

            CheckRequestType(types, requestPackages, inventory);
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
                    Debug.Log("Elegi un tipo");
                    break;
            }
        }

        private void DeliverPackagesAmount(ItemGroup requestPackages, Inventory inventory)
        {
            _itemsToErease.Clear();

            if (inventory.ContainsItemType(requestPackages))
            {
                foreach (var item in inventory.GetItems())
                {
                    if (!_itemsToErease.Contains(item.itemState) && _itemsToErease.Count < requestPackages.Items.Count)
                    {
                        _itemsToErease.Add(item.itemState);
                        //Debug.Log("entre" + item.itemState.ItemSo.name);
                    }
                }
            }

            if (_itemsToErease.Count >= requestPackages.Items.Count)
            {
                RemovePackagesFromInventory(inventory, _itemsToErease);
            }
            else
            {
                Debug.Log("No hay paquetes suficientes para remover");
                return;
            }

        }

        private void DeliverPackagesType(List<PackageItemGenerationDataSo> requestPackages, Inventory inventory)
        {
            List<PackageItemGenerationDataSo> currentRequest = new(requestPackages);
            _itemsToErease.Clear();

            foreach (var item in inventory.GetItems())
            {
                if (item.itemState is PackageItemState itemState)
                {
                    for (int i = 0; i < requestPackages.Count; i++)
                    {
                        if (itemState.typeOfPackage == requestPackages[i].TypeOfPackage)
                        {
                            if (!_itemsToErease.Contains(itemState))
                            {
                                _itemsToErease.Add(itemState);
                                requestPackages.RemoveAt(i);

                                break;
                            }
                        }

                    }
                }
            }

            if (_itemsToErease.Count >= currentRequest.Count)
            {
                RemovePackagesFromInventory(inventory, _itemsToErease);
            }
            else
            {
                Debug.Log("Los paquetes no coinciden");
                return;
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
        }
    }
}
