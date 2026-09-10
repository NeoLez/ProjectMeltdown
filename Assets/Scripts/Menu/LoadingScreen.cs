using System.Collections;
using TMPro;
using UnityEngine.Localization.Settings; 
using UnityEngine;
using UnityEngine.SceneManagement;
using Root.Managers;

namespace Root
{
    public class LoadingScreen : MonoBehaviour
    {
        public static LoadingScreen Instance { get; private set; }
        [SerializeField] private GameObject loadingPanel;
        [SerializeField] private TMP_Text loadingText;
        [SerializeField] private float minDisplayTime = 2f; 
        private bool loading;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            loadingPanel.SetActive(false);
        }

        public void LoadScene(string sceneName)
        {
            if (loading)
                return;

            FreezeGameplay();
            StartCoroutine(LoadSceneAsync(sceneName));
        }

        private IEnumerator LoadSceneAsync(string sceneName)
        {
            loading = true;
            loadingPanel.SetActive(true);
            StartCoroutine(LoadingTextAnimation());
            yield return null;
            float elapsed = 0f; 
            AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
            op.allowSceneActivation = false;

            while (!op.isDone)
            {
                elapsed += Time.deltaTime; 

                if (op.progress >= 0.9f && elapsed >= minDisplayTime) 
                {
                    op.allowSceneActivation = true;
                }

                yield return null;
            }

            loading = false;
            loadingPanel.SetActive(false);
            UnfreezeGameplay();
        }

        private void FreezeGameplay()
        {
            if (UIManager.Instance != null) UIManager.Instance.CloseMenu(UIManager.UITypes.PauseMenu);
            GameManager.Input.Movement.Disable();
            GameManager.Input.CameraMovement.Disable();
            GameManager.Input.Interaction.Disable();
            GameManager.Input.Inventory.Disable();
            GameManager.Input.Menu.Disable();
            if (GameManager.AudioSystem != null) GameManager.AudioSystem.PauseAll();
        }

        private void UnfreezeGameplay()
        {
            GameManager.Input.Movement.Enable();
            GameManager.Input.CameraMovement.Enable();
            GameManager.Input.Interaction.Enable();
            GameManager.Input.Inventory.Enable();
            GameManager.Input.Menu.Enable();
            GameManager.AudioSystem?.ResumeAll();
        }

        private IEnumerator LoadingTextAnimation()
        {
            int dots = 0;
            string baseText = LocalizationSettings.StringDatabase.GetLocalizedString("UI", "menu_loading");

            while (loading)
            {
                loadingText.text = baseText; 

                for (int i = 0; i < dots; i++)
                {
                    loadingText.text += ".";
                }

                dots++;

                if (dots > 3)
                    dots = 0;

                yield return new WaitForSeconds(0.4f);
            }
        }
    }
}