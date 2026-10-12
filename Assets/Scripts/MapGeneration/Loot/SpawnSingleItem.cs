using Root.Factories;
using Timers;
using Unity.Mathematics;
using UnityEngine;
using Random = System.Random;

namespace Root
{
    public class SpawnSingleItem : MonoBehaviour
    {
        [SerializeField] private ItemSo item;
        [SerializeField] private MapSection mapSection;
        void Start()
        {
            ItemSpawnContext ctx;
            if (mapSection != null) {
                ctx = new(mapSection, new Random(SeedUtils.PositionToSeed(transform.position)));
            }
            else {
                ctx = new();
            }
            
            var obj = item.CreatePhysicalItem(ctx);
            obj.transform.position = transform.position;
            obj.transform.rotation = transform.rotation;
            obj.transform.parent = transform.parent;
            obj.transform.GetChild(0).transform.position = transform.position;

            Rigidbody rB =  obj.GetComponent<Rigidbody>();
            rB.isKinematic = true;
            rB.useGravity = false;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos() {
            float maxDistance = 20;
            if (item == null || item.PhysicalItemPrefab == null) return;
            float distance = Vector2.Distance(Camera.current.transform.position, transform.position);
            if (distance > maxDistance) return;
            var mesh = item.PhysicalItemPrefab.GetComponentInChildren<MeshFilter>();
            if (mesh != null && mesh.sharedMesh != null) {
                Gizmos.color = new Color(0, 1, 1, math.lerp(0.4f, 0f, distance/maxDistance));
                Gizmos.DrawMesh(mesh.sharedMesh, transform.position, transform.rotation, mesh.transform.lossyScale);
            }
            else {
                Gizmos.color = Color.purple;
                Gizmos.DrawWireSphere(transform.position, 0.2f);
            }

            if (distance > maxDistance / 2) return;
            Gizmos.DrawLine(transform.position, transform.position + Vector3.up * 0.5f);

            Vector3 labelPos = transform.position + (Vector3.up * 0.5f);
            GUIStyle style = new GUIStyle();
            style.normal.background = Texture2D.whiteTexture;
            style.normal.textColor = Color.black;
            style.alignment = TextAnchor.MiddleCenter;
            style.fontStyle = FontStyle.Bold;
            style.fontSize = 16;

            UnityEditor.Handles.Label(labelPos, $"Item: {item.ItemName}", style);
        }
#endif
    }
}
