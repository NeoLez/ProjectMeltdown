using System.Collections;
using UnityEngine;

namespace Root
{
    public class GeneratorDoor : MonoBehaviour
    {
        public enum RotationAxis { X, Y, Z }

        [SerializeField] private GeneratorSlot generatorSlot;
        [SerializeField] private RotationAxis axis = RotationAxis.Y;
        [SerializeField] private float openAngle = 90f;
        [SerializeField] private float smooth = 3f;


        private Quaternion _closedRotation;
        private Quaternion _openRotation;
        private Coroutine _currentRoutine;

        private void Start()
        {
            _closedRotation = transform.localRotation;

            Vector3 eulerAxis = axis switch
            {
                RotationAxis.X => new Vector3(openAngle, 0f, 0f),
                RotationAxis.Z => new Vector3(0f, 0f, openAngle),
                _ => new Vector3(0f, openAngle, 0f),
            };

            _openRotation = _closedRotation * Quaternion.Euler(eulerAxis);

            generatorSlot.OnPowerRestored += HandlePowerRestored;
            generatorSlot.OnPowerLost += HandlePowerLost;
        }

        private void OnDestroy()
        {
            generatorSlot.OnPowerRestored -= HandlePowerRestored;
            generatorSlot.OnPowerLost -= HandlePowerLost;
        }

        private void HandlePowerRestored() => StartMove(_openRotation);
        private void HandlePowerLost() => StartMove(_closedRotation);

        private void StartMove(Quaternion target)
        {
            if (_currentRoutine != null)
                StopCoroutine(_currentRoutine);

            _currentRoutine = StartCoroutine(RotateDoor(target));
        }

        private IEnumerator RotateDoor(Quaternion target)
        {
            while (Quaternion.Angle(transform.localRotation, target) > 0.5f)
            {
                transform.localRotation = Quaternion.Lerp(transform.localRotation, target, smooth * Time.deltaTime);
                yield return null;
            }

            transform.localRotation = target;
            _currentRoutine = null;
        }
    }
}