using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Timers {
    public static class UIUtility
    {
        public static bool GetFirstComponentUnderCursor<T>(PointerEventData pointerEventData, out T component) where T : class
        {
            component = null;
            if (EventSystem.current == null) return false;


            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, results);

            foreach (RaycastResult result in results)
            {
                if (result.gameObject.TryGetComponent(out component))
                {
                    return true;
                }
            }

            return false;
        }
        
        /// <summary>
        /// Converts a screen position into local coordinates relative to a RectTransform on a Canvas.
        /// </summary>
        /// <param name="canvas">The Canvas the UI element belongs to.</param>
        /// <param name="targetRect">The RectTransform to convert coordinates into.</param>
        /// <param name="screenPosition">The screen space position.</param>
        /// <param name="localPoint">The resulting local position output.</param>
        /// <returns>True if the point was successfully converted inside the rectangle.</returns>
        public static bool ScreenToCanvasPosition(Canvas canvas, RectTransform targetRect, Vector2 screenPosition, out Vector2 localPoint)
        {
            // Screen Space - Overlay canvases don't use a camera (pass null)
            // Screen Space - Camera and World Space canvases require their assigned worldCamera
            Camera cam = null;
            if (canvas.renderMode == RenderMode.ScreenSpaceCamera || canvas.renderMode == RenderMode.WorldSpace)
            {
                cam = canvas.worldCamera;
            }

            return RectTransformUtility.ScreenPointToLocalPointInRectangle(
                targetRect, 
                screenPosition, 
                cam, 
                out localPoint
            );
        }
    }
}