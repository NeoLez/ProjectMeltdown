using System;
using UnityEngine;

namespace Root
{
    public class EconomyManager : MonoBehaviour
    {
        public static EconomyManager Instance;

        [SerializeField] private int currentMoney;

        public event Action<int> OnMoneyChanged;

        private void Awake()
        {
            Instance = this;
        }

        public void AddMoney(int amount)
        {
            currentMoney += amount;

            Debug.Log($"Dinero actual: ${currentMoney}");

            OnMoneyChanged?.Invoke(currentMoney);
        }

        public bool SpendMoney(int amount)
        {
            if (currentMoney < amount)
            {
                Debug.Log("Dinero insuficiente");
                return false;
            }

            currentMoney -= amount;

            Debug.Log($"Dinero actual: ${currentMoney}");

            OnMoneyChanged?.Invoke(currentMoney);

            return true;
        }

        public int GetMoney()
        {
            return currentMoney;
        }
    }
}