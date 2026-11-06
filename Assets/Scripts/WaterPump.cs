using System;
using Timers;
using UnityEngine;

namespace Root
{
    public class WaterPump : InteractableNormalCamera
    {
        [SerializeField] private Pipe pipe;
        [SerializeField] private bool alwaysOn;
        [SerializeField] private float pumpTime;
        public event Action OnActivate;
        
        private void Update()
        {
            if(alwaysOn)
                SendWater();
        }

        public override void Interact()
        {
            if (alwaysOn) return;
            SendWater();
        }

        private void SendWater()
        {
            if (pipe.HasFluid()) return;
            var outputAmount = pipe.SendSignal(null, true);
            if (outputAmount == 0) pipe.SendSignal(null, false);
            
            Invoke(nameof(FlushFluid), pumpTime);
        }

        private void FlushFluid()
        {
            pipe.SendSignal(null, false);
        }
    }
}