using UnityEngine;

namespace Root
{
    public class WalletController : MonoBehaviour
    {
        Animator _animator;
        [SerializeField] private GameObject _moneyText;

        private bool _opened;

        private void Start()
        {
            _animator = GetComponentInParent<Animator>();
        }

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

            _animator.SetBool("Open", _opened);

            if (!_opened)
                _moneyText.SetActive(false);
        }

        public void ShowMoney()
        {
            _moneyText.SetActive(true);
        }
    }
}