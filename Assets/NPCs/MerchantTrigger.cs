using UnityEngine;

    public class MerchantTrigger : MonoBehaviour
    {
    [SerializeField] GameObject _face;

    Animator _headAnimator;

    void Start()
        {
            _headAnimator = _face.GetComponent<Animator>();
        }

        private void OnTriggerEnter(Collider other)
        {
            _headAnimator.Play("Appear");
        }
        private void OnTriggerExit(Collider other)
        {
        _headAnimator.Play("Leave");
    }
    }

