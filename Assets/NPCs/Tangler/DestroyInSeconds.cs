using UnityEngine;

namespace Root
{
    public class DestroyInSeconds : MonoBehaviour
    {
        [SerializeField] private float _delay = 1.0f;

        void Start()
        {
            Destroy(gameObject, _delay);
        }
    }
}
