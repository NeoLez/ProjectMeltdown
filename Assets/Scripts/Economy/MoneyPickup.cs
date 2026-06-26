using UnityEngine;

namespace Root
{
    public class MoneyPickup : InteractableNormalCamera
    {
        [SerializeField] private int amount = 50;
        [SerializeField] private AudioClip _soundPickup; 

        public override void Interact()
        {
            EconomyManager.Instance.AddMoney(amount);
            MoneyFeedback.Instance.GrabbedBill();

            if (_soundPickup != null)
                GameManager.AudioSystem.PlaySoundPositional(_soundPickup, transform.position, GameManager.AudioSystem.VFX);

            Destroy(gameObject);
        }
    }
}