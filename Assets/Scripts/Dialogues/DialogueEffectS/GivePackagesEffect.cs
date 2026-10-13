using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Root
{
    [CreateAssetMenu(menuName = "Dialogue/Advanced/Effects/Give Packages")]
    public class GivePackagesEffect : DialogueEffectSO
    {
        List<ItemState> _itemSoList = new();
        public override void Execute(DialogueExecutionContext context)
        {
            PackageRequestType types = context.NPC.gameObject.GetComponent<NpcToll>().RequestType;
            ItemGroup requestPackages = context.NPC.gameObject.GetComponent<NpcToll>().PackagesToRequest;
            List<PackageItemGenerationDataSo> requesPackages2 = context.NPC.gameObject.GetComponent<NpcToll>().Packages;

            Inventory inventory = context.Instigator.gameObject.GetComponent<Inventory>();

            _itemSoList.Clear();

            CheckRequestType(types, requestPackages, inventory);

        /*    if (inventory.ContainsItemType(requesPackages))
            {
                foreach (var item in inventory.GetItems())
                {
                    var itemState = item.itemState as PackageItemState;
                    foreach (var item2 in requesPackages.Items)
                    {
                        var itemState2 = item2.Test as PackageItemState;

                        Debug.Log(itemState2);
                        if (itemState.typeOfPackage == itemState2.typeOfPackage)
                        {
                            if (!_itemSoList.Contains(item.itemState) && _itemSoList.Count < requesPackages.Items.Count)
                            {
                                _itemSoList.Add(item.itemState);
                                Debug.Log("entre" + itemState.typeOfPackage);

                                break;
                            }
                                
                        }
                    }
                    
                }
            }

            if (_itemSoList.Count == requesPackages.Items.Count)
            {
                Debug.Log("Removi" + _itemSoList.Count);
                RemovePackagesFromInventory(inventory, _itemSoList);
            }
            else
            {
                Debug.Log("No hay paquetes suficientes para remover");
            }*/
        }

        private void CheckRequestType(PackageRequestType type, ItemGroup requestPackages, Inventory inventory)
        {
            switch (type)
            {
                case PackageRequestType.Amount:
                    Algo(requestPackages, inventory);
                    break;
                case PackageRequestType.TypeOfPackages:
                    break;
                default:
                    Debug.Log("Elegi un tipo");
                    break;
            }
        }

        private void Algo(ItemGroup requestPackages, Inventory inventory)
        {
            _itemSoList.Clear();

            if (inventory.ContainsItemType(requestPackages))
            {
                foreach (var item in inventory.GetItems())
                {
                    if (!_itemSoList.Contains(item.itemState) && _itemSoList.Count < requestPackages.Items.Count)
                    {
                        _itemSoList.Add(item.itemState);
                        //Debug.Log("entre" + item.itemState.ItemSo.name);
                    }
                }
            }

            if (_itemSoList.Count < requestPackages.Items.Count) //funciona para el orto
            {
                Debug.Log("No hay paquetes suficientes para remover");
                return;
            }

            RemovePackagesFromInventory(inventory, _itemSoList);
        }

        private void Algo2(List<PackageItemGenerationDataSo> requestPackages, Inventory inventory)
        {
            foreach (var item in inventory.GetItems())
            {
                var itemState = item.itemState as PackageItemState;
                foreach (var item2 in requestPackages)
                {
                    if (itemState.typeOfPackage == item2.TypeOfPackage)
                    {
                        if (!_itemSoList.Contains(item.itemState) && _itemSoList.Count < requestPackages.Count)
                        {
                            _itemSoList.Add(item.itemState);
                            Debug.Log("entre" + itemState.typeOfPackage);

                            break;
                        }

                    }
                }
            }
            

            if (_itemSoList.Count == requestPackages.Count)
            {
                Debug.Log("Removi" + _itemSoList.Count);
                RemovePackagesFromInventory(inventory, _itemSoList);
            }
            else
            {
                Debug.Log("No hay paquetes suficientes para remover");
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
            _itemSoList.Clear();
        }
    }
}
