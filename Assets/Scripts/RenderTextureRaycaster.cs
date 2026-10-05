using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Root {
    public class RenderTextureRaycaster : GraphicRaycaster
    {
        public override void Raycast(PointerEventData eventData, List<RaycastResult> results) {
            Vector2 rtSize = GameManager.RTSize;

            var original = eventData.position;
            eventData.position = new Vector2(
                original.x / Screen.width  * rtSize.x,
                original.y / Screen.height * rtSize.y);

            base.Raycast(eventData, results);

            eventData.position = original;
        }
    }
}