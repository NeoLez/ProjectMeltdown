using UnityEngine;
using System.Collections;

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
            animator.SetBool(OpenHash, _opened);

            if (_opened)
                StartCoroutine(ShowMoneyAfterAnimation());
            else
                moneyText.SetActive(false);
        }

        private IEnumerator ShowMoneyAfterAnimation()
        {
            // Espera que empiece la transición
            yield return null;

            // Espera que termine la animación de apertura
            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
            yield return new WaitForSeconds(state.length);

            moneyText.SetActive(true);
        }

        public void ShowMoney()
        {
            moneyText.SetActive(true);
        }
    }
}