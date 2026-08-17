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
        private MeshRenderer render;

        private void Awake() {
            render = GetComponent<MeshRenderer>();
        }

        void Update()
        {
            var sp = train.GetCurrentMaxSpeed();
            if (sp > speed) render.material = safe;
            else if (sp < speed) render.material = warning;
            else render.material = limit;
        }
    }
}
