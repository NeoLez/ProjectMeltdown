using System;
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
        public bool IsOpened => _opened;
        private bool _isAnimating;
        public bool IsAnimating => _isAnimating;
        public static bool CanInteract => GameManager.Wallet == null || (!GameManager.Wallet.IsOpened && !GameManager.Wallet.IsAnimating);


        private void Awake()
        {
            GameManager.Wallet = this;
            animator.Play("Wallet_Close", 0, 1f);
        }

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
            if (!_opened && GameManager.ItemHolder != null && GameManager.ItemHolder.HasItem) return;
            ToggleWallet();
        }

        public void ToggleWallet()
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