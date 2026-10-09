using System;

namespace Root {
    public class ItemAction {
        public string ActionName { get; private set; }
        public string ActionDescription { get; private set; }
        private readonly Action _action;

        public ItemAction(string actionName, string actionDescription, Action action) {
            ActionName = actionName;
            ActionDescription = actionDescription;
            _action = action;
        }
        
        public void RunAction() {
            _action.Invoke();
        }
    }
}