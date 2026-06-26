using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Cursor = UnityEngine.Cursor;

namespace Root.Controller
{
    public static class MouseHandler
    {
        private static readonly List<MouseSettings> MouseSettingsList = new();

        public static bool ShowCrosshair { get; private set; }

        public static void RequestControl(
            CursorLockMode lockState,
            bool visible,
            Component requester,
            bool showCrosshair = false)
        {
            var mouseSetting = new MouseSettings();
            mouseSetting.lockState = lockState;
            mouseSetting.visible = visible;
            mouseSetting.requester = requester;
            mouseSetting.showCrosshair = showCrosshair;

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
                        SetMouseSettings(MouseSettingsList[i - 1]);
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

            ShowCrosshair = mouseSetting.showCrosshair;
        }

        public struct MouseSettings
        {
            public CursorLockMode lockState;
            public bool visible;
            public Component requester;
            public bool showCrosshair;

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
        
        /// <summary>
        /// Resets everything to default state, with an unlocked cursor. It should generally only be called before a scene change, where all elements that requested control of the object are no longer valid.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void ClearListAndSetToDefault()
        {
            MouseSettingsList.Clear();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}