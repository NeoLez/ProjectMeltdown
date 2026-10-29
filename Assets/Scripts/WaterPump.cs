using UnityEngine;

namespace Root
{
    public class WaterPump : InteractableNormalCamera
    {
        [SerializeField] private Pipe pipe;
        [SerializeField] private bool alwaysOn;

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
            var outputAmount = pipe.SendSignal(null, true);
            if (outputAmount == 0) pipe.SendSignal(null, false);
        }
    }
}