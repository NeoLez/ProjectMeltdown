using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Root {
    public class OneShotRenderSystem : MonoBehaviour {
        public static OneShotRenderSystem Instance { get; private set; }
        private void Awake() {
            Instance = this;
        }

        [SerializeField] private Camera cam;

        public void Render(RenderTexture target) {
            cam.targetTexture = target;
            var request = new UniversalRenderPipeline.SingleCameraRequest
            {
                destination = target
            };

            Assert.IsTrue(RenderPipeline.SupportsRenderRequest(cam, request));
            RenderPipeline.SubmitRenderRequest(cam, request);
        }

        public Camera GetCamera() {
            return cam;
        }

        public void SetCanvasSettings(Canvas canvas) {
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = cam;
            if (canvas.transform.localPosition.z <= cam.farClipPlane)
                canvas.transform.localPosition = Vector3.forward * cam.farClipPlane * 1.1f;
        }
    }
}