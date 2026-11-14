using Root.Controller;
using Root.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

namespace Root
{
    public class MenuController : MonoBehaviour
    {
        [SerializeField] private GameObject pantallaMenu;
        [SerializeField] private GameObject panelOpciones;
        [SerializeField] private GameObject pantallaClases;
        [SerializeField] private GameObject panelInfoClase;
        [SerializeField] private Slider masterVolumeSlider;
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;
        [SerializeField] private Slider sensitivitySlider; 

        // Localization
        [SerializeField] private TMP_Text nombreClase;
        [SerializeField] private TMP_Text descripcionClase;
        [SerializeField] private TMP_Text comienzaCon;

        private int selectedClass;
        private bool loadingScene;

        private void Awake()
        {
            PoolManager.Reset();
            MouseHandler.ClearListAndSetToDefault();
            MouseHandler.RequestControl(CursorLockMode.None, true, this);
            masterVolumeSlider.SetValueWithoutNotify(VolumeSettings.Get(VolumeSettings.MasterKey)); 
            musicVolumeSlider.SetValueWithoutNotify(VolumeSettings.Get(VolumeSettings.MusicKey)); 
            sfxVolumeSlider.SetValueWithoutNotify(VolumeSettings.Get(VolumeSettings.SfxKey)); 
            masterVolumeSlider.onValueChanged.AddListener(SliderMasterVolume);
            musicVolumeSlider.onValueChanged.AddListener(SliderMusicVolume);
            sfxVolumeSlider.onValueChanged.AddListener(SliderSFXVolume);
            sensitivitySlider.minValue = 0f; 
            sensitivitySlider.maxValue = 1f; 
            sensitivitySlider.SetValueWithoutNotify(MouseSensitivitySettings.ToSlider(MouseSensitivitySettings.Value)); 
            sensitivitySlider.onValueChanged.AddListener(SliderSensitivity);
        }

        private void Start()
        {
            pantallaMenu.SetActive(true);
            pantallaClases.SetActive(false);
            panelInfoClase.SetActive(false);
            LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
        }

        private void OnDestroy()
        {
            LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
        }

        public void Play()
        {
            pantallaMenu.SetActive(false);
            pantallaClases.SetActive(true);
            panelInfoClase.SetActive(false);
        }

        public void AbrirOpciones()
        {
            pantallaMenu.SetActive(false);
            panelOpciones.SetActive(true);
        }

        public void CerrarOpciones()
        {
            panelOpciones.SetActive(false);
            pantallaMenu.SetActive(true);
        }

        public void Volver()
        {
            pantallaClases.SetActive(false);
            pantallaMenu.SetActive(true);
            panelInfoClase.SetActive(false);
        }

        public void SliderSensitivity(float value)
        {
            MouseSensitivitySettings.Value = MouseSensitivitySettings.FromSlider(value); 
        }

        #region CLASS_SELECTION
        public void SelectClass(int classNumber)
        {
            selectedClass = classNumber;
            panelInfoClase.SetActive(true);
            UpdateClassInfo();
        }

        private void UpdateClassInfo()
        {
            if (selectedClass < 0)
                return;

            switch (selectedClass) // switch statement to update class info based on selectedClass
            {
                case 0:
                    nombreClase.text = LocalizationSettings.StringDatabase.GetLocalizedString("Classes","class_mechanic_name");
                    descripcionClase.text = LocalizationSettings.StringDatabase.GetLocalizedString("Classes","class_mechanic_description");
                    comienzaCon.text = LocalizationSettings.StringDatabase.GetLocalizedString("Classes","class_mechanic_starts_with");
                    break;

                case 1:
                    nombreClase.text = LocalizationSettings.StringDatabase.GetLocalizedString("Classes","class_electrician_name");
                    descripcionClase.text = LocalizationSettings.StringDatabase.GetLocalizedString("Classes","class_electrician_description");
                    comienzaCon.text = LocalizationSettings.StringDatabase.GetLocalizedString("Classes","class_electrician_starts_with");
                    break;

                case 2:
                    nombreClase.text = LocalizationSettings.StringDatabase.GetLocalizedString("Classes","class_tycoon_name");
                    descripcionClase.text = LocalizationSettings.StringDatabase.GetLocalizedString("Classes","class_tycoon_description");
                    comienzaCon.text = LocalizationSettings.StringDatabase.GetLocalizedString("Classes","class_tycoon_starts_with");
                    break;
            }
        }
        #endregion

        #region AUDIO
        public void SliderMasterVolume(float value)
        {
            VolumeSettings.Set(VolumeSettings.MasterKey, value); 
        }

        public void SliderMusicVolume(float value)
        {
            VolumeSettings.Set(VolumeSettings.MusicKey, value); 
        }

        public void SliderSFXVolume(float value)
        {
            VolumeSettings.Set(VolumeSettings.SfxKey, value); 
        }

        private void OnLocaleChanged(UnityEngine.Localization.Locale locale)
        {
            if (panelInfoClase.activeSelf)
            {
                UpdateClassInfo();
            }
        }
        #endregion region


        public void ConfirmPlay()
        {
            if (loadingScene) return;
            loadingScene = true;
            GameManager.VeryUglyKitNumber = selectedClass;
            LoadingScreen.Instance.LoadScene("Train 1");
            MouseHandler.ClearListAndSetToDefault();
            PoolManager.Reset();
        }
        public void Exit()
        {
            Application.Quit();
        }

    }
}