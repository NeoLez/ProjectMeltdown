using UnityEngine;

namespace Root {
    public class EmergencyStopButton : Interactable {
        public bool used;
        public float brakeSpeed;

        public GameObject onObject;
        public GameObject offObject;
        
        public override void Interact(bool state) {
            if (state && !used) {
                used = true;
                onObject.SetActive(false);
                offObject.SetActive(true);
                Debug.Log("Emergency Stop");
            }
        }

        public bool ShouldBreak() {
            return used && !reachedFullStop;
        }
        
        public void ReachedStop() {
            reachedFullStop = true;
        }
        
        public bool reachedFullStop;
    }
}