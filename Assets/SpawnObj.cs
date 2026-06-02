using UnityEngine;

namespace Root
{
    public class SpawnObj : MonoBehaviour
    {
        [SerializeField] private GameObject objPrefab;
        void Start() { 
        
            Debug.Log($"{transform.position }");
            var obj = Instantiate(objPrefab);
            obj.transform.position = transform.position;
            obj.transform.parent = GameManager.MapGeneration.itemRoot;
            obj.transform.GetChild(0).transform.position = transform.position;

            Debug.Log($"{transform.position} {obj.transform.GetChild(0).transform.position}");
            Debug.Break();
        }
    }
}
