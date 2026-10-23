using System;
using System.Collections.Generic;
using UnityEngine;

namespace Root
{
    public class Pipe : MonoBehaviour
    {
        private List<Pipe> _connections = new();
        private bool _currentState;
        public event Action<bool> OnStateChange;

        public void SendSignal(Pipe pipe, bool sendingFluid)
        {
            if (sendingFluid == _currentState) return;
            _currentState = sendingFluid;
            
            foreach (var connection in _connections)
            {
                if (connection == pipe) continue;
                
                connection.SendSignal(pipe, sendingFluid);
            }
            
            OnStateChange?.Invoke(sendingFluid);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.darkCyan;
            foreach (var connection in _connections)
            {
                Gizmos.DrawLine(transform.position, connection.transform.position);
            }
        }
    }
}
