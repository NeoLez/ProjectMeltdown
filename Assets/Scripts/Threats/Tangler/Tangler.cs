using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Root.Enemy
{
    public class Tangler : InteractableNormalCamera
    {
        [SerializeField] float _maxRadius = 10f;
        [SerializeField] float _minRadius = 2f;
        [SerializeField] GameObject _visuals;
        [SerializeField] GameObject _particles;
        [SerializeField] GameObject _particlesHit;
        [SerializeField] TanglerTentacle tentaclePrefab;
        [SerializeField] int maxTentacleQuantity = 3;
        [SerializeField] LayerMask layerMask;

        private Dictionary<int, TanglerTentacle> _tentacles = new();
        private int _tentacleAmount;
        private int _tentacleID;
        [SerializeField] private int _health = 3;
        private bool _isDead;

        [SerializeField] float tentacleSpawnCooldown = 15f;
        private float _lastTentacleSpawnTime;
        Animator _animator;

        [Header("Sounds")]
        [SerializeField] private AudioClip _soundDamage;
        [SerializeField] private AudioClip _soundDeath;
        [SerializeField] private AudioClip _soundIdle;
        [SerializeField] private AudioClip _soundLatch;
        [SerializeField] private AudioClip _soundDestroy;

        private AudioSource _idleLoop;

        private void Awake()
        {
            _lastTentacleSpawnTime = Time.time;
            _animator = _visuals.GetComponent<Animator>();
        }

        private void Start()
        {
            if (_soundIdle != null) 
            {
                GameObject idleGO = new GameObject("IdleLoop");
                idleGO.transform.SetParent(transform);
                idleGO.transform.localPosition = Vector3.zero;

                _idleLoop = idleGO.AddComponent<AudioSource>();
                _idleLoop.clip = _soundIdle;
                _idleLoop.loop = true;
                _idleLoop.spatialBlend = 1f;
                _idleLoop.maxDistance = 50f;
                _idleLoop.rolloffMode = AudioRolloffMode.Linear;
                _idleLoop.outputAudioMixerGroup = GameManager.AudioSystem.VFX;
                _idleLoop.Play();
            }
        }

        private void Update()
        {
            if (Time.time >= _lastTentacleSpawnTime + tentacleSpawnCooldown)
            {
                AttemptTentacleSpawn();
            }
        }

        private void AttemptTentacleSpawn()
        {
            if (!CanSpawn()) return;

            Vector3 origin = transform.position;
            Vector3 randomDirection = Random.insideUnitSphere.normalized;

            if (Physics.Raycast(origin, randomDirection, out var hit, _maxRadius, layerMask) && hit.distance >= _minRadius)
            {
                Debug.DrawLine(origin, hit.point, Color.green, 1f);

                var tentacle = Instantiate(tentaclePrefab, transform.position, Quaternion.identity, transform);
                _tentacles[_tentacleID] = tentacle;
                _lastTentacleSpawnTime = Time.time;
                _tentacleAmount++;
                _tentacleID++;

                tentacle.onCut += HandleOnTentacleCut;
                tentacle.SetSounds(_soundLatch, _soundDestroy);
                tentacle.Spawn(transform, hit.distance, randomDirection);
            }
            else
            {
                Debug.DrawRay(origin, randomDirection * _maxRadius, Color.red, 1f);
            }
        }

        public bool CanSpawn()
        {
            return !_isDead && (_tentacleAmount < maxTentacleQuantity);
        }

        public override void Interact()
        {
            if (_isDead) return;
            if (_health <= 0)
            {
                foreach (var tentacle in _tentacles)
                {
                    if (tentacle.Value.IsDead()) continue;
                    tentacle.Value.Cut(0);
                }

                _isDead = true;

                foreach (Collider collider in GetComponentsInChildren<Collider>()){
                    collider.enabled = false;
                }
                GetComponent<BloodSplatter>()?.SpawnBlood(transform.position);
                _visuals.SetActive(false);
                Instantiate(_particles, transform.position, Quaternion.identity, transform);

                if (_idleLoop != null) _idleLoop.Stop();
                if (_soundDeath != null) GameManager.AudioSystem.PlaySoundPositional(_soundDeath, transform.position, GameManager.AudioSystem.VFX);

                Invoke(nameof(Destroy), 5f);
            }
            else
            {
                _animator.Play("Damage", -1, 0f);
                Instantiate(_particlesHit, transform.position, transform.rotation);
                if (_soundDamage != null) GameManager.AudioSystem.PlaySoundPositional(_soundDamage, transform.position, GameManager.AudioSystem.VFX);
                _health--;
            }
        }

        private void HandleOnTentacleCut()
        {
            _lastTentacleSpawnTime = Time.time;
            _tentacleAmount--;
        }
    }
}