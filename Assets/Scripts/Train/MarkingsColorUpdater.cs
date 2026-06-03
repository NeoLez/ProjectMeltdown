using UnityEngine;

namespace Root
{
    [RequireComponent (typeof (MeshRenderer))]
    public class MarkingsColorUpdater : MonoBehaviour
    {
        [SerializeField] Train train;
        [SerializeField] float speed;
        [SerializeField] Material safe;
        [SerializeField] Material limit;
        [SerializeField] Material warning;
        [SerializeField] MeshRenderer renderer;
        void Update()
        {
            var sp = train.GetCurrentMaxSpeed();
            if (sp > speed) renderer.material = safe;
            else if (sp < speed) renderer.material = warning;
            else renderer.material = limit;
        }
    }
}
