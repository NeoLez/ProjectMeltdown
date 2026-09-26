using System.Collections;
using UnityEngine;

namespace Root
{
    public class StationDoor : InteractableNormalCamera
    {
        public enum ControlMode { Manual, Automatic }
        public enum DoorMode { Rotate, MoveUp, MoveDown }
        public enum RotationAxis { X, Y, Z }
        public enum PowerLossBehavior { Deactivate, MaintainState }
        private enum DoorState { Closed, Opening, Open, Closing }

        [Header("Tipo de puerta")]
        [SerializeField] private ControlMode controlMode = ControlMode.Automatic;

        [Header("Electricidad")]
        [SerializeField] private GeneratorSlot generatorSlot;
        [SerializeField] private bool requiresPowerToInteract = false;
        [SerializeField] private PowerLossBehavior onPowerLost = PowerLossBehavior.Deactivate;

        [Header("Movimiento")]
        [SerializeField] private DoorMode mode = DoorMode.Rotate;
        [SerializeField] private RotationAxis axis = RotationAxis.Y;
        [SerializeField] private float openAngle = 90f;
        [SerializeField] private float moveDistance = 3f;

        [Header("Velocidad de animacion")]
        [SerializeField] private float smooth = 3f;

        [Header("Sounds")]
        [SerializeField] private AudioClip _soundOpen;
        [SerializeField] private AudioClip _soundClose;
        [SerializeField] private AudioClip _soundDenied;

        [Header("Feedback de bloqueo")]
        [SerializeField] private float shakeDuration = 0.4f;
        [SerializeField] private float shakeMagnitude = 0.06f;

        private DoorState _state = DoorState.Closed;
        private bool _stationPowered;
        private bool _isShaking;

        private Quaternion _closedRotation;
        private Quaternion _openRotation;
        private Vector3 _closedPosition;
        private Vector3 _openPosition;
        private Coroutine _currentRoutine;

        public bool IsOpen => _state == DoorState.Open;

        private void Start()
        {
            if (generatorSlot != null)
            {
                generatorSlot.OnPowerRestored += HandlePowerRestored;
                generatorSlot.OnPowerLost += HandlePowerLost;
            }

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
            if (generatorSlot != null)
            {
                generatorSlot.OnPowerRestored -= HandlePowerRestored;
                generatorSlot.OnPowerLost -= HandlePowerLost;
            }
        }

        public override void ShowFeedback(bool canShow)
        {
            base.ShowFeedback(controlMode == ControlMode.Manual && canShow);
        }

        public override void Interact()
        {
            if (controlMode != ControlMode.Manual) return;

            if (requiresPowerToInteract && !_stationPowered)
            {
                if (_state == DoorState.Closed || _state == DoorState.Open)
                    RequestDeniedFeedback();
                return;
            }

            if (_state == DoorState.Closed) RequestOpen();
            else if (_state == DoorState.Open) RequestClose();
        }

        private void HandlePowerRestored()
        {
            _stationPowered = true;
            ReconcilePowerState();
        }

        private void HandlePowerLost()
        {
            _stationPowered = false;
            ReconcilePowerState();
        }

        private void ReconcilePowerState()
        {
            if (generatorSlot == null) return;

            if (controlMode == ControlMode.Automatic)
            {
                if (_stationPowered)
                    RequestOpen();
                else if (onPowerLost == PowerLossBehavior.Deactivate)
                    RequestClose();
            }
            else
            {
                if (requiresPowerToInteract && !_stationPowered && onPowerLost == PowerLossBehavior.Deactivate)
                    RequestClose();
            }
        }

        private void RequestOpen()
        {
            if (_state == DoorState.Opening || _state == DoorState.Closing) return;
            if (_state == DoorState.Open) return;

            _state = DoorState.Opening;
            if (_soundOpen != null)
                GameManager.AudioSystem.PlaySoundPositional(_soundOpen, transform.position, GameManager.AudioSystem.VFX);

            _currentRoutine = mode == DoorMode.Rotate
                ? StartCoroutine(RotateDoor(_openRotation))
                : StartCoroutine(MoveDoor(_openPosition));
        }

        private void RequestClose()
        {
            if (_state == DoorState.Opening || _state == DoorState.Closing) return;
            if (_state == DoorState.Closed) return;

            _state = DoorState.Closing;
            if (_soundClose != null)
                GameManager.AudioSystem.PlaySoundPositional(_soundClose, transform.position, GameManager.AudioSystem.VFX);

            _currentRoutine = mode == DoorMode.Rotate
                ? StartCoroutine(RotateDoor(_closedRotation))
                : StartCoroutine(MoveDoor(_closedPosition));
        }

        private void RequestDeniedFeedback()
        {
            if (_isShaking) return;

            if (_soundDenied != null)
                GameManager.AudioSystem.PlaySoundPositional(_soundDenied, transform.position, GameManager.AudioSystem.VFX);

            StartCoroutine(ShakeFeedback());
        }

        private IEnumerator ShakeFeedback()
        {
            _isShaking = true;
            Vector3 basePosition = transform.localPosition;
            float elapsed = 0f;

            while (elapsed < shakeDuration)
            {
                elapsed += Time.deltaTime;
                float damper = 1f - Mathf.Clamp01(elapsed / shakeDuration);
                transform.localPosition = basePosition + Random.insideUnitSphere * shakeMagnitude * damper;
                yield return null;
            }

            transform.localPosition = basePosition;
            _isShaking = false;
        }

        private IEnumerator RotateDoor(Quaternion target)
        {
            while (Quaternion.Angle(transform.localRotation, target) > 0.5f)
            {
                transform.localRotation = Quaternion.Lerp(transform.localRotation, target, smooth * Time.deltaTime);
                yield return null;
            }
            transform.localRotation = target;
            OnTransitionComplete();
        }

        private IEnumerator MoveDoor(Vector3 target)
        {
            while (Vector3.Distance(transform.localPosition, target) > 0.01f)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, target, smooth * Time.deltaTime);
                yield return null;
            }
            transform.localPosition = target;
            OnTransitionComplete();
        }

        private void OnTransitionComplete()
        {
            _currentRoutine = null;
            _state = _state == DoorState.Opening ? DoorState.Open : DoorState.Closed;
            ReconcilePowerState();
        }
    }
}