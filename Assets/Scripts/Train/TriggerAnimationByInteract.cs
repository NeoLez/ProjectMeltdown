using System.Collections.Generic;
using UnityEngine;

namespace Root
{
    public class TriggerAnimationByInteract : InteractableNormalCamera
    {
        Animator _animator;
        [SerializeField] GameObject _visual;
        [SerializeField] bool _MultipleColliders = false;
        [SerializeField] List<BoxCollider> colliders;
        private void Start()
        {
            _animator = _visual.GetComponent<Animator>();
        }
        public override void Interact()
        {
            if (_MultipleColliders)
            {
                if (colliders[0].enabled)
                {
                    colliders[0].enabled = false;
                    colliders[1].enabled = true;
                }
                else
                {
                    colliders[0].enabled = true;
                    colliders[1].enabled = false;
                }
            }
            if (_animator.GetBool("Open") == true) { _animator.SetBool("Open", false); }
            else {_animator.SetBool("Open", true); }
        }
    }
}
