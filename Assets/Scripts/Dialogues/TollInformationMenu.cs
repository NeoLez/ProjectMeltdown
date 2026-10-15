using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Root
{
    public class TollInformationMenu : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Canvas informationDisplayPrefab;
        [SerializeField] private float displayOffset;

        private Canvas _info;
        private RectTransform _buttonTransform;
        private Button _choiseButton;
        private Vector2 offset;
        private string _txt;
        private bool _canShow;

        private void Awake()
        {
            _choiseButton = GetComponent<Button>();
        }
        private void Start()
        {
            _buttonTransform = GetComponent<RectTransform>();

            offset.y += _buttonTransform.sizeDelta.y * displayOffset;
        }

        //TODO-que el texto se construya a partir de lo solicitado por cada toll
        public void Initialize(string requestText)
        {
            _txt = requestText;

            _canShow = !string.IsNullOrEmpty(requestText);
        }

        public void EnableChoise(bool state)
        {
            _choiseButton.interactable = state;
        }

        public void Show()
        {
            if (_info == null)
            {
                _info = Instantiate(informationDisplayPrefab, transform);
                _info.GetComponent<RectTransform>().anchoredPosition = offset;
                _info.GetComponentInChildren<TMP_Text>().text = _txt;
            }

            _info.enabled = true;
        }

        public void Hide()
        {
            _info.enabled = false;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!_canShow) return;

            Show();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!_canShow) return;

            Hide();
        }

        public void ClearPanelInformation()
        {
            _txt = string.Empty;
            if (_info != null) Destroy(_info.gameObject);
        }
    }
}
