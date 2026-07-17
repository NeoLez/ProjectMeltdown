using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
namespace Root
{
    public class HealthControl : MonoBehaviour
    {
        [SerializeField] float _maxHealth = 100f;
        [SerializeField] float _currentHealth = 100f;
        bool _regeneration = false;
        float _regenRate = 4f;
        float _cooldown = 0f;
        [SerializeField] float _cooldownDuration = 2f;
        [SerializeField] Image _dmg1; //lo tengo q cambiar por un shadergraph o una lista, perdon Leo jiji
        [SerializeField] Image _dmg2;
        [SerializeField] Image _dmg3;

        [SerializeField] private GameObject ui_muerte;
        private bool _muerto = false;


        private void Awake()
        {
            GameManager.Input.Interaction.Reset.performed += HandleResetPerformed;
        }

        private void OnDestroy()
        {
            GameManager.Input.Interaction.Reset.performed -= HandleResetPerformed;
        }

        private void HandleResetPerformed(InputAction.CallbackContext context)
        {
            if (_muerto)
                SceneManager.LoadScene("Menu");
        }
        private void Start()
        {
            _currentHealth = _maxHealth;

            GameManager.AudioSystem?.ResumeAll();
            GameManager.Input.Movement.Enable();
            GameManager.Input.CameraMovement.Enable();
            GameManager.Input.Interaction.Interact.Enable();
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
            if (_muerto) return;

            if (Time.time >= _cooldown)
            {
                GameManager.CameraController.Shake(0.5f, 0.1f);
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
            if (_currentHealth < 100 && _currentHealth > 60)
            {
                UIFeedback(_dmg1, 1 - _currentHealth * 0.01f);
                UIFeedback(_dmg3, 0f);
                UIFeedback(_dmg2, 0f);
            }
            else if (_currentHealth < 60 && _currentHealth > 30)
            {
                UIFeedback(_dmg2, 0.6f - _currentHealth * 0.01f);
                UIFeedback(_dmg3, 0f);
            }
            else
            {
                UIFeedback(_dmg3, 0.3f - _currentHealth * 0.01f);

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
            _muerto = true;
            _regeneration = false;

            GameManager.Input.Movement.Disable();
            GameManager.Input.CameraMovement.Disable();
            GameManager.Input.Interaction.Interact.Disable();
            GameManager.AudioSystem?.PauseAll();

            ui_muerte.SetActive(true);
        }
    }
}