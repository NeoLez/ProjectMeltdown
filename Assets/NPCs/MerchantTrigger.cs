using UnityEngine;

    public class MerchantTrigger : MonoBehaviour
    {
    [SerializeField] GameObject _face;

    Animator _anim;

    void Start()
        {
            _anim = _face.GetComponent<Animator>();
        }

        private void OnTriggerEnter(Collider other)
        {
            _anim.SetBool ("Appear", true);
        }
        private void OnTriggerExit(Collider other)
        {
            _anim.SetBool("Appear", false);
        }
    }

