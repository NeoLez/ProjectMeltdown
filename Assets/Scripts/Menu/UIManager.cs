using System.Collections.Generic;
using UnityEngine;

namespace Root.Managers {
    public class UIManager : MonoBehaviour {
        public static UIManager Instance { get; protected set; }
        public enum UITypes {
            PauseMenu,
            Inventory,
            Dialogue,
            GameOver,
        }
        [SerializeField] private Menu.Menu pauseMenu;
        [SerializeField] private Menu.Menu dialogueMenu;
        [SerializeField] private Menu.Menu inventoryMenu;
        [SerializeField] private Menu.Menu gameOverMenu;
        
        private readonly Dictionary<UITypes, int> _lockedUI = new();

        private void Awake() {
            Instance = this;
            _lockedUI.Add(UITypes.PauseMenu, 0);
            _lockedUI.Add(UITypes.Dialogue, 0);
            _lockedUI.Add(UITypes.Inventory, 0);
            _lockedUI.Add(UITypes.GameOver, 0);
        }

        public bool OpenMenu(UITypes type) {
            Debug.Log(_lockedUI[type]);
            if (_lockedUI[type] >= 1) return false;
            Open(GetMenuFromType(type));
            return true;
        }
        
        public bool CloseMenu(UITypes type) {
            if (_lockedUI[type] > 1) return false;
            Close(GetMenuFromType(type));
            return true;
        }

        private void Open(Menu.Menu menu) {
            if(menu.IsEnabled) return;
            foreach (var ui in menu.uiToDisable) {
                Close(GetMenuFromType(ui));
            }

            foreach (var ui in menu.uiToLock) {
                _lockedUI[ui] += 1;
                GetMenuFromType(ui).IsLocked = true;
            }

            menu.Open();
        }
        
        private void Close(Menu.Menu menu) {
            if(!menu.IsEnabled) return;
            foreach (var ui in menu.uiToLock) {
                _lockedUI[ui] -= 1;
                if (_lockedUI[ui] == 0) GetMenuFromType(ui).IsLocked = false; 
                if (_lockedUI[ui] < 0) _lockedUI[ui] = 0;
            }

            menu.Close();
        }

        private Menu.Menu GetMenuFromType(UITypes type) {
            switch (type) {
                case UITypes.PauseMenu:
                    return pauseMenu;
                case UITypes.Dialogue:
                    return dialogueMenu;
                case UITypes.Inventory:
                    return inventoryMenu;
                case UITypes.GameOver:
                    return gameOverMenu;
                
                default:
                    Debug.LogError($"{type} not supported");
                    return null;
            }
        }
    }
}