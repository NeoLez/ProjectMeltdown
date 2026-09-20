using TMPro;
using UnityEngine;
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

        private void Awake()
        {
            _canvasScaler = GetComponent<CanvasScaler>();
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

        public RenderTexture CreateStampTexture(GameObject obj)
        {
            RenderTexture render = new RenderTexture(new RenderTextureDescriptor(width, height, RenderTextureFormat.ARGB32, 16));

            var package = obj.GetComponent<DeliveryPackageItem>();
            SetDisplayValue(package.GetPrice());

            OneShotRenderSystem.Instance.Render(render);
            return render;
        }

        public void EnableCanvas(bool state)
        {
            gameObject.SetActive(state);
        }

        private void SetDisplayValue(float value)
        {
            m_Text.text = string.Format(format, value);
        }
    }
}
