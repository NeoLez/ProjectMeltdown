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
        
        private Tween _currentTween;
        private bool _animating = false;
        private Vector2 _heights;

        private void Awake()
        {
            _heights = new Vector2(transform.position.y + _minimumHeight, transform.position.y + _maximumHeight);
            if(pump)
                pump.OnActivate += GoDown;
            if(currentlyLow) 
            {
                transform.position = new Vector3(transform.position.x, _heights[0], transform.position.z);
            }
            else
            {
                transform.position = new Vector3(transform.position.x, _heights[1], transform.position.z);
            }
        }


        private void Update()
        {
            if (!_currentTween.isAlive && pipe.HasFluid())
            {
                GoUp();
            }
        }

        public void GoUp()
        {
            _currentTween.Stop();
            transform.position = new Vector3(transform.position.x, _heights[0], transform.position.z);
            Tween.Position(transform, new TweenSettings<Vector3>(new Vector3(transform.position.x, _heights[1], transform.position.z), _time));
        }
        
        public void GoDown()
        {
            _currentTween.Stop();
            transform.position = new Vector3(transform.position.x, _heights[1], transform.position.z);
            Tween.Position(transform, new TweenSettings<Vector3>(new Vector3(transform.position.x, _heights[0], transform.position.z), _time));
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
    }
}