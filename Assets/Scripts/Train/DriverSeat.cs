using Root.Controller;
using UnityEngine;

namespace Root {
    [RequireComponent(typeof(LockedCameraController))]
    public class DriverSeat : InteractableNormalCamera {
        private LockedCameraController _cameraController;

        private void Awake() {
            _cameraController = GetComponent<LockedCameraController>();
        }

        public override void Interact() {
            GameManager.Player.GetComponent<CameraController>().enabled = false;
            GameManager.Input.Movement.Disable();
            _cameraController.enabled = true;
        }
    }
}