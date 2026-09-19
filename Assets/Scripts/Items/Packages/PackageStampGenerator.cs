using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace Root
{
    public class PackageStampGenerator : MonoBehaviour
    {
        [Header("Price")]
        [SerializeField] private TMP_Text m_Text;
        [SerializeField] private string format = "{0}$";

        private int width;
        private int height;
        private CanvasScaler _canvasScaler;

        private void Awake()
        {
            _canvasScaler = GetComponent<CanvasScaler>();
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

        public void CreateStamp(GameObject obj)
        {
            RenderTexture render = new RenderTexture(new RenderTextureDescriptor(width, height, RenderTextureFormat.ARGB32, 16));

            var package = obj.GetComponent<DeliveryPackageItem>();
            SetDisplayValue(package.GetPrice());

            var r = obj.GetComponentInChildren<DecalProjector>();
            var mate = new Material(r.material);
            r.material = mate;
            r.material.SetTexture("_Texture", render);

            OneShotRenderSystem.Instance.Render(render);

            r.fadeFactor = 1.0f; //asi no vemos el cambio de textura 
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
