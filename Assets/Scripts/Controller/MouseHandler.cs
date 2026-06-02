using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Cursor = UnityEngine.Cursor;

namespace Root.Controller
{
    public static class MouseHandler
    {
        private static readonly List<MouseSettings> MouseSettingsList = new();

        // crosshair como parametro opcional
        public static void RequestControl(CursorLockMode lockState, bool visible, Component requester, GameObject crosshair = null)
        {
            var mouseSetting = new MouseSettings();
            mouseSetting.lockState = lockState;
            mouseSetting.visible = visible;
            mouseSetting.requester = requester;
            mouseSetting.crosshair = crosshair;
            
            MouseSettingsList.Add(mouseSetting);
            SetMouseSettings(mouseSetting);
        }

        public static void RelinquishControl(Component requester)
        {
            for (int i = MouseSettingsList.Count - 1; i >= 0; i--)
            {
                if (MouseSettingsList[i].requester == null)
                {
                    MouseSettingsList.RemoveAt(i);
                    continue;
                }

                if (MouseSettingsList[i].requester == requester)
                {
                    if (i == MouseSettingsList.Count - 1)
                    {
                        SetMouseSettings(MouseSettingsList[i-1]);
                    }
                    MouseSettingsList.RemoveAt(i);
                    break;
                }
            }
            
        }

        private static void SetMouseSettings(MouseSettings mouseSetting)
        {
            Cursor.lockState = mouseSetting.lockState;
            Cursor.visible = mouseSetting.visible;
            // crosshair activo con el cursor bloqueado
            if (mouseSetting.crosshair != null)
                mouseSetting.crosshair.SetActive(mouseSetting.lockState == CursorLockMode.Locked);
        }
        
        public struct MouseSettings
        {
            public CursorLockMode lockState;
            public bool visible;
            public Component requester;
            public GameObject crosshair; 

            public override string ToString()
            {
                return $"{lockState}, {visible}, {requester.name}\n";
            }
        }

        public static void PrintState()
        {
            StringBuilder sb = new();
            foreach (var mouseSetting in MouseSettingsList)
            {
                sb.Append(mouseSetting.ToString());
            }
            Debug.Log(sb.ToString());
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void ResetStaticData()
        {
            MouseSettingsList.Clear();
        }
    }
}