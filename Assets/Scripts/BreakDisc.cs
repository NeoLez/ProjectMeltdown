using System.Collections.Generic;
using UnityEngine;

namespace Root
{
    public class BreakDisc : MonoBehaviour
    {
        [SerializeField] private int DiscUsage = 3;
        [SerializeField] List<GameObject> _object;
        //[SerializeField] Animator _animator;

        public int GetDiscUsage()
        {
            return DiscUsage;  
        }

        public void SetDiscUsage()
        {
            if (DiscUsage <= 0) return;
            DiscUsage--;
            ChangeModel(DiscUsage);
        }

        private void ChangeModel(int a)
        {
             for (int i = 0; i < 2; i++)
             {
                if(a != i)
                {
                    _object[i].SetActive(false);
                }
             }
            _object[a].SetActive(true);
        }
    }
}
