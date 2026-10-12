using NaughtyAttributes;
using UnityEngine;

namespace Root {
    [System.Serializable]
    public class PackageItemState : ItemState {
        public int price;
        public float maxDurability;
        public float currentDurability;
        public RenderTexture stampTexture;
        public TypeOfPackage typeOfPackage;
        public bool canBeDelivered = true;
        [SerializeReference] public MapPointsGen.Node DestinationNode;
        [SerializeField, Expandable] private PackageItemGenerationDataSo packageDataGenerator;
        public PackageItemState(PackageItemSo itemSo) : base(itemSo) { }


        [ShowNativeProperty] public Vector2 DestinationNodePosition => new (DestinationNode.dist, DestinationNode.height);
        
        public override string ToString() {
            return $"Price: {price}, MaxDurability: {maxDurability}, Durability: {currentDurability}";
        }

        //TODO: Deterministic number generation
        public void Initialize(MapPointsGen.Node destinationNode) {
            maxDurability = Random.Range(packageDataGenerator.MinDurability, packageDataGenerator.MaxDurability);
            currentDurability = maxDurability;

            price = Random.Range(packageDataGenerator.MinPriceValue, packageDataGenerator.MaxPriceValue);
            DestinationNode = destinationNode;
            stampTexture = PackageStampGenerator.Instance.CreateStampTexture(this);
            typeOfPackage = packageDataGenerator.TypeOfPackage;

            canBeDelivered = true;
        }
    }
}