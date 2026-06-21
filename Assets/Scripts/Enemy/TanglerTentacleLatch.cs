using UnityEngine;

namespace Root.Enemy {
    public class TanglerTentacleLatch : InteractableNormalCamera {
        public TanglerTentacle tangler;
        [SerializeField] GameObject _particles;
        public int ChainLatchNumber;
        bool _once = true;
        public override void Interact() {
            tangler.Cut(ChainLatchNumber);
        }

        public void DestroyInTime(float time) {
            Invoke(nameof(DestroyThingy), time);
        }

        public void DestroyThingy() 
        {
            Instantiate(_particles, transform.position, transform.rotation);
            Destroy(gameObject);
        }
        private void OnTriggerEnter(Collider other)
        {            
            if (other.CompareTag("Player") || _once)
            {             
                    var PJ = other.GetComponent<HealthControl>();
                    PJ.TakeDamage(35f);
                    Interact();
                _once = false;
            }
        }
    }
}