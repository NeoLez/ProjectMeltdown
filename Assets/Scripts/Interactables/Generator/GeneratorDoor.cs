using System.Collections;
using UnityEngine;

namespace Root
{
    public class GeneratorDoor : MonoBehaviour
    {
        public enum DoorMode { Rotate, MoveUp, MoveDown }
        public enum RotationAxis { X, Y, Z }

        [SerializeField] private GeneratorSlot generatorSlot;
        [SerializeField] private DoorMode mode = DoorMode.Rotate;
        [SerializeField] private RotationAxis axis = RotationAxis.Y;
        [SerializeField] private float openAngle = 90f;

        [Header("Movimiento")]
        [SerializeField] private float moveDistance = 3f;

        [Header("General")]
        [SerializeField] private float smooth = 3f;

        [Header("Sounds")]
        [SerializeField] private AudioClip _soundOpen;
        [SerializeField] private AudioClip _soundClose;

        private Quaternion _closedRotation;
        private Quaternion _openRotation;
        private Vector3 _closedPosition;
        private Vector3 _openPosition;
        private Coroutine _currentRoutine;

        private void Start()
        {
            generatorSlot.OnPowerRestored += HandlePowerRestored;
            generatorSlot.OnPowerLost += HandlePowerLost;

            if (mode == DoorMode.Rotate)
            {
                _closedRotation = transform.localRotation;

                Vector3 eulerAxis = axis switch
                {
                    RotationAxis.X => new Vector3(openAngle, 0f, 0f),
                    RotationAxis.Z => new Vector3(0f, 0f, openAngle),
                    _ => new Vector3(0f, openAngle, 0f),
                };

                _openRotation = _closedRotation * Quaternion.Euler(eulerAxis);
            }
            else
            {
                _closedPosition = transform.localPosition;
                Vector3 dir = mode == DoorMode.MoveUp ? Vector3.up : Vector3.down;
                _openPosition = _closedPosition + dir * moveDistance;
            }
        }

        private void OnDestroy()
        {
            generatorSlot.OnPowerRestored -= HandlePowerRestored;
            generatorSlot.OnPowerLost -= HandlePowerLost;
        }

        private void HandlePowerRestored()
        {
            if (_currentRoutine != null) StopCoroutine(_currentRoutine);
            if (_soundOpen != null)
                GameManager.AudioSystem.PlaySoundPositional(_soundOpen, transform.position, GameManager.AudioSystem.VFX);
            _currentRoutine = mode == DoorMode.Rotate
                ? StartCoroutine(RotateDoor(_openRotation))
                : StartCoroutine(MoveDoor(_openPosition));
        }

        private void HandlePowerLost()
        {
            if (_currentRoutine != null) StopCoroutine(_currentRoutine);
            if (_soundClose != null)
                GameManager.AudioSystem.PlaySoundPositional(_soundClose, transform.position, GameManager.AudioSystem.VFX);
            _currentRoutine = mode == DoorMode.Rotate
                ? StartCoroutine(RotateDoor(_closedRotation))
                : StartCoroutine(MoveDoor(_closedPosition));
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

        private IEnumerator MoveDoor(Vector3 target)
        {
            while (Vector3.Distance(transform.localPosition, target) > 0.01f)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, target, smooth * Time.deltaTime);
                yield return null;
            }
            transform.localPosition = target;
            _currentRoutine = null;
        }
    }
}