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

        public int SendSignal(Pipe pipe, bool sendingFluid)
        {
            if (sendingFluid == _currentState) return 0;
            _currentState = sendingFluid;
            OnStateChange?.Invoke(sendingFluid);
            
            int numConsumers = fluidConsumer ? 1 : 0;
            foreach (var connection in connections)
            {
                if (connection == pipe) continue;
                
                numConsumers += connection.SendSignal(pipe, sendingFluid);
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
