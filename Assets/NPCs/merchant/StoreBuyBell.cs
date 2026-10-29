using Root;
using Root.Controller;
using Root.Managers;
using System;
using System.Collections;
using UnityEngine;

namespace Root
{
    public class StoreBuyBell : StoreBell
    {
        [SerializeField] private Transform npcPositionPivot;
        [SerializeField] private Transform npcLookingPivot;
        private Vector3 _newPlayerPosition;
        private Transform cameraPivot;

        [SerializeField] private Animator merchantAnimator;
        [SerializeField] private StoreMenu menu;
        [SerializeField] private Collider colliders;

        public Action<bool> _OnStoreShow;
        private bool _wasAnimationTriggered;

        private void Awake()
        {
            OnBellRanged += HandleStore;
            //OnBellRangedAgain += OpenStore2;
            _OnStoreShow += HandleInteraction;
        }

        private void Start()
        {
            CloseStore();

            cameraPivot = GameObject.FindAnyObjectByType<CameraController>().CameraPivot;
        }

        private void HandleStore()
        {
            if (!_wasAnimationTriggered)
                StartCoroutine(TriggerAnimation());
            else
            {
                OpenStore();
                OpenStorePanel();
            }
        }

        private void OpenStore()
        {
            IsStoreOpened = true;
            storeManager.ShowStore();

            _newPlayerPosition = new Vector3(npcPositionPivot.position.x, GameManager.Player.gameObject.transform.position.y, npcPositionPivot.position.z);

            GameManager.Player.GetComponent<MovementController>().CenterPlayerDialogueInteraction(cameraPivot, _newPlayerPosition);
            GameManager.Player.GetComponent<CameraController>().FocusCamera(npcLookingPivot);
        }

        private void OpenStorePanel()
        {
            UIManager.Instance.OpenMenu(UIManager.UITypes.Store);
        }

        public void CloseStore()
        {
            storeManager.HideStore();
            IsStoreOpened = false;
        }

        private IEnumerator TriggerAnimation()
        {
            if (merchantAnimator == null) yield break;

            OpenStore();
            merchantAnimator.Play("2_StartShop");
            yield return new WaitForSeconds(4f);
            _wasAnimationTriggered = true;

            OpenStorePanel();
        }

        private void HandleInteraction(bool enable)
        {
            if (enable)
            {
                colliders.enabled = true;
                OpenStorePanel();
            }
            else
            {
                colliders.enabled = false;
            }
            
        }

        private void OnDestroy()
        {
            _OnStoreShow -= HandleInteraction;

            OnBellRanged = null;
        }

    }

}



