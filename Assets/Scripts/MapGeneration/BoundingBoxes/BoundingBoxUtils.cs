using Root.Log;
using UnityEngine;
using Logger = Root.Log.Logger;
using LogType = Root.Log.LogType;

namespace Timers {
    public static class BoundingBoxUtils {
        private static readonly Collider[] OverlapResults = new Collider[2];
        
        private static int _layerMask = -1;
        private static int BoundingBoxLayerMask {
            get 
            {
                if (_layerMask == -1)
                {
                    _layerMask = LayerMask.GetMask("BoundingBox");
                }
                return _layerMask;
            }
        }
        
        public static bool TryGetBoundingBoxAtPosition(Vector3 position, out BoundingBox result) {
            float pointRadius = 0.001f;
            
            int hitCount = Physics.OverlapSphereNonAlloc(
                position, 
                pointRadius, 
                OverlapResults, 
                BoundingBoxLayerMask, 
                QueryTriggerInteraction.Collide
            );

            result = null;
            
            for (int i = 0; i < hitCount; i++) {
                Collider col = OverlapResults[i];
                
                if (col.TryGetComponent(out BoundingBox box)) {
                    result = box;
                    break;
                }
                
                Logger.Log("Found collider with BoundingBox layer but no BoundingBox behaviour", LogType.Default, LogSeverity.Warning, col.gameObject);
            }
            
            System.Array.Clear(OverlapResults, 0, hitCount);

            return result != null;
        }
    }
}