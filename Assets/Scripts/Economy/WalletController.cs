using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using Root.Managers; 

namespace Root
{
    public class WalletController : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private GameObject moneyText;

        private bool _opened;
        private bool _isAnimating;

        private static readonly int OpenHash = Animator.StringToHash("Open");

        private void OnEnable()
        {
            GameManager.Input.Interaction.Wallet.performed += OnWalletPerformed;
        }

        private void OnDisable()
        {
            GameManager.Input.Interaction.Wallet.performed -= OnWalletPerformed;
        }

        private void OnWalletPerformed(InputAction.CallbackContext ctx)
        {
            if (_isAnimating) return; 
            ToggleWallet();
        }

        private void ToggleWallet()
        {
            _opened = !_opened;
            animator.SetBool(OpenHash, _opened);

            if (!_opened)
                moneyText.SetActive(false);

            StartCoroutine(PlayAnimation()); 
        }

        private IEnumerator PlayAnimation() 
        {
            _isAnimating = true;
            yield return null;
            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
            yield return new WaitForSeconds(state.length);

            if (_opened)
                moneyText.SetActive(true);

            _isAnimating = false;
        }

        public void ShowMoney()
        {
            moneyText.SetActive(true);
        }
    }
}