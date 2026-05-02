using UnityEngine;

namespace Root {
    [RequireComponent(typeof(VisualContainer))]
    public class CameraPivot : MonoBehaviour {
        
        [SerializeField] private Camera cam;

        public Ray ScreenPointToRay(Vector2 mousePosition) {
            var r = cam.ScreenPointToRay(mousePosition);
            r.origin = transform.position;
            r.direction = transform.TransformDirection(cam.transform.InverseTransformDirection(r.direction));
            return r;
        }
    }
}