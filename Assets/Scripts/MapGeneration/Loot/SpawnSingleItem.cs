using Timers;
using UnityEngine;

namespace Root
{
    public class SpawnSingleItem : MonoBehaviour
    {
        [SerializeField] private ItemSo item;
        void Start()
        {
            var obj = item.CreatePhysicalItem();
            obj.transform.position = transform.position;
            obj.transform.rotation = transform.rotation;
            obj.transform.parent = transform.parent;
            obj.transform.GetChild(0).transform.position = transform.position;

            Rigidbody rB =  obj.GetComponent<Rigidbody>();
            rB.isKinematic = true;
            rB.useGravity = false;
        }
    }
}
