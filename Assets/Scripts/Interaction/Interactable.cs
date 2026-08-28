using UnityEngine;

namespace Root {
    public abstract class Interactable : MonoBehaviour {
        public abstract void StartInteraction();
        public abstract void EndInteraction();
    }
}