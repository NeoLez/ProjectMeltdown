using UnityEngine;
using UnityEngine.InputSystem;

namespace Root.Controller
{
    public abstract class InteractableDraggable : Interactable
    {
        [SerializeField] private Transform mousePivotPoint;
        private Camera _camera;
        protected bool active;
        
        public override void StartInteraction()
        {
            active = true;
            MouseHandler.RequestControl(CursorLockMode.Confined, false, this);
        }

        public override void EndInteraction()
        {
            active = false;
            MouseHandler.RelinquishControl(this);
        }

#pragma warning disable CS0162
        protected void UpdateMousePosition()
        {
#if UNITY_EDITOR_LINUX
            return;
#endif
            if(!active) return;
            Mouse.current.WarpCursorPosition(_camera.WorldToScreenPoint(mousePivotPoint.position) * GameManager.GetResolutionRatio());
        }

        protected void SetCamera(Camera cam)
        {
            _camera = cam;
        }
    }
#pragma warning restore CS0162
}