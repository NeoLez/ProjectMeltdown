using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace Root
{
    public class PackageStampGenerator : MonoBehaviour
    {
        public static PackageStampGenerator Instance;
        [Header("Price")]
        [SerializeField] private TMP_Text m_Text;
        [SerializeField] private string format = "{0}$";

        private int width;
        private int height;
        private CanvasScaler _canvasScaler;
        private Canvas _canvas;

        private void Awake()
        {
            _canvasScaler = GetComponent<CanvasScaler>();
            _canvas = GetComponent<Canvas>();
            Instance = this;
        }

        private void Start()
        {
            SetTextureResolution();
        }

        private void SetTextureResolution()
        {
            var x = _canvasScaler.referenceResolution.x;
            var y = _canvasScaler.referenceResolution.y;
            width = (int)x;
            height = (int)y;
        }

        public RenderTexture CreateStampTexture(PackageItemState state)
        {
            RenderTexture render = new RenderTexture(new RenderTextureDescriptor(width, height, RenderTextureFormat.ARGB32, 16));

            SetDisplayValue(state.price);

            EnableCanvas(true);
            OneShotRenderSystem.Instance.Render(render);
            EnableCanvas(false);
            return render;
        }

        public void SetStampTexture(GameObject obj, PackageItemState state)
        {
            DecalProjector heldItemProjector = obj.GetComponentInChildren<DecalProjector>();
            var mate = new Material(heldItemProjector.material);
            heldItemProjector.material = mate;
            heldItemProjector.material.SetTexture("_Texture", state.stampTexture);
            heldItemProjector.fadeFactor = 1.0f;
        }

        private void EnableCanvas(bool state)
        {
            _canvas.enabled = state;
        }

        private void SetDisplayValue(float value)
        {
            m_Text.text = string.Format(format, value);
        }
    }
}
