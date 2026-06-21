using UnityEngine;
using UnityEngine.UI;
namespace Root
{
    public class HealthControl : MonoBehaviour
    {
        [SerializeField]  float _maxHealth = 100f;
        [SerializeField] float _currentHealth = 100f;
        bool _regeneration = false;
        float _regenRate = 4f;
        float _cooldown = 0f;
        [SerializeField] float _cooldownDuration = 2f;
        [SerializeField] Image _dmg1; //lo tengo q cambiar por un shadergraph o una lista, perdon Leo jiji
        [SerializeField] Image _dmg2;
        [SerializeField] Image _dmg3;


        private void Start()
        {
            _currentHealth = _maxHealth;
        }

        void Update()
        {
            if (_regeneration)
            {
                Regen();
                DamageFeedback();
            }
        }

        public void TakeDamage(float dmg)
        {
            if (Time.time >= _cooldown)
            {
                _currentHealth -= dmg;
                _currentHealth = Mathf.Clamp(_currentHealth, 0f, _maxHealth);
                _regeneration = true;
                if (_currentHealth <= 0f)
                {
                    Die();
                }
                _cooldown = Time.time + _cooldownDuration;
            }
        }

        private void DamageFeedback()
        {
            if(_currentHealth < 100 && _currentHealth > 60)
            {
                UIFeedback(_dmg1, 1- _currentHealth*0.01f );
                UIFeedback(_dmg3, 0f);
                UIFeedback(_dmg2, 0f);
            }
            else if(_currentHealth < 60 && _currentHealth > 30)
            {
                UIFeedback(_dmg2, 0.6f - _currentHealth * 0.01f );
                UIFeedback(_dmg3, 0f);
            }
            else
            {
                UIFeedback(_dmg3, 0.3f - _currentHealth * 0.01f );

            }
        }

        
        private void Regen()
        {
            Heal(_regenRate * Time.deltaTime);
        }
        public void Heal(float healAmount)
        {
            if (_currentHealth == _maxHealth)
            {

                UIFeedback(_dmg3, 0f);
                UIFeedback(_dmg2, 0f);
                UIFeedback(_dmg1, 0f);
                _regeneration = false; return;
                
            }
            _currentHealth += healAmount;
            _currentHealth = Mathf.Clamp(_currentHealth, 0f, _maxHealth);
        }
        private void UIFeedback(Image Image, float alpha)
        {
            Color temp = Image.color;
            temp.a = alpha;
            Image.color = temp;
        }
        private void Die()
        {
            Debug.Log("MUERTEE");
        }
    }
}
