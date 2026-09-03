using Root.Controller;
using UnityEngine;

namespace Root {
    [RequireComponent(typeof(VisualContainer))]
    public class CameraPivot : MonoBehaviour {
        
        [SerializeField] private Camera cam;
        [SerializeField] private CameraController cameraController;
        [SerializeField] private LockedCameraController lockedCameraController;
        
        private VisualContainer visualContainer;
        
        private void Awake() {
            GameManager.CameraPivot = this;
            visualContainer = GetComponent<VisualContainer>();
            
            visualContainer.OnMatchRealPosition += SyncExitTrain;
            visualContainer.OnChangeToVirtualPosition += SyncEnterTrain;
        }

        private void SyncExitTrain() {
            // EXTING TRAIN: The visual container is snapping away. We tell the 
            // physical pivot to adapt to the exact world rotation the visuals were looking at.
            ApplySync(cam.transform.rotation);
        }

        private void SyncEnterTrain() {
            // ENTERING TRAIN: The physical player just teleported to the Sky Train (originCenter), 
            // but the visual camera shouldn't move. We calculate the inverse rotation so the 
            // physical pivot faces the exact angle required to keep the visuals perfectly still.
            Quaternion targetPivotWorldRot = visualContainer.originCenter.rotation * 
                                             Quaternion.Inverse(visualContainer.goal.rotation) * cam.transform.rotation;
            
            ApplySync(targetPivotWorldRot);
        }

        private void ApplySync(Quaternion rot) {
            if (cameraController.enabled) cameraController.SyncToRotation(rot);
            if (lockedCameraController.enabled) lockedCameraController.SyncToRotation(rot);
        }

        public Ray ScreenPointToRay(Vector2 mousePosition) {
            var r = cam.ScreenPointToRay(mousePosition);
            r.origin = transform.position;
            r.direction = transform.TransformDirection(cam.transform.InverseTransformDirection(r.direction));
            return r;
        }

        private void OnDestroy() {
            if (visualContainer != null) {
                visualContainer.OnMatchRealPosition -= SyncExitTrain;
                visualContainer.OnChangeToVirtualPosition -= SyncEnterTrain;
            }
        }
    }
}