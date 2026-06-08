using UnityEngine;

namespace Root
{
    public class AskRender : MonoBehaviour
    {
        [SerializeField] RenderTexture renderTexture;
        void Update()
        {
            OneShotRenderSystem.Instance.Render(renderTexture);
        }
    }
}
