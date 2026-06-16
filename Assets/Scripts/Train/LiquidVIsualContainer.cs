using UnityEngine;

namespace Root
{
    public class LiquidVIsualContainer : MonoBehaviour
    {
        [SerializeField] TrainBrakeController _controller;
        private Renderer targetRenderer;
        private Material targetMaterial;

        private float Amount = Shader.PropertyToID("_FillAmount");

        void Start()
        {
            targetRenderer = GetComponent<Renderer>();
            targetMaterial = targetRenderer.material;
        }

        void Update()
        {
            float dynamicValue = _controller.GetDamagePercentage();
            var a = dynamicValue * -1f;
            targetMaterial.SetFloat(Shader.PropertyToID("_FillAmount"), a);
        }
    }
}
