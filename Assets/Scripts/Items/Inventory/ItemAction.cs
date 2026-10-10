using System;

namespace Root {
    public struct ItemAction {
        public readonly string ActionName;
        public readonly Action _action;

        public ItemAction(string actionName, Action action) {
            ActionName = actionName;
            _action = action;
        }
        
        public void RunAction() {
            _action.Invoke();
        }
    }
}