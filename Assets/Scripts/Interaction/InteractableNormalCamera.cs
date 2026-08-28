using UnityEngine;

namespace Root {
    public abstract class InteractableNormalCamera : MonoBehaviour {
        public abstract void Interact();

        public virtual void ShowFeedback(bool canShow) { }
    }
}