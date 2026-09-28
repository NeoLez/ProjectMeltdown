using PrimeTween;
using Root.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Root
{
    public class PackageDeliverPost : InteractableNormalCamera, IItemDragReceiver
    {
        public bool HasConfirmedDelivery { get; private set; }

        [SerializeField] private Transform dropPivot;
        [SerializeField] private Canvas displayCanvas;
        [SerializeField] private TMP_Text priceCounter;
        [SerializeField] private TMP_Text textNotifier;
        [SerializeField] private ItemGroup packages;

        [SerializeField] private MapSection mapSection;

        private string _format = "{0}$";

        private List<PackageItemState> _depositedPackages = new();
        private int _currentPackageSum;

        [Header("Deposit Door")]
        [SerializeField] private Transform pivot;
        [SerializeField] private Vector3 maximumRotation = new Vector3(45f, 0f, 0f);
        [SerializeField] private float deliveryDoorDuration = 1f;
        /* [SerializeField] private Animator animator;
        private int _animStateOpen = Animator.StringToHash("OpenDepositDoor");
        private int _animStateClose = Animator.StringToHash("CloseDepositDoor");*/
        private bool _isAnimating;

        public Action<bool> OnPackagesDelivered;
        private List<TypeOfPackage> _packagesType = new();
        private int _amount;
        private Action OnDeliveryFinished;

        private Coroutine _textRoutine;
        private string[] _finalMessage = { "Entrega confirmada", "Hasta Luego" };

        private void Awake()
        {
            OnDeliveryFinished += DisablePost;
        }

        private void Start()
        {
            RefreshSumAmount(0);
        }

        private MissionObjectiveSO MissionsCheck() //TODO-Expand this into a list of MissionObjectiveSO
        {
            MissionObjectiveSO activeMissions = MissionsManager.Instance.SingleMission();

            return activeMissions;
        }

        public void DepositPackage(PackageItemState itemState)
        {
            if (HasConfirmedDelivery) return;
            if (_isAnimating) return;

            StartLeverAnimation(maximumRotation, deliveryDoorDuration);

            RefreshSumAmount(itemState.price);
            if (!_depositedPackages.Contains(itemState))
            {
                _depositedPackages.Add(itemState);
            }

            _packagesType.Add(itemState.typeOfPackage);
            _amount++;
        }

        public void CheckGoal()
        {
            int money;

            if (MissionsManager.Instance.VerifyDeliveryConditions(MissionsCheck().Id, _amount, _packagesType))
            {
                money = PackagesSystemController.Instance.CheckPackageConditions(true);
            }
            else
            {
                money = PackagesSystemController.Instance.CheckPackageConditions(false, _depositedPackages);
            }

            MissionsManager.Instance.DeleteDepositedPackage(MissionsCheck().Id, _depositedPackages);
            MissionsManager.Instance.FinishMission(MissionsCheck());

            SpawnBills(MoneyManager.Instance.NumberToBills(money));

            HasConfirmedDelivery = true;

            OnPackagesDelivered?.Invoke(true); //si yo tengo otros paquetes que entregar, lo pongo en false asi puedo volver a presionar el boton
            if (_textRoutine == null)
                _textRoutine = StartCoroutine(UpdateTextRoutine(_finalMessage, true));
        }

        private void SpawnBills(List<ValueTuple<BillItemSo, int>> bills)
        {
            foreach (var tuple in bills)
            {
                for (int i = 0; i < tuple.Item2; i++)
                {
                    var bill = tuple.Item1.CreatePhysicalItem();
                    bill.transform.position = dropPivot.transform.position;
                }
            }
        }

       /* private IEnumerator TriggerDepositAnims()
        {
            _isAnimating = true;
            animator.SetTrigger(_animStateOpen);
            yield return new WaitForSeconds(deliveryDoorDuration);
            animator.SetTrigger(_animStateClose);
            _isAnimating = false;
        }*/

        private void StartLeverAnimation(Vector3 rotationAngle, float returnDuration)
        {
            _isAnimating = true;
            Tween.LocalRotation(
            target: pivot,
            startValue: pivot.localRotation,
            endValue: pivot.localRotation *  Quaternion.Euler(rotationAngle),
            duration: returnDuration,
            ease: Ease.OutQuad,
            cycles: 2,
            cycleMode: CycleMode.Yoyo
            ).OnComplete(() => _isAnimating = false);
        }

        private void RefreshSumAmount(int amount)
        {
            _currentPackageSum += amount;
            priceCounter.text = string.Format(_format, _currentPackageSum);

            PackagesSystemController.Instance.SumCurrentDeposited(_currentPackageSum);
        }


        private IEnumerator UpdateTextRoutine(string txt, bool canReset)
        {
            priceCounter.enabled = false;
            textNotifier.text = txt; //TODO-Change to Localization
            yield return new WaitForSeconds(2f);
            ResetPostStatus(canReset);

            _textRoutine = null;
        }
        private IEnumerator UpdateTextRoutine(string[] message, bool canReset)
        {
            if (message.Length > 0)
            {
                foreach (var text in message)
                {
                    priceCounter.enabled = false;
                    textNotifier.text = text;
                    yield return new WaitForSeconds(2f);
                }
                ResetPostStatus(canReset);
            }

            _textRoutine = null;
        }

        private void DisableText()
        {
            displayCanvas.enabled = false;
        }

        private void ResetPostStatus(bool canReset)
        {
            if (canReset)
            {
                _depositedPackages.Clear();
                _currentPackageSum = 0;
                RefreshSumAmount(0);
            }

            if (HasConfirmedDelivery) OnDeliveryFinished?.Invoke();

            priceCounter.enabled = true;
            textNotifier.text = "Monto Total: "; //TODO-Change to Localization
        }

        private void DisablePost()
        {
            if (!MissionsManager.Instance.AreMissionsActive())
            {
                DisableText();
                gameObject.SetActive(false);
                return;
            }
        }

        public bool DepositedPackages()
        {
            return _depositedPackages.Count > 0;
        }

        private void OnDestroy()
        {
            OnDeliveryFinished -= DisablePost;

            _depositedPackages.Clear();
        }


        public override void Interact()
        {
            PlayerItemHolder holder = GameManager.Player.GetComponent<PlayerItemHolder>();

            if (holder == null) return;

            if (!holder.HasItem)
            {
                if (GameManager.Player.GetComponent<Inventory>().ContainsItemType(packages))
                {
                    PlayerInventoryUI.Instance.OpenInventory();
                }
                else
                {
                    if (_textRoutine == null) _textRoutine = StartCoroutine(UpdateTextRoutine("No tiene ningun paquete para depositar", true));
                }

                return;
            }

            var itemState = holder.HeldItem as PackageItemState;
            if (itemState == null) return;

            if (!itemState.canBeDelivered)
            {
                if (_textRoutine == null) _textRoutine = StartCoroutine(UpdateTextRoutine("Paquete fuera de mision", false));
                return;
            }


            if (itemState.DestinationNode != mapSection.Node) {
                if (_textRoutine == null) _textRoutine = StartCoroutine(UpdateTextRoutine("Destino incorrecto", false));
                return;
            }
            //if (!MissionsManager.Instance.AreMissionsActive()) return;

            DepositPackage(itemState);
            holder.ForceClearHeldItem();
        }

        public bool CanTakeItem(Vector2 position, Vector2Int size, InventoryItem item)
        {
            var itemState = item.itemState as PackageItemState;
            if (!itemState.canBeDelivered)
            {
                if (_textRoutine == null) _textRoutine = StartCoroutine(UpdateTextRoutine("Paquete fuera de mision", false));
            }
            if (itemState.DestinationNode != mapSection.Node) {
                if (_textRoutine == null) _textRoutine = StartCoroutine(UpdateTextRoutine("Destino incorrecto", false));
                return false;
            }

            return item.itemState is PackageItemState && !_isAnimating && !HasConfirmedDelivery && itemState.canBeDelivered;
        }

        public bool TakeItem(Vector2 position, InventoryItem.InventoryItemRotation rotation, InventoryItem item)
        {
            if (!CanTakeItem(position, InventoryItem.GetRotationCorrectedSize(item.Size, rotation), item)) return false;

            DepositPackage(item.itemState as PackageItemState);
            return true;
        }

        public void ClearFeedback()
        {
        }
    }
}
