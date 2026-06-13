using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using UnityEngine;

namespace Root
{
    public class TanglerBehavior : MonoBehaviour
    {
        [SerializeField] float _radius = 5f;
        [SerializeField] float _SpreadAngle = 200f;
        [SerializeField] float _cooldown = 5f;

        [SerializeField] bool _reproduce = true;
        public int _reproductionChance = 5;
        [SerializeField] int _tangleQuantity = 3;
        bool _maxTangle = false;
        int _tangles = 1;

        public TentaculeBehavior LatchPoint;
        public GameObject TanglerPoint;
        public float spacing = 1.0f;
        LayerMask layerMask;

        public Dictionary<int, List<TentaculeBehavior>> TanglesDictionary = new();

        private void Start()
        {
            layerMask = LayerMask.GetMask("Ground");
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

            if (Physics.Raycast(origin, randomDirection, out hit, _radius, layerMask)) 
            { 
                Debug.DrawLine(origin, hit.point, Color.green, 1f); 
                Spreading();                

                StartCoroutine(SpawnAlongRay(origin, hit.distance, randomDirection));
            }
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

        private IEnumerator SpawnAlongRay(Vector3 origin, float distance, Vector3 direction)
        {
            TanglesDictionary[_tangles] = new();

            int objectCount = Mathf.FloorToInt(distance / spacing);
            Quaternion spawnRotation = Quaternion.LookRotation(direction);
            for (int i = 0; i <= objectCount; i++)
            {
                float currentDistance = i * spacing;

                Vector3 spawnPosition = origin + (direction * currentDistance);
                var Chance = Random.Range(0, _reproductionChance);

                if (objectCount == i && _reproduce && Chance == 0)
                {
                    Instantiate(TanglerPoint, spawnPosition, spawnRotation);
                    _reproduce = false;
                }
                else
                { //TanglesDictionary[_tangles].Add
                    var og = Instantiate(LatchPoint, spawnPosition, spawnRotation, transform);
                    og.TentacleNumber = _tangles;
                    TanglesDictionary[_tangles].Add(og);
                }                                
                yield return new WaitForSeconds(0.05f);
            }
        }

        public void DestroyTangle(int FirstDestroyed)
        {
            _tangles--;
        }
    }
}
