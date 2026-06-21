using UnityEngine;
using UnityEngine.SceneManagement;

namespace Root
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] private GameObject pausePanel;

        private bool paused;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePause();
            }
        }

        public void TogglePause()
        {
            paused = !paused;

            pausePanel.SetActive(paused);

            if (paused)
            {
                GameManager.Input.Movement.Disable();
                GameManager.Input.CameraMovement.Disable();
                GameManager.Input.Interaction.Disable();

                AudioListener.pause = true;

                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                GameManager.Input.Movement.Enable();
                GameManager.Input.CameraMovement.Enable();
                GameManager.Input.Interaction.Enable();

                AudioListener.pause = false;

                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        public void Resume()
        {
            if (paused)
                TogglePause();
        }

        public void ReturnToMenu()
        {
            GameManager.Input.Enable();

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            SceneManager.LoadScene("Menu");
        }
    }
}