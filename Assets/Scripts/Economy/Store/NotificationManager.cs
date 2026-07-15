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

        public void ShowNotification(string message)
        {
            if (currentNotification != null)
                StopCoroutine(currentNotification);

            currentNotification = StartCoroutine(NotificationRoutine(message));
        }

        private IEnumerator NotificationRoutine(string message)
        {
            notificationPanel.SetActive(true);
            notificationText.text = message;

            if (audioSource != null && notificationSound != null)
                //audioSource.PlayOneShot(notificationSound);
                GameManager.AudioSystem.PlaySound(notificationSound, GameManager.AudioSystem.VFX);

            yield return new WaitForSeconds(duration);

            notificationPanel.SetActive(false);
        }
    }
}