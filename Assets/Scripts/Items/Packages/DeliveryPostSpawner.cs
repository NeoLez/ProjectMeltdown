using UnityEngine;

namespace Root
{
    public class DeliveryPostSpawner : MonoBehaviour
    {
        [SerializeField] private PackageDeliverPost deliveryPost;

        private void OnEnable()
        {
            SpawnPost();
        }

        public void SpawnPost()
        {
            PackageDeliverPost post = Instantiate(deliveryPost);
            post.transform.position = transform.position;
            post.transform.rotation = transform.rotation;
            post.transform.parent = transform.parent;
        }
    }
}
