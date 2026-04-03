using UnityEngine;

namespace Root {
    public abstract class Interactable : MonoBehaviour {
        [SerializeField] private GameObject SelectedVisuals;
        public abstract void Interact(bool state);

        public void Select(bool state) {
            if (SelectedVisuals == null) return;
            SelectedVisuals.SetActive(state);
        }
    }
}