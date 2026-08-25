using UnityEngine;
using UnityEngine.EventSystems;

namespace Root {
    public class InventoryItemDisplay : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
        
        public virtual void Initialize(Sprite itemIcon) {
            
        }

        public void OnBeginDrag(PointerEventData eventData) {
        }

        public void OnDrag(PointerEventData eventData) {
        }

        public void OnEndDrag(PointerEventData eventData) {
        }
    }
}