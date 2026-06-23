using Root.Controller;
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

                GameManager.AudioSystem?.PauseAll();

                MouseHandler.RequestControl(CursorLockMode.None, true, this, false);
            }
            else
            {
                GameManager.Input.Movement.Enable();
                GameManager.Input.CameraMovement.Enable();
                GameManager.Input.Interaction.Enable();

                GameManager.AudioSystem?.ResumeAll();

                MouseHandler.RelinquishControl(this);
            }
        }

        public void Resume()
        {
            if (paused)
                TogglePause();
        }

        public void ReturnToMenu()
        {
            GameManager.AudioSystem?.ResumeAll();
            GameManager.Input.Enable();

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            SceneManager.LoadScene("Menu");
        }
    }
}