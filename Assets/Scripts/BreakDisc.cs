using UnityEngine;

namespace Root
{
    public class BreakDisc : MonoBehaviour
    {
        public int DiscUsage = 3;
        //[SerializeField] Animator _animator;
        /*public int GetDiscUsage()
        {
            return(DiscUsage);  
        }*/
        public void SetDiscUsage()
        {
            if (DiscUsage <= 0) return;
            DiscUsage--;
        }
    }
}
