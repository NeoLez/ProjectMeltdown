using System;
using System.Collections;
using UnityEngine;

namespace Root
{
    public class TollDoor : MonoBehaviour
    {
        [SerializeField] private float smooth = 8f;
        [SerializeField] private Quaternion openRotation;

        public void OpenTollBarrier()
        {
            StartCoroutine(Open(openRotation));
        }
        private IEnumerator Open(Quaternion target)
        {
            while (Quaternion.Angle(transform.localRotation, target) > 0.5f)
            {
                transform.localRotation = Quaternion.Lerp(transform.localRotation, target, smooth * Time.deltaTime);
                yield return null;
            }

            transform.localRotation = target;
        }

    }
}
