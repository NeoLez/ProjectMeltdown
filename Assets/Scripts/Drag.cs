using UnityEngine;
using UnityEngine.InputSystem;

namespace Root {
    public class Drag : MonoBehaviour {
        public Rigidbody obj;
        [SerializeField] private float rayDistance;
        private float _distance;

        private void Awake() {
            GameManager.Input.Interaction.Interact.started += HandleDrag;
        }

        private void OnDestroy() {
            GameManager.Input.Interaction.Interact.started -= HandleDrag;
        }

        public void HandleDrag(InputAction.CallbackContext _) {
            if (obj != null) {
                StopDrag();
            }
            else {
                StartDrag();
            }
        }

        private void StartDrag() {
            if (Physics.Raycast(GameManager.Camera.transform.position, GameManager.Camera.transform.forward,
                    out var hit, rayDistance)) {
                if (hit.rigidbody == null) return;
                if (hit.rigidbody.TryGetComponent<DraggableObject>(out var comp)) {
                    obj = hit.rigidbody;
                    obj.useGravity = false;
                    comp.OnStartedDragging?.Invoke();
                    obj.constraints = RigidbodyConstraints.FreezeRotation;
                    _distance = Vector3.Distance(obj.transform.position, GameManager.Camera.transform.position);
                }
            }
        }

        private void StopDrag() {
            obj.useGravity = true;
            obj.constraints = RigidbodyConstraints.None;
            obj.GetComponent<DraggableObject>().OnStoppedDragging?.Invoke();
            obj = null;
        }

        private void Update() {
            if (obj == null) return;
            obj.MovePosition(GameManager.Camera.transform.position + _distance * GameManager.Camera.transform.forward);
        }
    }
}