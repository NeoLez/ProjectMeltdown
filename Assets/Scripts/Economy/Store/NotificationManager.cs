using TMPro;
using UnityEngine;
using System.Collections;

namespace Root
{
    public class NotificationManager : MonoBehaviour
    {
        public static NotificationManager Instance;

        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip notificationSound;

        [SerializeField] private GameObject notificationPanel;
        [SerializeField] private TMP_Text notificationText;
        [SerializeField] private float duration = 2f;

        private Coroutine currentNotification;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            notificationPanel.SetActive(false);
        }

        public void ShowNotification(string message, float customDuration = 0)
        {
            if (currentNotification != null)
                StopCoroutine(currentNotification);

            currentNotification = StartCoroutine(NotificationRoutine(message, customDuration));
        }

        private IEnumerator NotificationRoutine(string message, float customDuration = 0)
        {
            if(customDuration == 0) customDuration = duration;

            notificationPanel.SetActive(true);
            notificationText.text = message;

            if (audioSource != null && notificationSound != null)
                GameManager.AudioSystem.PlaySound(notificationSound, GameManager.AudioSystem.VFX);

            yield return new WaitForSeconds(customDuration);

            notificationPanel.SetActive(false);
        }

    }
}