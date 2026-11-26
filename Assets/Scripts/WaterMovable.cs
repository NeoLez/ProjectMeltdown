using System.Collections.Generic;
using PrimeTween;
using UnityEngine;
using UnityEngine.Assertions;

namespace Root
{
    public class WaterMovable : MonoBehaviour
    {
        [SerializeField] private float _minimumHeight;
        [SerializeField] private float _maximumHeight;
        [SerializeField] private int currentLevel;
        [SerializeField] private int maximumLevel;
        [SerializeField] private float _time;
        [SerializeField] private List<Pipe> pipes = new();
        [SerializeField] private WaterPump pump;
        [SerializeField] private AudioSource waterMovement;
        public event System.Action<bool> OnRisingChanged;
        public bool IsRising { get; private set; }

        private void SetRising(bool value)
        {
            if (IsRising == value) return;
            IsRising = value;
            OnRisingChanged?.Invoke(value);
        }
        private Tween _currentTween;
        private bool _animating = false;
        private Vector2 _heights;

        private void Awake()
        {
            float baseY = transform.localPosition.y;
            _heights = new Vector2(baseY + _minimumHeight, baseY + _maximumHeight);
            
            if(pump)
                pump.OnActivate += GoDown;
            foreach (var pipe in pipes)
            {
                if (pipe)
                    pipe.OnStateChange += (state) =>
                    {
                        if(state)
                            GoUp();
                    };
            }
            
            SetLocalY(GetLevelHeight(currentLevel));
        }
        private void Start()
        {
            GameManager.AudioSystem.AssignOutputMixerGroup(waterMovement, GameManager.AudioSystem.VFX);
        }

        public void GoUp()
        {
            if (IsMaxed) return;

            currentLevel++;
            _currentTween.Stop();
            SetRising(true);
            MoveTo(GetLevelHeight(currentLevel), true);
        }

        public void GoDown()
        {
            if (IsLow) return;

            currentLevel--;
            _currentTween.Stop();
            SetRising(false);  
            MoveTo(GetLevelHeight(currentLevel), false);
            waterMovement.Play();
        }

        private void SetLocalY(float y)
        {
            Vector3 p = transform.localPosition;
            transform.localPosition = new Vector3(p.x, y, p.z);
        }

        private void MoveTo(float localY, bool rising)
        {
            Vector3 p = transform.localPosition;
            _currentTween = Tween.LocalPosition(transform,
                new TweenSettings<Vector3>(new Vector3(p.x, localY, p.z), _time)).OnComplete(() =>
                {
                    if (rising) SetRising(false);
                    waterMovement.Stop();
                    pump?.StopAudio(true);
                });
        }

        private float GetLevelHeight(int level)
        {
            Debug.Log(_heights[0] + ((_heights[1] - _heights[0]) / maximumLevel) * level, this);
            return _heights[0] + (((_heights[1] - _heights[0]) / maximumLevel) * level);
        }

        private void OnValidate()
        {
            Assert.IsNotNull(pipes);
            foreach (var pipe in pipes)
            {
                Assert.IsTrue(pipe.IsConsumer());
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.aquamarine;
            Gizmos.DrawLine(transform.position + Vector3.up * _minimumHeight, transform.position + Vector3.up * _maximumHeight);
        }
        
        public bool IsLow => currentLevel == 0;
        public bool IsMaxed => currentLevel == maximumLevel;
    }
}