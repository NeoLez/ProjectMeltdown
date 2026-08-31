using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Root.Enemy
{
    public class TanglerTentacle : MonoBehaviour
    {
        public float spacing = 1.0f;
        [Range(0, 1)] public float reproductionChance;
        public Tangler tanglerPoint;
        public bool reproduce = true;
        public TanglerTentacleLatch latchPoint;
        public event Action onCut;
        public float tanglerSpawnDistanceOffset;

        private List<TanglerTentacleLatch> _tanglerTentacleLatches = new();
        private bool _isCut;
        private bool _isGenerating = true;

        private AudioClip _soundLatch;
        private AudioClip _soundDestroy;

        public void SetSounds(AudioClip latch, AudioClip destroy)
        {
            _soundLatch = latch;
            _soundDestroy = destroy;
        }

        public void Spawn(Transform origin, float distance, Vector3 direction)
        {
            StartCoroutine(SpawnAlongRay(origin, distance, direction));
        }

        private IEnumerator SpawnAlongRay(Transform origin, float distance, Vector3 direction)
        {
            int objectCount = Mathf.CeilToInt(distance / spacing);
            Quaternion spawnRotation = Quaternion.LookRotation(direction);
            for (int i = 0; i <= objectCount; i++)
            {
                float currentDistance = i * spacing;
                Vector3 spawnPosition = origin.position + (direction * currentDistance);

                if (reproduce && objectCount == i)
                {
                    AttemtReproduction(origin.position + direction * (distance - tanglerSpawnDistanceOffset));
                }
                else
                {
                    var obj = Instantiate(latchPoint, spawnPosition, spawnRotation, transform);
                    obj.ChainLatchNumber = i;
                    obj.tangler = this;
                    obj.SetSounds(_soundLatch, i == 0);
                    _tanglerTentacleLatches.Add(obj);
                }
                yield return new WaitForSeconds(0.05f);
            }

            _isGenerating = false;
        }

        private void AttemtReproduction(Vector3 spawnPosition)
        {
            var shouldReproduce = Random.Range(0.0f, 1.0f) <= reproductionChance;
            if (!shouldReproduce) return;

            Instantiate(tanglerPoint, spawnPosition, Quaternion.identity, transform.parent.parent);
            reproduce = false;
        }

        public void Cut(int latchNumber)
        {
            if (IsDead()) return;
            _isGenerating = false;
            _isCut = true;
            StopAllCoroutines();

            _tanglerTentacleLatches[latchNumber].DestroyInTime(0);

            int uwu = 1;
            for (int i = latchNumber + 1; i < _tanglerTentacleLatches.Count; i++)
            {
                _tanglerTentacleLatches[i].DestroyInTime(0.05f * uwu);
                uwu++;
            }

            uwu = 1;
            for (int i = latchNumber - 1; i >= 0; i--)
            {
                _tanglerTentacleLatches[i].DestroyInTime(0.05f * uwu);
                uwu++;
            }

            if (_soundDestroy != null)
                GameManager.AudioSystem.PlaySoundPositional(_soundDestroy, transform.position, GameManager.AudioSystem.VFX);

            GetComponent<BloodSplatter>()?.SpawnBlood(_tanglerTentacleLatches[latchNumber].transform.position);
            onCut?.Invoke();
        }

        public bool IsDead()
        {
            return _isCut;
        }

        public bool IsGenerating => _isGenerating;
    }
}