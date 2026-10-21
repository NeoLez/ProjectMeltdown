using PrimeTween;
using Root.Controller;
using Root.Managers;
using UnityEngine;
using UnityEngine.UIElements;

namespace Root
{
    [RequireComponent(typeof(LockedCameraController))]
    public class DriverSeat : InteractableNormalCamera
    {
        private LockedCameraController _cameraController;
        [SerializeField] private bool enableAnimationDelay;
        [SerializeField] private Transform animPivot;
        [SerializeField] private float duration = 0.1f;

        private Quaternion _initialRotation;

        private void Awake()
        {
            _cameraController = GetComponent<LockedCameraController>();

            _cameraController.OnPlayerSeated += RotateTowardsPlayer;
        }

        private void Start()
        {
            _initialRotation = animPivot.localRotation;
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

        public void RotateTowardsPlayer(bool canRotate)
        {
            if (canRotate)
            {
                animPivot.localRotation = _initialRotation;
            }
            else
            {
                Vector3 distance = (GameManager.Player.gameObject.transform.position - transform.position).normalized;

                if (distance.sqrMagnitude < 0.001f) return;
                float targetAngle = Mathf.Atan2(distance.x, distance.z) * Mathf.Rad2Deg;

                Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);
                
                if(enableAnimationDelay)
                    Tween.LocalRotation(animPivot, animPivot.localRotation, animPivot.localRotation * targetRotation, duration);
                else
                    animPivot.localRotation = targetRotation;
            }
        }
    }
}