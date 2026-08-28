using UnityEngine;

namespace Root
{
    public class MerchantHand : MonoBehaviour {
        public Transform objectPivot;
        public Animator _anim;

        public void ShowHand() {
            _anim.Play("Armature_001|PullOut");
        }

        public void HideHand() {
            _anim.Play("Armature_001|PullBack");
        }
    }
}
