using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Root
{
    public class DoorButton : InteractableNormalCamera
    {
        [Header("Puertas controladas")]
        [SerializeField] private List<StationDoor> doors = new List<StationDoor>();

        [Header("Electricidad")]
        [SerializeField] private GeneratorSlot generatorSlot;
        [SerializeField] private bool requiresPower = true;

        [Header("Palanca")]
        [SerializeField] private StationDoor.RotationAxis axis = StationDoor.RotationAxis.X;
        [SerializeField] private float leverAngle = 60f;
        [SerializeField] private float smooth = 8f;
        [SerializeField, Range(0f, 1f)] private float deniedTravel = 0.25f;

        [Header("Sonidos")]
        [SerializeField] private AudioClip _soundPress;
        [SerializeField] private AudioClip _soundDenied;

        private Quaternion _upRotation;
        private Quaternion _downRotation;
        private bool _leverDown;
        private bool _isMoving;

        private bool HasPower => !requiresPower || (generatorSlot != null && generatorSlot.IsPowered());

        private void Start()
        {
            _upRotation = transform.localRotation;

            Vector3 eulerAxis = axis switch
            {
                StationDoor.RotationAxis.Y => new Vector3(0f, leverAngle, 0f),
                StationDoor.RotationAxis.Z => new Vector3(0f, 0f, leverAngle),
                _ => new Vector3(leverAngle, 0f, 0f),
            };

            _downRotation = _upRotation * Quaternion.Euler(eulerAxis);
        }

        private void Update()
        {
            SyncLever();
        }

        public override void ShowFeedback(bool canShow)
        {
            base.ShowFeedback(canShow && HasPower && HasUsableDoors());
        }

        public override void Interact()
        {
            if (_isMoving) return;

            if (!HasPower)
            {
                StartCoroutine(DeniedNudge());
                return;
            }

            int toggled = 0;
            foreach (StationDoor door in doors)
            {
                if (door != null && door.TryToggleFromButton())
                    toggled++;
            }

            if (toggled > 0)
            {
                PlaySound(_soundPress);
                SyncLever();
            }
        }

        private void SyncLever()
        {
            if (_isMoving) return;

            bool shouldBeDown = AnyDoorOpen();
            if (shouldBeDown != _leverDown)
                StartCoroutine(MoveLever(shouldBeDown));
        }

        private IEnumerator MoveLever(bool down)
        {
            _isMoving = true;
            yield return RotateTo(down ? _downRotation : _upRotation);
            _leverDown = down;
            _isMoving = false;
        }

        private IEnumerator DeniedNudge()
        {
            _isMoving = true;
            PlaySound(_soundDenied);

            Quaternion start = transform.localRotation;
            Quaternion other = _leverDown ? _upRotation : _downRotation;
            Quaternion partial = Quaternion.Slerp(start, other, deniedTravel);

            yield return RotateTo(partial);
            yield return RotateTo(start);

            _isMoving = false;
        }

        private IEnumerator RotateTo(Quaternion target)
        {
            while (Quaternion.Angle(transform.localRotation, target) > 0.5f)
            {
                transform.localRotation = Quaternion.Lerp(transform.localRotation, target, smooth * Time.deltaTime);
                yield return null;
            }

            transform.localRotation = target;
        }

        private bool AnyDoorOpen()
        {
            foreach (StationDoor door in doors)
                if (door != null && door.IsOpenOrOpening)
                    return true;

            return false;
        }

        private bool HasUsableDoors()
        {
            foreach (StationDoor door in doors)
                if (door != null && door.CanRespondToButton)
                    return true;

            return false;
        }

        private void PlaySound(AudioClip clip)
        {
            if (clip == null) return;
            GameManager.AudioSystem.PlaySoundPositional(clip, transform.position, GameManager.AudioSystem.VFX);
        }
    }
}
