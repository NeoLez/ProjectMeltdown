using UnityEngine;

namespace Root
{
    public class WalletController : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private GameObject moneyText;

        private bool _opened;

        private static readonly int OpenHash =
            Animator.StringToHash("Open");

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                ToggleWallet();
            }
        }

        private void ToggleWallet()
        {
            _opened = !_opened;

            if (_opened)
                Debug.Log("Abriendo billetera");
            else
                Debug.Log("Cerrando billetera");

            animator.SetBool(OpenHash, _opened);

            if (!_opened)
                moneyText.SetActive(false);
        }

        public void ShowMoney()
        {
            moneyText.SetActive(true);
        }
    }
}