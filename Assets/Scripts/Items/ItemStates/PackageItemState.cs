using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace Root {
    [System.Serializable]
    public class PackageItemState : ItemState {
        public int price;
        [FormerlySerializedAs("durability")] public float maxDurability;
        public float currentDurability;
        public RenderTexture stampTexture;
        public TypeOfPackage typeOfPackage;
        public bool canBeDelivered = true;
        [SerializeReference] public MapPointsGen.Node DestinationNode;
        [SerializeField, Expandable] private PackageItemGenerationDataSo packageDataGenerator;
        public PackageItemState(PackageItemSo itemSo) : base(itemSo) { }


        [ShowNativeProperty] public Vector2 DestinationNodePosition => new (DestinationNode.dist, DestinationNode.height);
        
        public override string ToString() {
            return $"MaxCharge: {price}, CurrentCharge: {maxDurability}";
        }

        //TODO: Deterministic number generation
        public void Initialize(MapPointsGen.Node destinationNode) {
            maxDurability = Random.Range(packageDataGenerator.MinDurability, packageDataGenerator.MaxDurability);
            currentDurability = maxDurability;

            price = Random.Range(packageDataGenerator.MinPriceValue, packageDataGenerator.MaxPriceValue);
            stampTexture = PackageStampGenerator.Instance.CreateStampTexture(this);
            typeOfPackage = packageDataGenerator.TypeOfPackage;
            DestinationNode = destinationNode;

            canBeDelivered = true;
        }

        public override ItemState Clone() {
            var clone = new PackageItemState(ItemSo as PackageItemSo) {
                price = price,
                maxDurability = maxDurability,
                typeOfPackage = typeOfPackage,
                canBeDelivered = canBeDelivered,
                DestinationNode = DestinationNode,
                packageDataGenerator = packageDataGenerator,
                //TODO: Should the texture also be cloned?
            };
            return clone;
        }
    }
}