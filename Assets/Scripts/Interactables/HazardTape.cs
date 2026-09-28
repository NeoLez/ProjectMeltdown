using UnityEngine;

namespace Root
{
    public class HazardTape : InteractableNormalCamera
    {
        [SerializeField] private AudioClip _soundRemove;
        [SerializeField] private GameObject objectToRemove;

        private bool _removed;

        public override void Interact()
        {
            if (_removed) return;
            _removed = true;

            if (_soundRemove != null)
                GameManager.AudioSystem.PlaySoundPositional(_soundRemove, transform.position, GameManager.AudioSystem.VFX);

            Destroy(objectToRemove != null ? objectToRemove : gameObject);
        }
    }
}