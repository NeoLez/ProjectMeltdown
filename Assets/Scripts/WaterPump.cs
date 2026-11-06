using System;
using UnityEngine;

namespace Root
{
    public class WaterPump : InteractableNormalCamera
    {
        [SerializeField] private Pipe pipe;
        [SerializeField] private bool alwaysOn;
        [SerializeField] private float pumpTime;
        [SerializeField] private WaterMovable water;
        public event Action OnActivate;
        
        private void Update()
        {
            if(alwaysOn) 
                SendWater();
        }

        public override void Interact()
        {
            if (alwaysOn || water.IsLow) return;
            SendWater();
        }

        private void SendWater()
        {
            if (pipe.HasFluid()) return;
            var outputAmount = pipe.FindConsumers(null, true);
            if (outputAmount == 0)
            {
                pipe.SendSignal(null, false);
                return;
            };
            
            pipe.SendSignal(null, true);
            OnActivate?.Invoke();
            Invoke(nameof(FlushFluid), pumpTime);
        }

        private void FlushFluid()
        {
            pipe.SendSignal(null, false);
        }
    }
}