using Root;
using Root.Controller;
using Root.Managers;
using System;
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

        private void Awake()
        {
            OnBellRanged += OpenStore;
            //OnBellRangedAgain += OpenStore2;
            _OnStoreShow += HandleInteraction;
        }

        private void Start()
        {
            CloseStore();

            cameraPivot = GameObject.FindAnyObjectByType<CameraController>().CameraPivot;
        }

        private void OpenStore()
        {
            IsStoreOpened = true;
            storeManager.ShowStore();

            _newPlayerPosition = new Vector3(npcPositionPivot.position.x, GameManager.Player.gameObject.transform.position.y, npcPositionPivot.position.z);

            GameManager.Player.GetComponent<MovementController>().CenterPlayerDialogueInteraction(cameraPivot, _newPlayerPosition);
            GameManager.Player.GetComponent<CameraController>().FocusCamera(npcLookingPivot);

            UIManager.Instance.OpenMenu(UIManager.UITypes.Store);
        }

        private void OpenStore2()
        {
            IsStoreOpened = true;

            UIManager.Instance.OpenMenu(UIManager.UITypes.Store);
        }

        public void CloseStore()
        {
            storeManager.HideStore();
            IsStoreOpened = false;
        }

        public void CanShowItems(bool show)
        {
            if (show)
            {
                OpenStore();
                //merchantAnimator.SetBool("Appear", true);
            }
            else 
            {
                merchantAnimator.SetBool("Appear", false);
            }

        }

        private void HandleInteraction(bool enable)
        {
            if (enable)
            {
                colliders.enabled = true;
                CanShowItems(true);
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



