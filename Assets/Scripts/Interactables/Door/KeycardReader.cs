using UnityEngine;

namespace Root
{
    public class KeycardReader : InteractableNormalCamera
    {
        [SerializeField] private StationDoor[] doors;

        [Header("Luz indicadora")]
        [SerializeField] private Renderer lightRenderer;
        [SerializeField] private int emissiveMaterialIndex = 1;
        [SerializeField] private Color deniedColor = Color.red;
        [SerializeField] private Color grantedColor = Color.green;
        [SerializeField] private float emissionIntensity = 2f;

        [Header("Sonidos")]
        [SerializeField] private AudioClip _soundGranted;
        [SerializeField] private AudioClip _soundDenied;

        private static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");
        private MaterialPropertyBlock _mpb;
        private bool _powered;

        private void Awake()
        {
            _mpb = new MaterialPropertyBlock();
            _powered = IsZonePowered();
            RefreshIdleLight();
        }

        private void Start()
        {
            foreach (StationDoor door in doors)
            {
                if (door == null) continue;
                door.OnPowerChanged += HandlePowerChanged;
            }
        }

        private void OnDestroy()
        {
            foreach (StationDoor door in doors)
            {
                if (door == null) continue;
                door.OnPowerChanged -= HandlePowerChanged;
            }
        }

        public override void ShowFeedback(bool canShow)
        {
            base.ShowFeedback(_powered && canShow);
        }

        public override void Interact()
        {
            if (!_powered) return;

            bool hasCard = TryGetHeldKeycard(out KeycardState card);
            bool granted = false;

            if (hasCard)
            {
                foreach (StationDoor door in doors)
                {
                    if (door == null) continue;
                    if (door.TryAuthorizeWithCard(card))
                        granted = true;
                }
            }

            if (hasCard)
                SetLight(granted ? grantedColor : deniedColor);
            else
                RefreshIdleLight();

            if (!hasCard) return;

            AudioClip clip = granted ? _soundGranted : _soundDenied;
            if (clip != null)
                GameManager.AudioSystem.PlaySoundPositional(clip, transform.position, GameManager.AudioSystem.VFX);
        }

        private bool TryGetHeldKeycard(out KeycardState card)
        {
            card = null;

            if (!GameManager.Player.TryGetComponent(out PlayerItemHolder holder)) return false;
            if (!holder.HasItem) return false;

            card = holder.HeldItem as KeycardState;
            return card != null;
        }

        private void HandlePowerChanged()
        {
            _powered = IsZonePowered();
            RefreshIdleLight();
        }

        private bool IsZonePowered()
        {
            bool anyDoor = false;

            foreach (StationDoor door in doors)
            {
                if (door == null) continue;
                anyDoor = true;
                if (door.HasPower) return true;
            }

            return !anyDoor;
        }

        private bool AnyDoorOpen()
        {
            foreach (StationDoor door in doors)
            {
                if (door == null) continue;
                if (door.IsOpenOrOpening) return true;
            }

            return false;
        }

        private void RefreshIdleLight()
        {
            if (!_powered)
            {
                SetLight(Color.black);
                return;
            }

            SetLight(AnyDoorOpen() ? grantedColor : deniedColor);
        }

        private void SetLight(Color color)
        {
            if (lightRenderer == null) return;

            lightRenderer.GetPropertyBlock(_mpb, emissiveMaterialIndex);
            _mpb.SetColor(EmissionColorId, color * emissionIntensity);
            lightRenderer.SetPropertyBlock(_mpb, emissiveMaterialIndex);
        }
    }
}