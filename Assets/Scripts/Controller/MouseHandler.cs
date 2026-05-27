using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Cursor = UnityEngine.Cursor;

namespace Root.Controller
{
    public static class MouseHandler
    {
        private static List<MouseSettings> mouseSettings = new();

        public static void RequestControl(CursorLockMode lockState, bool visible, Component requester)
        {
            var mouseSetting = new MouseSettings();
            mouseSetting.lockState = lockState;
            mouseSetting.visible = visible;
            mouseSetting.requester = requester;
            
            mouseSettings.Add(mouseSetting);
            SetMouseSettings(mouseSetting);
        }

        public static void RelinquishControl(Component requester)
        {
            Debug.Log("A");
            for (int i = mouseSettings.Count - 1; i >= 0; i--)
            {
                if (mouseSettings[i].requester == null)
                {
                    mouseSettings.RemoveAt(i);
                    Debug.Log("Cleaned");
                    continue;
                }

                if (mouseSettings[i].requester == requester)
                {
                    Debug.Log("Removed");
                    if (i == mouseSettings.Count - 1)
                    {
                        Debug.Log("ASD");
                        SetMouseSettings(mouseSettings[i-1]);
                    }
                    mouseSettings.RemoveAt(i);
                    break;
                }
            }
            
        }

        private static void SetMouseSettings(MouseSettings mouseSetting)
        {
            Cursor.lockState = mouseSetting.lockState;
            Cursor.visible = mouseSetting.visible;
            PrintState();
        }
        
        public struct MouseSettings
        {
            public CursorLockMode lockState;
            public bool visible;
            public Component requester;

            public override string ToString()
            {
                return $"{lockState}, {visible}, {requester.name}\n";
            }
        }

        public static void PrintState()
        {
            StringBuilder sb = new();
            foreach (var mouseSetting in mouseSettings)
            {
                sb.Append(mouseSetting.ToString());
            }
            Debug.Log(sb.ToString());
        }
    }
}