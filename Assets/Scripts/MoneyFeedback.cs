using System.Collections;
using UnityEngine;

namespace Root
{
    public class MoneyFeedback : MonoBehaviour
    {
        Coroutine activeRoutine;
        public static MoneyFeedback Instance;
        [SerializeField] GameObject _bill;
        Transform _transform;
        Vector3 _offset;
        int _same;

        private void Awake()
        {
            _transform = GetComponent<Transform>();
            Instance = this;
            _offset = Vector3.zero + transform.position;
        }

        public void GrabbedBill()
        {
            _same++;
            Instantiate(_bill, _offset, Quaternion.identity, _transform);
            StartProcess();
            _offset += new Vector3(0, 100, 0);
        }
        void StartProcess()
        {
            if (activeRoutine != null)
            {
                StopCoroutine(activeRoutine);
            }
            activeRoutine = StartCoroutine(FeedbackTime(_same));
        }
        IEnumerator FeedbackTime(int a)
        {
            for (int i = 0; i < a; i++)             
            {
                a--;
                yield return new WaitForSeconds(1f);
                activeRoutine = null;
            }
            _offset = Vector3.zero + transform.position;
        }
    }
}
