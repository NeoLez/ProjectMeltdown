using System.Collections.Generic;
using UnityEngine;

namespace Root
{
    public class EnemyFov : MonoBehaviour
    {
        [Header("FOV values")]
        [SerializeField, Range(0.5f, 50f)] float _radiusVision;
        [SerializeField, Range(1, 360)] float _angleVision;
        [SerializeField] LayerMask _myLOSObstacle;

        public bool DetectPlayer(Player player)
        {
            //Si el player esta en mi FOV
            if (InFOV(player.transform.position))
            {
                return true;
            }        
            return false;
        }


        public bool InFOV(Vector3 endPos)
        {
            Vector3 dir = endPos - transform.position;
            if (dir.magnitude > _radiusVision) return false; //Radio
            if (Vector3.Angle(transform.forward, dir) > _angleVision / 2) return false; //Angulo
            if (!InLOS(transform.position, endPos)) return false; //Vision sin obstaculos
            return true;
        }

        public bool InLOS(Vector3 startPos, Vector3 endPos)
        {
            Vector3 dir = endPos - startPos;
            // Debug.DrawRay(startPos, dir, Color.red, dir.magnitude);
            return !Physics.Raycast(startPos, dir.normalized, dir.magnitude, _myLOSObstacle);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _radiusVision);

            Gizmos.color = Color.magenta;
            Vector3 dirA = GetAngleFromDir(_angleVision / 2 + transform.eulerAngles.y);
            Vector3 dirB = GetAngleFromDir(-_angleVision / 2 + transform.eulerAngles.y);
            Gizmos.DrawLine(transform.position, transform.position + dirA.normalized * _radiusVision);
            Gizmos.DrawLine(transform.position, transform.position + dirB.normalized * _radiusVision);
        }

        Vector3 GetAngleFromDir(float angleInDegrees)
        {
            return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
        }
    }
}
