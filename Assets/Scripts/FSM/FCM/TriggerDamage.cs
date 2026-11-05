using Root;
using UnityEngine;

public class TriggerDamage : MonoBehaviour
{
    [SerializeField] private ZombieStateMachine _state;
    private HealthControl _playerHealth;

    private void Start()
    {
        _playerHealth = GameManager.Player.gameObject.GetComponent<HealthControl>();
    }

    public void DamagePlayer()
    {
        _playerHealth.TakeDamage(_state.damage);
    }

}
