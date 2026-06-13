namespace Root.Enemy {
    public class TanglerTentacleLatch : InteractableNormalCamera {
        public TanglerTentacle tangler;
        
        public int ChainLatchNumber;
        
        public override void Interact() {
            tangler.Cut(ChainLatchNumber);
        }

        public void DestroyInTime(float time) {
            Invoke(nameof(DestroyThingy), time);
        }

        public void DestroyThingy() {
            Destroy(gameObject);
        }
    }
}