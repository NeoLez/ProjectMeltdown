using UnityEngine;

namespace Root
{
    public class TriggerAnimationByInteract : InteractableNormalCamera
    {
        Animator _animator;
        [SerializeField] GameObject _visual;
        private void Start()
        {
            _animator = _visual.GetComponent<Animator>();
        }
        public override void Interact()
        {
            Debug.Log("abrir");
            if (_animator.GetBool("Open") == true) { _animator.SetBool("Open", false); }
            else {_animator.SetBool("Open", true); }
        }
    }
}
