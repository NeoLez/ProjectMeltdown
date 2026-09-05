using UnityEngine;

namespace Root
{
    public class NPCInteraction : InteractBehaviour
    {
        [SerializeField] private Transform instancePivot;
        [SerializeField] private GameObject[] packagesToDeliver;

        private void Start()
        {
            Dialogue.OnDialogueEnded += FinishedExecutingDialogue;
            Dialogue.OnSelectedChoice += ChosingOptions;
        }

        private void OnDestroy()
        {
            Dialogue.OnDialogueEnded -= FinishedExecutingDialogue;
            Dialogue.OnSelectedChoice -= ChosingOptions;
        }

        public override void ExecuteDialogue()
        {
            if (!gameObject.activeInHierarchy) return;

            if (hasBeenTriggeredOnce) return;

            SubtitleManager.Instance.SetTextValues(Dialogue);

            TriggerDialogue();
        }

        void TriggerDialogue()
        {
            if (Dialogue != null)
            {
                DialogueManager.Instance.TriggerDialogue();
            }
        }

        public override void FinishedExecutingDialogue()
        {
            if (!Dialogue.CanRepeatDialogue) hasBeenTriggeredOnce = true;

            base.FinishedExecutingDialogue();
        }


        private void ChosingOptions(int index)
        {
            switch (index)
            {
                case 0: //opcion SI
                    GivePlayerMission();
                    break;
                case 1: //opcion NO
                    Debug.Log("sos un tonto");
                    break;
                default:
                    break;
            }
        }

        //previo un sistema de eleccion
        public void GivePlayerMission()
        {
            PackagesSystemController.Instance.GeneratePackages(instancePivot, packagesToDeliver);
        }
        //que el chabon ya venga con una mision creada, solo la activa cuando vos la elegis
    }
}
