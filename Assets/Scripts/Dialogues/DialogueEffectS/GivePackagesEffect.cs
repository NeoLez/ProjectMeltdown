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
        List<PackageItemGenerationDataSo> packageItemGenerationDataSos = new();
        public override void Execute(DialogueExecutionContext context)
        {
            PackageRequestType types = context.NPC.gameObject.GetComponent<NpcToll>().RequestType;
            ItemGroup requestPackages = context.NPC.gameObject.GetComponent<NpcToll>().PackagesToRequest;
            packageItemGenerationDataSos = context.NPC.gameObject.GetComponent<NpcToll>().Packages;

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
                    DeliverPackagesAmount(requestPackages, inventory);
                    break;
                case PackageRequestType.TypeOfPackages:
                    DeliverPackagesType(packageItemGenerationDataSos, inventory);
                    break;
                default:
                    Debug.Log("Elegi un tipo");
                    break;
            }
        }

        private void DeliverPackagesAmount(ItemGroup requestPackages, Inventory inventory)
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

            if (_itemSoList.Count >= requestPackages.Items.Count)
            {
                RemovePackagesFromInventory(inventory, _itemSoList);
            }
            else
            {
                Debug.Log("No hay paquetes suficientes para remover");
                return;
            }

        }

        private void DeliverPackagesType(List<PackageItemGenerationDataSo> requestPackages, Inventory inventory)
        {
            _itemSoList.Clear();

            foreach (var item in inventory.GetItems())
            {
                //var itemState = item.itemState as PackageItemState;

                if (item.itemState is PackageItemState itemState)
                {
                    for (int i = 0; i < _itemSoList.Count; i++)
                    {
                        if (itemState.typeOfPackage == requestPackages[i].TypeOfPackage)
                        {
                            if (!_itemSoList.Contains(itemState))
                            {
                                _itemSoList.Add(itemState);
                                requestPackages.RemoveAt(i);
                                Debug.Log(requestPackages.Count);

                                break;
                            }
                        }

                    }
                }
            }
               /* foreach (var item2 in requestPackages)
                {
                    if (!requestPackages.Contains(item2)) break;
                    if (itemState.typeOfPackage == item2.TypeOfPackage)
                    {
                        if (!_itemSoList.Contains(item.itemState) && _itemSoList.Count < requestPackages.Count)
                        {
                            _itemSoList.Add(item.itemState);
                            requestPackages.Remove(item2); //error
                            //Debug.Log("entre" + itemState.typeOfPackage);
                            Debug.Log(requestPackages.Count);
                        }

                    }
                }*/
            


            if (_itemSoList.Count >= requestPackages.Count)
            {
                RemovePackagesFromInventory(inventory, _itemSoList);
            }
            else
            {
                Debug.Log("No hay paquetes suficientes para remover");
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
            _itemSoList.Clear();
        }
    }
}
