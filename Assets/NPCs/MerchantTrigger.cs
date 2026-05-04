using UnityEngine;

    public class MerchantTrigger : MonoBehaviour
    {
    [SerializeField] GameObject _head;
    [SerializeField] GameObject _face;

    Animator _handAnimator;
    Animator _headAnimator;

    void Start()
        {
            _headAnimator = _face.GetComponent<Animator>();
            _handAnimator = _head.GetComponent<Animator>();
        }

        private void OnTriggerEnter(Collider other)
        {
            _handAnimator.Play("rig_Pullback");
            _headAnimator.Play("Armature_001_HeadAppear");
        }
        private void OnTriggerExit(Collider other)
        {
            
        }
        // Update is called once per frame
        void Update()
        {
        
        }
    }

