using Timers;
using UnityEngine;

namespace Root
{
    public class HurtPlayerOnTouch : MonoBehaviour
    {
        [SerializeField] private float damage;
        [SerializeField] private float damageCooldown;
        private Timer _damageTimer = new();
        private bool _touchingPlayer;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == GameManager.Player.gameObject)
                _touchingPlayer = true;   
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject == GameManager.Player.gameObject)
                _touchingPlayer = false;
        }

        private void Update()
        {
            if (_touchingPlayer && _damageTimer.IsCompleted())
            {
                _damageTimer.Reset(damageCooldown);
                GameManager.Player.GetComponent<HealthControl>().TakeDamage(damage);
            }
        }
    }
}