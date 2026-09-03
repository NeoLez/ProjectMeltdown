using System;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class PromptManager : MonoBehaviour
{
    public static PromptManager Instance { get; private set; }

    [SerializeField] private GameObject promptPanel;
    [SerializeField] private TMP_Text promptText;

    private Action _onConfirm;
    private string _localizationKey;
    private bool _isOpen;
    public bool IsOpen => _isOpen; 

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        promptPanel.SetActive(false);
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
    }

    private void OnDestroy()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
    }

    public void ShowPrompt(string localizationKey, Action onConfirm)
    {
        _localizationKey = localizationKey;
        _onConfirm = onConfirm;
        _isOpen = true;
        UpdateLocalizedText();
        promptPanel.SetActive(true);
    }

    public void Confirm()
    {
        if (!_isOpen) return;
        Action action = _onConfirm;
        ClosePrompt();
        action?.Invoke();
    }

    public void Cancel()
    {
        if (!_isOpen) return;
        ClosePrompt();
    }

    private void ClosePrompt()
    {
        _isOpen = false;
        _onConfirm = null;
        _localizationKey = null;
        promptPanel.SetActive(false);
    }

    private void OnLocaleChanged(UnityEngine.Localization.Locale locale)
    {
        if (!_isOpen) return;
        UpdateLocalizedText();
    }

    private async void UpdateLocalizedText()
    {
        if (string.IsNullOrEmpty(_localizationKey)) return;
        string localizedText = await LocalizationSettings.StringDatabase.GetLocalizedStringAsync(_localizationKey).Task;
        if (!_isOpen) return;
        promptText.text = localizedText;
    }
}