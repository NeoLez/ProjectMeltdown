using System;
using System.Collections;
using System.Text;
using TMPro;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Root
{
    public class PackageObjectivesUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text texts;
        [SerializeField] private Canvas ui;
        [SerializeField] private GameObject objectivePanel;

        [SerializeField] private Image newObjectiveNotifier;
        [SerializeField] private float blinkDuration = 1.5f;
        [SerializeField] private float blinkSpeed = 10.0f;

        private PlayerInputActions _input;

        private MissionObjectiveSO _activeMission;
        private void Awake()
        {
            _input = GameManager.Input;
            _input.Interaction.Enable();
            _input.Interaction.Objectives.performed += OpenClosePanel;
        }
        private void Start()
        {
            ChangeCanvas(false);
            newObjectiveNotifier.enabled = false;
        }


        public void ActivateNotification()
        {
            StartCoroutine(EnableNotification());
        }

        private IEnumerator EnableNotification()
        {
            newObjectiveNotifier.enabled = true;

            if (newObjectiveNotifier == null) yield break;

            Color originalColor = newObjectiveNotifier.color;
            float elapsedTime = 0f;

            while (elapsedTime < blinkDuration)
            {
                elapsedTime += Time.deltaTime;

                float newAlpha = Mathf.PingPong(elapsedTime * blinkSpeed, 1.0f);

                newObjectiveNotifier.color = new Color(originalColor.r, originalColor.g, originalColor.b, newAlpha);

                yield return null;
            }

            newObjectiveNotifier.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1.0f);
            newObjectiveNotifier.enabled = false;
        }


        public void SetNewObjective(MissionObjectiveSO mission)
        {
            _activeMission = mission;

            texts.text = string.Format(mission.Destination, mission.AmountOfPackages);
        }

        public void ClearCurrentObjective()
        {
            texts.text = "";
        }

        public void ChangeCanvas(bool enable)
        {
            objectivePanel.SetActive(enable);
        }

        bool _openPanel;
        private void OpenClosePanel(InputAction.CallbackContext _)
        {
            if (!_openPanel)
            {
                _openPanel = true;
            }
            else
            {
                _openPanel = false;
            }

            ChangeCanvas(_openPanel);
        }

    }
}
