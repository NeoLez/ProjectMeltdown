using Root.Controller;
using Root.Managers;
using UnityEngine;

namespace Root {
    [RequireComponent(typeof(LockedCameraController))]
    public class DriverSeat : InteractableNormalCamera {
        private LockedCameraController _cameraController;

        private void Awake() {
            _cameraController = GetComponent<LockedCameraController>();
        }

        public override void Interact()
        {
            if (GameManager.Player.GetComponent<PlayerItemHolder>()?.HasItem == true)
                return;

            GameManager.Player.GetComponent<CameraController>().enabled = false;
            GameManager.Input.Movement.Disable();
            GameManager.Input.Inventory.Disable();
            UIManager.Instance.CloseMenu(UIManager.UITypes.Inventory);
            _cameraController.enabled = true;
        }
    }
}