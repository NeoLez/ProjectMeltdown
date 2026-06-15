using UnityEngine;

namespace Root
{
    public class TriggerAnimationByInteract : InteractableNormalCamera
    {
        Animator _animator;
        void Start()
        {
            _animator = GetComponent<Animator>();
        }
        public override void Interact()
        {
            Debug.Log("abrir");
            if (_animator.GetBool("Open")) { _animator.SetBool("Open", true); }
            else {_animator.SetBool("Open", false); }
        }
    }
}
