using System;
using System.Collections.Generic;
using Root.Managers;
using UnityEngine;

namespace Root.Menu {
    public class Menu : MonoBehaviour {
        [field: SerializeField] public bool IsEnabled { get; protected set; }
        public bool IsLocked;

        public event Action OnOpen;
        public event Action OnClose;

        public List<UIManager.UITypes> uiToLock = new();
        public List<UIManager.UITypes> uiToDisable = new();

        public virtual void Open() {
            if (IsEnabled) return;
            IsEnabled = true;
            OnOpen?.Invoke();
        }
        
        public virtual void Close() {
            if(!IsEnabled) return;
            IsEnabled = false;
            OnClose?.Invoke();
        }
    }
}
