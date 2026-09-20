using System.Data;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
namespace Root
{
    public class MoveMonsterIK : MonoBehaviour
    {
        Rig _ikRig = null;
         TwoBoneIKConstraint constraint;
        [SerializeField] Transform IKposition = null;
        [SerializeField] GameObject _origin;
        [SerializeField] float _moveSpeed = 15f;
        float _TimePerStep = 0;
        [SerializeField] float _pace = 0.5f;
        [SerializeField] float _maxRadius = 10f;
        [SerializeField] float _minRadius = 2f;
        Vector3 _hit;
        Vector3 _current;
        Vector3 _target;
        private float sinTime;
        bool _move = false;
        Animator animator;
        private void Start()
        {
            StepAttempt();
            animator = GetComponent<Animator>();
            //OnDrawGizmosSelected();
        }
        private void Update()
        {
                if (_TimePerStep <= 0) StepAttempt();
                else _TimePerStep = _TimePerStep - 1 * Time.deltaTime;

            if (_current != _target)
            {
                sinTime += Time.deltaTime * _moveSpeed;
                sinTime = Mathf.Clamp(sinTime, 0, Mathf.PI);
                //constraint.weight = Mathf.Clamp(sinTime, 0, Mathf.PI);
                float t = evaluate(sinTime);
                IKposition.position = Vector3.Lerp(_current, _target, t);
            }            
        }
        private float evaluate(float x)
        {
            return 0.5f * Mathf.Sin(x - Mathf.PI / 2f) + 0.5f;
        }
        private void StepAttempt()
        {
            Vector3 origin = _origin.transform.position;
            Vector3 randomDirection = Random.insideUnitSphere.normalized;

            if (Physics.Raycast(origin, randomDirection, out var hit, _maxRadius) && hit.distance >= _minRadius)
            {
                Debug.DrawLine(origin, hit.point, Color.green, 1f);
                //animator.SetIKRotation(AvatarIKGoal.RightHand, Quaternion.FromToRotation(transform.forward,hit.normal));
                sinTime = 0;
                _target = hit.point;
                _current = IKposition.position;
                _TimePerStep = _pace;
            }
            else StepAttempt();
        }
        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;

            Gizmos.DrawWireSphere(_origin.transform.position, _maxRadius);
        }
    }
}
