using System;
using System.Collections.Generic;
using UnityEngine;

namespace Root.Managers {
    [CreateAssetMenu(menuName = "Managers/Money Manager")]
    public class MoneyManager : ScriptableObject {
        private static MoneyManager _instance;
        [SerializeField] private List<BillItemSo> bills;

        public static MoneyManager Instance {
            get {
                if (_instance == null) {
                    _instance = Resources.Load<MoneyManager>("Managers/MoneyManager");

                    if (_instance == null) {
                        Debug.LogError("MoneyManager Asset missing. Create one at 'Resources/Managers/MoneyManager'.");
                    }
                }
                return _instance;
            }
        }

        public List<ValueTuple<BillItemSo, int>> NumberToBills(int moneyAmount) {
            List<ValueTuple<BillItemSo, int>> result = new();

            Debug.Log(moneyAmount);
            
            foreach (BillItemSo bill in bills) {
                var currentBillAmount = moneyAmount / bill.BillDenomination;
                Debug.Log(currentBillAmount + " " + bill.BillDenomination);
                if (currentBillAmount == 0) continue;
                
                moneyAmount -= currentBillAmount * bill.BillDenomination;
                result.Add(new ValueTuple<BillItemSo, int>(bill, currentBillAmount));
            }
            
            if(moneyAmount != 0) Debug.LogError("Re troll", this);
            
            return result;
        }

        public int BillsToNumber(List<BillItem> bills) {
            int result = 0;

            foreach (BillItem bill in bills) {
                result += bill.ItemSo.BillDenomination;
            }
            
            return result;
        }

        private void OnValidate() {
            bills.Sort((bill1, bill2) => {
                if (bill1.BillDenomination > bill2.BillDenomination)
                    return -1;
                if (bill1.BillDenomination < bill2.BillDenomination)
                    return 1;
                Debug.LogError("Something wrong with the bills", this);
                return 0;
            });
        }
    }
}