using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Root
{
    public class GraphicSettings : MonoBehaviour
    {
        [Header("Quality")]
        [SerializeField] private TMP_Dropdown qualityDropdown;

        [Header("Resolution")]
        [SerializeField] private TMP_Dropdown resolutionDropdown;
        [SerializeField] private TMP_Dropdown fullscreenDropdown;

        private List<Resolution> _availableResolutions = new List<Resolution>();

        private const string GraphiscKey = "GraphicsSettings";
        private const string _ResolutionWidthKey = "ResolutionWidth";
        private const string _ResolutionHeightKey = "ResolutionHeight";
        private const string _FullScreenKey = "FullScreen";

        private void Awake()
        {
            int defaultLevel = QualitySettings.GetQualityLevel();
            int savedLevel = Mathf.Clamp(PlayerPrefs.GetInt(GraphiscKey, defaultLevel), 0, QualitySettings.names.Length - 1);
            QualitySettings.SetQualityLevel(savedLevel);
        }

        private void Start()
        {
            SetUpQualityLevel();

            SetupResolutions();
            SetupFullscreen();
        }

        private void SetUpQualityLevel()
        {
            if (qualityDropdown == null)
            {
                Debug.LogError("QualityDropdown is missing, please assign it", this);
                return;
            }

            qualityDropdown.ClearOptions();
            qualityDropdown.AddOptions(new List<string>(QualitySettings.names));
            qualityDropdown.SetValueWithoutNotify(QualitySettings.GetQualityLevel());
            qualityDropdown.RefreshShownValue();
            qualityDropdown.onValueChanged.AddListener(SetQuality);
        }

        private void SetQuality(int qualityIndex)
        {
            if (qualityIndex < 0 || qualityIndex >= QualitySettings.names.Length)
            {
                return;
            }

            QualitySettings.SetQualityLevel(qualityIndex);
            PlayerPrefs.SetInt(GraphiscKey, qualityIndex);
            PlayerPrefs.Save();
        }
        private void SetupResolutions()
        {
            Resolution[] resolutions = Screen.resolutions;
            List<string> options = new List<string>();

            HashSet<string> addedResolutions = new HashSet<string>();

            foreach (var item in resolutions)
            {
                string key = item.width + "x" + item.height;

                if (!addedResolutions.Add(key))
                    continue;

                _availableResolutions.Add(item);
                options.Add(key);
            }

            resolutionDropdown.AddOptions(options);

            int savedWidth = PlayerPrefs.GetInt(_ResolutionWidthKey, Screen.currentResolution.width);
            int savedHeight = PlayerPrefs.GetInt(_ResolutionHeightKey, Screen.currentResolution.height);

            int selectedIndex = FindResolutionIndex(savedWidth, savedHeight);

            resolutionDropdown.SetValueWithoutNotify(selectedIndex);

            resolutionDropdown.onValueChanged.AddListener(SetResolution);
        }

        private void SetupFullscreen()
        {
            List<string> options = new List<string>
        {
            "Pantalla completa",
            "Pantalla sin bordes",
            "Ventana"
        };

            FullScreenMode savedMode = GetSavedFullScreenMode();
            int dropdownIndex = FullscreenModeToDropdownIndex(savedMode);
            fullscreenDropdown.AddOptions(options);

            fullscreenDropdown.SetValueWithoutNotify(dropdownIndex);

            fullscreenDropdown.onValueChanged.AddListener(SetFullscreen);
        }

        private int FindResolutionIndex(int width, int height)
        {
            for (int i = 0; i < _availableResolutions.Count; i++)
            {
                if (_availableResolutions[i].width == width &&
                    _availableResolutions[i].height == height)
                    return i;
            }

            return -1;
        }

        private int FullscreenModeToDropdownIndex(FullScreenMode mode)
        {
            switch (mode)
            {
                case FullScreenMode.ExclusiveFullScreen:
                    return 0;
                case FullScreenMode.FullScreenWindow:
                    return 1;
                case FullScreenMode.Windowed:
                    return 2;
                default:
                    return -1;
            }
        }
        private FullScreenMode GetSavedFullScreenMode()
        {
            return (FullScreenMode)PlayerPrefs.GetInt(_FullScreenKey, (int)FullScreenMode.FullScreenWindow);
        }

        private FullScreenMode DropdownIndexToFullscreenMode(int index)
        {
            switch (index)
            {
                case 0:
                    return FullScreenMode.ExclusiveFullScreen;
                case 1:
                    return FullScreenMode.FullScreenWindow;
                case 2:
                    return FullScreenMode.Windowed;
                default:
                    return FullScreenMode.FullScreenWindow;
            }
        }

        public void SetResolution(int index)
        {
            if (index < 0 || index >= _availableResolutions.Count)
                return;

            Resolution resolution = _availableResolutions[index];

            PlayerPrefs.SetInt(_ResolutionWidthKey, resolution.width);
            PlayerPrefs.SetInt(_ResolutionHeightKey, resolution.height);
            PlayerPrefs.Save();

            ApplyCurrentSettings();
        }

        public void SetFullscreen(int index)
        {
            FullScreenMode mode = DropdownIndexToFullscreenMode(index);

            PlayerPrefs.SetInt(_FullScreenKey, (int)mode);
            PlayerPrefs.Save();

            ApplyCurrentSettings();
        }

        private void ApplyCurrentSettings()
        {
            int width = PlayerPrefs.GetInt(_ResolutionWidthKey, Screen.width);
            int height = PlayerPrefs.GetInt(_ResolutionHeightKey, Screen.height);
            FullScreenMode mode = GetSavedFullScreenMode();

            Screen.SetResolution(width, height, mode);
        }

        private void OnDestroy()
        {
            if (resolutionDropdown != null)
                resolutionDropdown.onValueChanged.RemoveListener(SetResolution);

            if (fullscreenDropdown != null)
                fullscreenDropdown.onValueChanged.RemoveListener(SetFullscreen);

            if (qualityDropdown != null)
            {
                qualityDropdown.onValueChanged.RemoveAllListeners();
            }
        }
    }
}
