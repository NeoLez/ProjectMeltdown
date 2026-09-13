using TMPro;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.Localization.SmartFormat.Core.Parsing;
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
        private CanvasScaler m_Canvas;

        private void Awake()
        {
            m_Canvas = GetComponent<CanvasScaler>();
        }

        private void Start()
        {
            EnableCanvas(false);
            SetTextureResolution();
        }

        private void SetTextureResolution()
        {
            var x = m_Canvas.referenceResolution.x;
            var y = m_Canvas.referenceResolution.y;
            width = (int)x;
            height = (int)y;
        }

        public void CreateStamp(GameObject obj)
        {
            RenderTexture render = new RenderTexture(new RenderTextureDescriptor(width, height, RenderTextureFormat.ARGB32, 16));

            var package = obj.GetComponent<DeliveryPackageItem>();
            SetDisplayValue(package.PackageData.Price);

            var r = obj.GetComponentInChildren<DecalProjector>();
            var mate = new Material(r.material);
            r.material = mate;
            r.material.SetTexture("_Texture",render);

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
