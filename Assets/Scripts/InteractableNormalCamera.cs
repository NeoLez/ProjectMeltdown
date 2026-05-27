using UnityEngine;

namespace Root {
    public abstract class InteractableNormalCamera : MonoBehaviour {
        [SerializeField] private GameObject SelectedVisuals;
        public abstract void Interact();
    }
}