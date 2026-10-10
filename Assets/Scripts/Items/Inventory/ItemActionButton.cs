using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Root {
    public class ItemActionButton : MonoBehaviour, IPointerClickHandler {
        [SerializeField] private TMP_Text actionNameText;
        private ItemAction _action;

        public void SetAction(ItemAction action) {
            _action = action;
            actionNameText.text = _action.ActionName;
        }
        
        public void OnPointerClick(PointerEventData eventData) {
            if (eventData.button == PointerEventData.InputButton.Left) {
                _action.RunAction();
            }
            PlayerInventoryUI.Instance.ClearOptionsDialogue();
        }
    }
}