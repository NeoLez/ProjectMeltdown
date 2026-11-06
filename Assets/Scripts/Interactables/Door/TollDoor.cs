using System.Collections;
using UnityEngine;

namespace Root
{
    public class TollDoor : MonoBehaviour
    {
        [SerializeField] private float openHeight = 3f;
        [SerializeField] private float speed = 2f;

        private Vector3 _closedPosition;
        private Coroutine _routine;

        private void Awake()
        {
            _closedPosition = transform.localPosition;
        }

        public void OpenTollBarrier()
        {
            if (_routine != null) StopCoroutine(_routine);
            _routine = StartCoroutine(Open(_closedPosition + Vector3.up * openHeight));
        }

        private IEnumerator Open(Vector3 target)
        {
            while (transform.localPosition != target)
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, target, speed * Time.deltaTime);
                yield return null;
            }

            _routine = null;
        }
    }
}