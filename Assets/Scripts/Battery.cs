using UnityEngine;

namespace Root {
    public class Battery : MonoBehaviour {
        [SerializeField] private float visualsScale;
        public float energy;
        public float maxEnergy;
        [SerializeField] Animator _animator;

        public void AnimatorOn()
        {
            _animator.SetBool("Insert", true);
        }
    }
}