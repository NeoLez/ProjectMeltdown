using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Root {
    public class Wallet : MonoBehaviour {
        public static Wallet Instance;
        private void Awake() {
            Debug.Log("Wallet.Awake");
            Instance = this;
        }
        
        private readonly Dictionary<BillItemSo, int> _billAmounts = new ();

        public void AddBill(BillItemSo billItemSo) {
            if (!_billAmounts.TryAdd(billItemSo, 1)) {
                _billAmounts[billItemSo]++;
            }
        }

        public IReadOnlyDictionary<BillItemSo, int> GetBills() { 
            return _billAmounts;
        }

        public bool RemoveBill(BillItemSo billItemSo) {
            if (!_billAmounts.ContainsKey(billItemSo)) {
                _billAmounts[billItemSo]--;
                return true;
            }

            return false;
        }
    }
}