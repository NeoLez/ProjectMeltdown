using System;
using System.Collections.Generic;
using UnityEngine;

namespace Root {
    public class BrakeDiscItem : PhysicalItem {
        public ItemConsumableState State => itemState as ItemConsumableState;
        [SerializeField] private List<GameObject> visualStages;

        public override void StateUpdate() {
            ChangeModel(State.usesLeft);
        }

        public int GetDiscUsage()
        {
            return State.usesLeft;  
        }

        public void SetDiscUsage()
        {
            if (State.usesLeft <= 0) return;
            State.usesLeft--;
            ChangeModel(State.usesLeft);
        }

        private void ChangeModel(int a)
        {
            for (int i = 0; i < 2; i++)
            {
                if(a != i)
                {
                    visualStages[i].SetActive(false);
                }
            }
            visualStages[a > visualStages.Count - 1 ? visualStages.Count - 1 : a].SetActive(true);
        }
        
        protected override bool IsStateTypeValid(ItemState state) {
            return state is ItemConsumableState;
        } 
    }
}