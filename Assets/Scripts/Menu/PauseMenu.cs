using Root.Controller;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Root
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private Slider masterVolumeSlider;
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;

        private bool paused;

        private void Awake()
        {
            masterVolumeSlider.onValueChanged.AddListener(SliderMasterVolume);
            musicVolumeSlider.onValueChanged.AddListener(SliderMusicVolume);
            sfxVolumeSlider.onValueChanged.AddListener(SliderSFXolume);
        }

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
                GameManager.PlayerInventoryUI.CloseInventory();

                GameManager.AudioSystem?.PauseAll();
                GameManager.DialogueManager?.StopCurrentDialogue();
                MouseHandler.RequestControl(CursorLockMode.None, true, this, false);
            }
            else
            {
                GameManager.Input.Movement.Enable();
                GameManager.Input.CameraMovement.Enable();
                GameManager.Input.Interaction.Enable();

                GameManager.AudioSystem?.ResumeAll();
                GameManager.DialogueManager?.ResumeCurrentDialogue();
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

        private void OnDestroy()
        {
            masterVolumeSlider.onValueChanged.RemoveAllListeners();
            musicVolumeSlider.onValueChanged.RemoveAllListeners();
            sfxVolumeSlider.onValueChanged.RemoveAllListeners();
        }
    }
}