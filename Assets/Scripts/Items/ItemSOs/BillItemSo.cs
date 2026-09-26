using UnityEngine;

namespace Root {
    [CreateAssetMenu(menuName = "Items/Bill Item")]
    public class BillItemSo : ItemSo {
        [field: SerializeField] public int BillDenomination { get; private set; }
        [field: SerializeField] public GameObject Bill { get; private set; }
    }
}