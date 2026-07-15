using System.Collections.Generic;
using UnityEngine;

namespace Root
{
    public class TriggerAnimationByInteract : InteractableNormalCamera
    {
        Animator _animator;
        [SerializeField] GameObject _visual;
        [SerializeField] Animator _pivot;
        [SerializeField] bool _multipleColliders = false;
        [SerializeField] BoxCollider _externalCollider;
        [SerializeField] List<BoxCollider> colliders;
        [SerializeField] private AudioClip sound;
        private void Start()
        {
            _animator = _visual.GetComponent<Animator>();
        }
        public override void Interact()
        {
            if (_multipleColliders)
            {
                Colliders();
            }
            if (_animator.GetBool("Open") == true) { _animator.SetBool("Open", false); _pivot.SetBool("Open", false);}
            else {_animator.SetBool("Open", true); _pivot.SetBool("Open", true);}
            //if (sound != null) AudioSource.PlayClipAtPoint(sound, transform.position);
            if (sound != null) GameManager.AudioSystem.PlaySoundPositional(sound, transform.position, GameManager.AudioSystem.VFX);
            if (_externalCollider != null) 
            {
                if (_externalCollider.enabled) { _externalCollider.enabled = false; }
                else { _externalCollider.enabled = true; }
            }
        }

        private void Colliders()
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
    }
}
