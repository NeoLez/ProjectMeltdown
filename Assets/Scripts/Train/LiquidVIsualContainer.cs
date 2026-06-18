using UnityEngine;

namespace Root
{
    public class LiquidVIsualContainer : MonoBehaviour
    {
        [SerializeField] TrainBrakeController _controller;
        private Renderer targetRenderer;
        private Material targetMaterial;
        //-0.95, 0.1  + 0.95 both = 0, 1.1
        float _max = 1.05f;
        float _current;
        private float Amount = Shader.PropertyToID("_FillAmount");

        void Start()
        {
            _current = _max;
            targetRenderer = GetComponent<Renderer>();
            targetMaterial = targetRenderer.material;
        }

        void Update()
        {
            float dynamicValue = _controller.GetDamagePercentage();
            var a = _max - (_max * dynamicValue);
            _current = a;
            targetMaterial.SetFloat(Shader.PropertyToID("_FillAmount"), _current -0.95f);
        }
    }
}
