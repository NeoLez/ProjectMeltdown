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

        private string _format = "{0}$";

        private List<PackageItemState> _depositedPackages = new();
        private int _currentPackageSum;

        [SerializeField] private Animator animator;
        [SerializeField] private float deliveryDoorDuration = 1f;
        private int _animStateOpen = Animator.StringToHash("OpenDepositDoor");
        private int _animStateClose = Animator.StringToHash("CloseDepositDoor");
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

            StartCoroutine(TriggerDepositAnims());

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

            if (_textRoutine == null)
                _textRoutine = StartCoroutine(UpdateTextRoutine(_finalMessage, true));

            MissionsManager.Instance.DeleteDepositedPackage(MissionsCheck().Id, _depositedPackages);
            MissionsManager.Instance.FinishMission(MissionsCheck());

            SpawnBills(MoneyManager.Instance.NumberToBills(money));

            HasConfirmedDelivery = true;

            OnPackagesDelivered?.Invoke(true); //si yo tengo otros paquetes que entregar, lo pongo en false asi puedo volver a presionar el boton
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

        private IEnumerator TriggerDepositAnims()
        {
            _isAnimating = true;
            animator.SetTrigger(_animStateOpen);
            yield return new WaitForSeconds(deliveryDoorDuration);
            animator.SetTrigger(_animStateClose);
            _isAnimating = false;
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
