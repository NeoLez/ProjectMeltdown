using System;
using System.Collections.Generic;
using UnityEngine;

namespace Root
{
    public class Pipe : MonoBehaviour
    {
        public List<Pipe> connections = new();
        protected bool _currentState;
        public event Action<bool> OnStateChange;
        [SerializeField] private bool fluidConsumer;
        [SerializeField, NaughtyAttributes.EnableIf("fluidConsumer")] private WaterMovable water;

        public void SendSignal(Pipe pipe, bool sendingFluid)
        {
            if (sendingFluid == _currentState) return;
            
            _currentState = sendingFluid;
            OnStateChange?.Invoke(sendingFluid);
            
            foreach (var connection in connections)
            {
                if (connection == pipe) continue;
                
                connection.SendSignal(this, sendingFluid);
            }
        }

        public int FindConsumers(Pipe pipe, bool sendingFluid)
        {
            if (sendingFluid == _currentState) return 0;
            
            int numConsumers = 0;
            if (fluidConsumer && (!water || water.IsLow))
            {
                numConsumers = 1;
            }
            
            foreach (var connection in connections)
            {
                if (connection == pipe) continue;
                
                numConsumers += connection.FindConsumers(this, sendingFluid);
            }
            return numConsumers;
        }

        public bool HasFluid()
        {
            return _currentState;
        }
        
        public bool IsConsumer()
        {
            return fluidConsumer;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.darkCyan;
            foreach (var connection in connections)
            {
                Gizmos.DrawLine(transform.position, connection.transform.position);
            }
        }
    }
}
