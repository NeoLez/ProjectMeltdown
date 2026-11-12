using PrimeTween;
using UnityEngine;
using UnityEngine.Assertions;

namespace Root
{
    public class WaterMovable : MonoBehaviour
    {
        [SerializeField] private float _minimumHeight;
        [SerializeField] private float _maximumHeight;
        [SerializeField] private bool currentlyLow;
        [SerializeField] private float _time;
        [SerializeField] private Pipe pipe;
        [SerializeField] private WaterPump pump;
        [SerializeField] private AudioSource waterMovement;

        private Tween _currentTween;
        private bool _animating = false;
        private Vector2 _heights;

        private void Awake()
        {
            float baseY = transform.localPosition.y;
            _heights = new Vector2(baseY + _minimumHeight, baseY + _maximumHeight);
            
            if(pump)
                pump.OnActivate += GoDown;
            if (pipe)
                pipe.OnStateChange += (state) =>
                {
                    if(state)
                        GoUp();
                };
            SetLocalY(currentlyLow ? _heights[0] : _heights[1]);
        }
        private void Start()
        {
            GameManager.AudioSystem.AssignOutputMixerGroup(waterMovement, GameManager.AudioSystem.VFX);
        }

        public void GoUp()
        {
            currentlyLow = false;
            _currentTween.Stop();
            MoveTo(_heights[1]);
        }

        public void GoDown()
        {
            currentlyLow = true;
            _currentTween.Stop();
            MoveTo(_heights[0]);
            waterMovement.Play();
        }
        
        private void SetLocalY(float y)
        {
            Vector3 p = transform.localPosition;
            transform.localPosition = new Vector3(p.x, y, p.z);
        }
        
        private void MoveTo(float localY)
        {
            Vector3 p = transform.localPosition;
            _currentTween = Tween.LocalPosition(transform,
                new TweenSettings<Vector3>(new Vector3(p.x, localY, p.z), _time)).OnComplete(() =>
                {
                    waterMovement.Stop();
                    pump.StopAudio(true);
                }); 
        }

        private void OnValidate()
        {
            Assert.IsNotNull(pipe);
            Assert.IsTrue(pipe.IsConsumer());
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.aquamarine;
            Gizmos.DrawLine(transform.position + Vector3.up * _minimumHeight, transform.position + Vector3.up * _maximumHeight);
        }
        
        public bool IsLow => currentlyLow;
    }
}