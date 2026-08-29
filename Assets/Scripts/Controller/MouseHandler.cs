using Timers;
using UnityEngine;
using Cursor = UnityEngine.Cursor;

namespace Root.Controller
{
    public static class MouseHandler {
        private static readonly ClaimableValue<MouseSettings> MouseValue = new(SetMouseSettings, DefaultMouseSettings);

        public static bool GetCrosshair() {
            return MouseValue.GetCurrentValue().ShowCrosshair;
        }

        public static void RequestControl(CursorLockMode lockState, bool visible, Component requester, bool showCrosshair = false)
        {
            MouseValue.RequestControl(new MouseSettings(lockState, visible, showCrosshair), requester);
        }

        public static void RelinquishControl(Component requester)
        {
            MouseValue.RelinquishControl(requester);
        }

        private static void SetMouseSettings(MouseSettings mouseSetting)
        {
            Cursor.lockState = mouseSetting.LockState;
            Cursor.visible = mouseSetting.Visible;
        }

        private static MouseSettings DefaultMouseSettings() {
            return new MouseSettings(CursorLockMode.None, true, false);
        }

        private struct MouseSettings
        {
            public CursorLockMode LockState;
            public bool Visible;
            public bool ShowCrosshair;

            public MouseSettings(CursorLockMode lockState, bool visible, bool showCrosshair) {
                LockState = lockState;
                Visible = visible;
                ShowCrosshair = showCrosshair;
            }
            
            public override string ToString()
            {
                return $"{LockState}, {Visible}, {ShowCrosshair}\n";
            }
        }

        public static void PrintState()
        {
            Debug.Log(MouseValue.ToString());
        }
        
        /// <summary>
        /// Resets everything to default state, with an unlocked cursor. It should generally only be called before a scene change, where all elements that requested control of the object are no longer valid.
        /// </summary>
        public static void ClearListAndSetToDefault()
        {
            MouseValue.ClearListAndSetToDefault();
        }
    }
}