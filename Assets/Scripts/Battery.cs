using UnityEngine;

namespace Root {
    public class Battery : MonoBehaviour {
        [SerializeField] private float visualsScale;
        public float energy;
        public float maxEnergy;
        [SerializeField] Animator _animator;

        private void Start()
        {
            
        }

        public void AnimatorOn()
        {
            _animator.Play("Insert");
            
        }
    }
}