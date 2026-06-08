using Root;
using UnityEngine;

    public class MerchantTrigger : MonoBehaviour
    {
    [SerializeField] GameObject _face;
    [SerializeField] StoreManager _storeManager;

    Animator _anim;

    void Start()
        {
            _anim = _face.GetComponent<Animator>();
        }

        private void OnTriggerEnter(Collider other) {
            if (other.gameObject != GameManager.Player.gameObject) return;
            _anim.SetBool ("Appear", true); 
            Invoke(nameof(DelayedShow), 1f);
        }
        private void OnTriggerExit(Collider other) {
            if (other.gameObject != GameManager.Player.gameObject) return;
            _anim.SetBool("Appear", false);
            _storeManager.HideItems();
        }

        public void DelayedShow() {
            _storeManager.ShowItems();
        }
    }

