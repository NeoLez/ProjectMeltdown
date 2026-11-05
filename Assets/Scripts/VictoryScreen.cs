using Root.Controller;
using Root.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Root
{
    public class VictoryScreen : MonoBehaviour
    {
        [SerializeField] private Canvas victoryPanel;
        [SerializeField] private Button returnToMenu;

        private LoadingScreen _loadingScreen;

        private void Awake()
        {
            returnToMenu.onClick.AddListener(ReturnToMainMenu);
        }

        private void Start()
        {
            _loadingScreen = LoadingScreen.Instance;
            victoryPanel.enabled = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            /*if(loadingScreen != null)
            {
                loadingScreen.LoadScene("Train 1");
                GameManager.seed = new System.Random(GameManager.seed).Next();
            }*/

            if (victoryPanel != null)
            {
                MouseHandler.RequestControl(CursorLockMode.None, true, this, false);
                victoryPanel.enabled = true;
                FreezeGameplay();
            }
        }

        private void ReturnToMainMenu()
        {
            victoryPanel.enabled = false;
            MouseHandler.RelinquishControl(this);
            _loadingScreen.LoadScene("Menu");
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
    }
}
