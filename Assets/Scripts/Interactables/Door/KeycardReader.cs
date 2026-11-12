using PrimeTween;
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

        [Header("Animacion de tarjeta")]
        [SerializeField] private Transform cardStartPoint;
        [SerializeField] private Transform cardEndPoint;
        [SerializeField] private float swipeDuration = 0.6f;
        [SerializeField] private float swipeEndPause = 0.15f;
        [SerializeField] private Ease swipeEase = Ease.InOutSine;

        private static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");
        private MaterialPropertyBlock _mpb;
        private bool _powered;
        private bool _isSwiping;
        private Sequence _swipeSequence;
        private System.Action _abortSwipe;

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

        private void OnDisable()
        {
            if (!_isSwiping) return;

            _swipeSequence.Stop();
            _abortSwipe?.Invoke();
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
            if (!_powered || _isSwiping) return;

            bool hasCard = TryGetHeldKeycard(out KeycardState card);

            if (!hasCard)
            {
                RefreshIdleLight();
                return;
            }

            if (cardStartPoint != null && cardEndPoint != null)
                PlayCardSwipe(card);
            else
                ResolveCard(card);
        }

        private void ResolveCard(KeycardState card)
        {
            if (!_powered)
            {
                RefreshIdleLight();
                return;
            }

            bool granted = false;

            foreach (StationDoor door in doors)
            {
                if (door == null) continue;
                if (door.TryAuthorizeWithCard(card))
                    granted = true;
            }

            SetLight(granted ? grantedColor : deniedColor);

            AudioClip clip = granted ? _soundGranted : _soundDenied;
            if (clip != null)
                GameManager.AudioSystem.PlaySoundPositional(clip, transform.position, GameManager.AudioSystem.VFX);
        }

        private void PlayCardSwipe(KeycardState card)
        {
            if (!GameManager.Player.TryGetComponent(out PlayerItemHolder holder))
            {
                ResolveCard(card);
                return;
            }

            var cardItem = card.ItemSo.CreatePhysicalItem(card);
            SetCardPhysics(cardItem, false);
            Transform cardTransform = cardItem.transform;
            cardTransform.parent = null;
            cardTransform.SetPositionAndRotation(cardStartPoint.position, cardStartPoint.rotation);
            holder.ForceClearHeldItem();
            _isSwiping = true;

            void ReturnCard(bool resolve)
            {
                _isSwiping = false;
                _abortSwipe = null;

                SetCardPhysics(cardItem, true);
                if (holder != null) holder.Pickup(cardItem);

                if (resolve) ResolveCard(card);
            }

            _abortSwipe = () => ReturnCard(false);

            _swipeSequence = Sequence.Create()
                .Chain(Tween.Custom(0f, 1f, swipeDuration, t => cardTransform.SetPositionAndRotation(
                    Vector3.LerpUnclamped(cardStartPoint.position, cardEndPoint.position, t),
                    cardStartPoint.rotation), swipeEase))
                .ChainDelay(swipeEndPause)
                .ChainCallback(() => ReturnCard(true));
        }

        private void SetCardPhysics(Component cardItem, bool active)
        {
            if (cardItem.TryGetComponent(out Rigidbody rb))
                rb.isKinematic = !active;

            foreach (Collider col in cardItem.GetComponentsInChildren<Collider>())
                col.enabled = active;
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