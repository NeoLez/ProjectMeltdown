using Root.Controller;
using Root.Managers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Root
{
    public class PauseMenu : Menu.Menu
    {
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private GameObject optionsPanel; 
        [SerializeField] private Slider masterVolumeSlider;
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;
        [SerializeField] private Slider sensitivitySlider; 

        private bool paused;

        private void Awake()
        {
            masterVolumeSlider.SetValueWithoutNotify(VolumeSettings.Get(VolumeSettings.MasterKey)); 
            musicVolumeSlider.SetValueWithoutNotify(VolumeSettings.Get(VolumeSettings.MusicKey)); 
            sfxVolumeSlider.SetValueWithoutNotify(VolumeSettings.Get(VolumeSettings.SfxKey)); 
            masterVolumeSlider.onValueChanged.AddListener(SliderMasterVolume);
            musicVolumeSlider.onValueChanged.AddListener(SliderMusicVolume);
            sfxVolumeSlider.onValueChanged.AddListener(SliderSFXolume);
            GameManager.Input.Menu.Pause.performed += HandleInput;
            sensitivitySlider.minValue = 0f; 
            sensitivitySlider.maxValue = 1f; 
            sensitivitySlider.SetValueWithoutNotify(MouseSensitivitySettings.ToSlider(MouseSensitivitySettings.Value)); 
            sensitivitySlider.onValueChanged.AddListener(SliderSensitivity);
        }

        private void HandleInput(InputAction.CallbackContext _)
        {
            if (PromptManager.Instance != null && PromptManager.Instance.IsOpen)
            {
                PromptManager.Instance.Cancel();
                return;
            }

            if (paused && optionsPanel.activeSelf) 
            {
                CloseOptions(); 
                return; 
            }

            if (paused)
                UIManager.Instance.CloseMenu(UIManager.UITypes.PauseMenu);
            else
                UIManager.Instance.OpenMenu(UIManager.UITypes.PauseMenu);
        }

        public override void Open() {
            base.Open();
            paused = true;
            pausePanel.SetActive(true);
            optionsPanel.SetActive(false);
            GameManager.Input.Movement.Disable();
            GameManager.Input.CameraMovement.Disable();
            GameManager.Input.Interaction.Disable();
            GameManager.PlayerInventoryUI.CloseInventory();
            GameManager.AudioSystem?.PauseAll();
            GameManager.DialogueManager?.StopCurrentDialogue();
            MouseHandler.RequestControl(CursorLockMode.None, true, this, false);
        }
        
        public override void Close() {
            base.Close();
            paused = false;
            pausePanel.SetActive(false);
            optionsPanel.SetActive(false);
            GameManager.Input.Movement.Enable();
            GameManager.Input.CameraMovement.Enable();
            GameManager.Input.Interaction.Enable();
            GameManager.AudioSystem?.ResumeAll();
            GameManager.DialogueManager?.ResumeCurrentDialogue();
            MouseHandler.RelinquishControl(this);
        }

        public void Resume() {
            if (paused)
                UIManager.Instance.CloseMenu(UIManager.UITypes.PauseMenu);
        }

        public void OpenOptions() 
        {
            pausePanel.SetActive(false); 
            optionsPanel.SetActive(true); 
        }

        public void CloseOptions() 
        {
            optionsPanel.SetActive(false); 
            pausePanel.SetActive(true); 
        }

        public void ReturnToMenu()
        {
            PromptManager.Instance.ShowPrompt("confirm_return_menu_message", () =>{
                GameManager.AudioSystem?.ResumeAll();
                GameManager.Input.Enable();
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                MouseHandler.RelinquishControl(this);
                LoadingScreen.Instance.LoadScene("Menu");
            });
        }

        public void SliderSensitivity(float value)
        {
            MouseSensitivitySettings.Value = MouseSensitivitySettings.FromSlider(value); 
        }

        public void SliderMasterVolume(float value)
        {
            VolumeSettings.Set(VolumeSettings.MasterKey, value);
        }

        public void SliderMusicVolume(float value)
        {
            VolumeSettings.Set(VolumeSettings.MusicKey, value);
        }

        public void SliderSFXolume(float value)
        {
            VolumeSettings.Set(VolumeSettings.SfxKey, value); 
        }

        private void OnDestroy()
        {
            GameManager.Input.Menu.Pause.performed -= HandleInput; 
            masterVolumeSlider.onValueChanged.RemoveAllListeners();
            musicVolumeSlider.onValueChanged.RemoveAllListeners();
            sfxVolumeSlider.onValueChanged.RemoveAllListeners();
            sensitivitySlider.onValueChanged.RemoveAllListeners();
        }
    }
}