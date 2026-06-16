using UnityEngine;

namespace Root
{
    public class TriggerAnimationByInteract : InteractableNormalCamera
    {
        [SerializeField] Animator _animator;
        public override void Interact()
        {
            Debug.Log("abrir");
            if (_animator.GetBool("Open")) { _animator.SetBool("Open", true); }
            else {_animator.SetBool("Open", false); }
        }
    }
}
