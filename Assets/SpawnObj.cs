using UnityEngine;

namespace Root
{
    public class SpawnObj : MonoBehaviour
    {
        [SerializeField] private ItemGenerationPoolSO pool;
        void Start() { 
            
            var obj = Instantiate(pool.GetRandom().GameObject);
            obj.transform.position = transform.position;
            obj.transform.rotation = Quaternion.Euler(Vector3.up * Random.Range(0f, 360f));
            obj.transform.parent = transform.parent;
            obj.transform.GetChild(0).transform.position = transform.position;
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position, 0.3f);
        }
    }
}
