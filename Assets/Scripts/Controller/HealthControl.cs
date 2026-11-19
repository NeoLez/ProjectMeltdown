using Root.Managers;
using System;
using System.Collections;
using Timers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
namespace Root
{
    public class HealthControl : MonoBehaviour
    {
        public static event System.Action OnPlayerDied;
        public static bool IsPlayerDead { get; private set; }
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


        [Header("Infection")]
        [SerializeField] private Slider infectionBar;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private float maxInfectionLevel;
        [SerializeField] private float infectionCooldown;
        private bool _canInfect;
        private Timer _infectionTimer = new();

        [SerializeField] private Component[] scriptsToModify;
        [SerializeField] private float infectionSpeed;
        [SerializeField] private Color[] infectionStages;
        [SerializeField] private float[] infectionPercentageStages;
        public event Action<float> OnInfectionIncreased;
        private Coroutine _infectionCoroutine;
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
                LoadingScreen.Instance.LoadScene("Menu");
        }

        private void Start()
        {
            _currentHealth = _maxHealth;
            IsPlayerDead = false;

            GameManager.AudioSystem?.ResumeAll();
            GameManager.Input.Movement.Enable();
            GameManager.Input.CameraMovement.Enable();
            GameManager.Input.Interaction.Interact.Enable();

            infectionBar.image.color = infectionStages[0];
            backgroundImage.color = infectionStages[0];
        }

        void Update()
        {
            if (_regeneration)
            {
                Regen();
                DamageFeedback();
            }

            if (_infectionTimer.IsCompleted())
            {
                _canInfect = true;
                _infectionTimer.Reset(infectionCooldown);
            }
        }

        public void TakeDamage(float dmg)
        {
            if (_muerto) return;

            if (Time.time >= _cooldown)
            {
                CameraShakeManager.Instance.Shake(0.5f, 0.1f);
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
            if (_currentHealth < 100 && _currentHealth >= 60)
            {
                UIFeedback(_dmg1, 1 - _currentHealth * 0.01f);
                UIFeedback(_dmg3, 0f);
                UIFeedback(_dmg2, 0f);
            }
            else if (_currentHealth < 60 && _currentHealth >= 30)
            {
                UIFeedback(_dmg1, 1 - _currentHealth * 0.01f);
                UIFeedback(_dmg2, 0.6f - _currentHealth * 0.01f);
                UIFeedback(_dmg3, 0f);
            }
            else
            {
                UIFeedback(_dmg1, 1 - _currentHealth * 0.01f);
                UIFeedback(_dmg2, 0.6f - _currentHealth * 0.01f);
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

        public void InstantDeath()
        {
            Die();
        }

        private void Die()
        {
            _muerto = true;
            _regeneration = false;
            IsPlayerDead = true;

            GameManager.Input.Movement.Disable();
            GameManager.Input.CameraMovement.Disable();
            GameManager.Input.Interaction.Interact.Disable();
            GameManager.AudioSystem?.PauseAll();

            ui_muerte.SetActive(true);
            OnPlayerDied?.Invoke();
        }


        public void Infect(float amount)
        {
            if (!_canInfect) return;

            infectionBar.value += amount / maxInfectionLevel;
            _canInfect = false;

            OnInfectionIncreased?.Invoke(infectionBar.value);

            ChangeColorIndicator(infectionBar.value);

            if (infectionBar.value == infectionBar.maxValue)
            {
                Die();
            }
        }

        public void StartInfection(MonoBehaviour behaviour)
        {
            _infectionCoroutine = StartCoroutine(GradualInfectionRoutine(behaviour));
        }

        public void StopInfection()
        {
            if (_infectionCoroutine != null)
            {
                StopCoroutine(_infectionCoroutine);
                _infectionCoroutine = null;
            }
        }

        private IEnumerator GradualInfectionRoutine(MonoBehaviour behaviour)
        {
            while (behaviour != null && infectionBar.value < maxInfectionLevel)
            {
                infectionBar.value += (1 / maxInfectionLevel) * infectionSpeed;

                ApplyModifiers();

                ChangeColorIndicator(infectionBar.value);
                if (infectionBar.value >= infectionBar.maxValue)
                {
                    Die();
                    break;
                }
                yield return null;
            }

            StopInfection();
        }

        private void ApplyModifiers()
        {
            for (int i = 0; i < scriptsToModify.Length; i++)
            {
                var component = scriptsToModify[i].GetComponent<InfectableComponent>();
                component.Infect(infectionBar.value);
            }
        }

        private void ChangeColorIndicator(float value)
        {
            if (infectionPercentageStages.Length > 0)
            {
                if (value >= infectionPercentageStages[0] && value <= infectionPercentageStages[1])
                {
                    infectionBar.image.color = infectionStages[1];
                    backgroundImage.color = infectionStages[1];
                }
                else if (value >= infectionPercentageStages[1])
                {
                    infectionBar.image.color = infectionStages[2];
                    backgroundImage.color = infectionStages[2];
                }
                else
                {
                    infectionBar.image.color = infectionStages[0];
                    backgroundImage.color = infectionStages[0];
                }
            }
        }

        public void Cure(float amount)
        {
            //aca chequear si esta al maximo, no usar cualquier item
            StopInfection();
            if (infectionBar.minValue == 0)
            {
                Debug.Log("full vida");
                return;
            }
            infectionBar.value -= amount / maxInfectionLevel;
        }

    }
}