using UnityEngine;

namespace Root {
    [CreateAssetMenu(menuName = "SO/ItemSO", fileName =  "ItemSO")]
    public class ItemSO : ScriptableObject {
        [field: SerializeField] public GameObject GameObject { get; private set; }
        [field: SerializeField] public GameObject HeldItemGameObject { get; private set; }
        [field: SerializeField] public string ItemName { get; private set; }
        [field: SerializeField] public string Description { get; private set; }
    }
}