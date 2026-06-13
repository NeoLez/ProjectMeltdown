using System.Collections;
using UnityEngine;

namespace Root
{
    public class TanglerBehavior : MonoBehaviour
    {
        [SerializeField] float _radius = 5f;
        [SerializeField] float _SpreadAngle = 200f;
        [SerializeField] float _cooldown = 5f;
        bool _maxTangle = false;
        [SerializeField] int _tangleQuantity = 3;
        int _tangles = 0;

        private void Start()
        {
            StartCoroutine(Spread());
        }
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, _radius);
        }

        IEnumerator Spread()
        {
            while (!_maxTangle)
            {
                Debug.Log("Loop");
                FireRandomRay();
                yield return new WaitForSeconds(_cooldown);
            }

            Debug.Log("Max tangle, end loop");
        }


        void FireRandomRay()
        {
            Vector3 baseDirection = transform.forward;

            float randomPitch = Random.Range(-_SpreadAngle, _SpreadAngle);
            float randomYaw = Random.Range(-_SpreadAngle, _SpreadAngle);
            Quaternion spreadRotation = Quaternion.Euler(randomPitch, randomYaw, 0);

            Vector3 randomDirection = spreadRotation * baseDirection;

            RaycastHit hit;
            Vector3 origin = transform.position;

            if (Physics.Raycast(origin, randomDirection, out hit, _radius)) { Debug.DrawLine(origin, hit.point, Color.green, 1f); Spreading(); }
            else { Debug.DrawRay(origin, randomDirection * _radius, Color.red, 1f); }
        }

        private void Spreading()
        {
            if (_tangles != _tangleQuantity)
            {
                _tangles++;
            }
            else { _maxTangle = true; }   
            
        }
    }
}
