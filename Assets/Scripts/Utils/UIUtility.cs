using System.Collections.Generic;
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
    }
}