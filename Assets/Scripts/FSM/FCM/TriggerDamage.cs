using Root;
using UnityEngine;

public class TriggerDamage : MonoBehaviour
{
    [SerializeField] private ZombieStateMachine stateMachine;
    private HealthControl _playerHealth;

    private void Start()
    {
        _playerHealth = GameManager.Player.gameObject.GetComponent<HealthControl>();
    }

    public void DamagePlayer()
    {
        _playerHealth.TakeDamage(stateMachine.damage);
    }

}
