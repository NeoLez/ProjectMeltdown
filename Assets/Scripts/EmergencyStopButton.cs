using UnityEngine;

namespace Root {
    public class EmergencyStopButton : Interactable {
        public int maxUses;
        public int usesLeft;
        public bool isBraking;
        
        public float brakeSpeed;

        public GameObject onObject;
        public GameObject offObject;

        private void Awake() {
            UpdateVisuals();
        }

        public override void Interact(bool state) {
            if (state && !isBraking && !IsSpent()) {
                isBraking = true;
                usesLeft--;
                UpdateVisuals();
            }
        }

        private void UpdateVisuals() {
            var spent = IsSpent();
            onObject.SetActive(!spent);
            offObject.SetActive(spent);
        }
        
        public bool IsBreaking() {
            return isBraking;
        }
        
        public void FinishBraking() {
            isBraking = false;
        }

        public int Repair(int amount) {
            int maxRepairs = maxUses - usesLeft;
            if (amount > maxRepairs) {
                usesLeft = maxUses;
                UpdateVisuals();
                return amount - maxRepairs;
            }
            
            usesLeft += amount;
            UpdateVisuals();
            return 0;
        }

        public bool IsSpent() {
            return usesLeft <= 0;
        }
    }
}