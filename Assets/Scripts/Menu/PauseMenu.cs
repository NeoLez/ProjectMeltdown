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
                Debug.Log("Pause Menu Locked Controls");
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

        public void SliderMasterVolume(float value)
        {
            GameManager.AudioSystem.GeneralMixer.SetFloat("MasterVolume", Mathf.Log10(value) * 20f);
        }

        public void SliderMusicVolume(float value)
        {
            GameManager.AudioSystem.GeneralMixer.SetFloat("MusicVolume", Mathf.Log10(value) * 20f);
        }

        public void SliderSFXolume(float value)
        {
            GameManager.AudioSystem.GeneralMixer.SetFloat("SFXVolume", Mathf.Log10(value) * 20f);
        }
    }
}