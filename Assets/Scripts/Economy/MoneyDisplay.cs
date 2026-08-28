using TMPro;
using UnityEngine;

namespace Root
{
    public class MoneyDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text label;
        [SerializeField] private string format = "${0}";

        private void Start()
        {
            EconomyManager.Instance.OnMoneyChanged += UpdateDisplay;
            UpdateDisplay(EconomyManager.Instance.GetMoney());
        }

        private void OnDestroy()
        {
            EconomyManager.Instance.OnMoneyChanged -= UpdateDisplay;
        }

        private void UpdateDisplay(int amount)
        {
            label.text = string.Format(format, amount);
        }
    }
}