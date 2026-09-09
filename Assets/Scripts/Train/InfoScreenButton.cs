using UnityEngine;

namespace Root
{
    [RequireComponent(typeof(Collider))]
    public class InfoScreenButton : Interactable
    {
        [SerializeField] private InfoScreenController screen;
        [SerializeField] private bool isNext = true; // true = siguiente
                                                     // false = anterior

        public override void StartInteraction()
        {
            if (isNext)
                screen.NextInfo();
            else
                screen.PreviousInfo();
        }

        public override void EndInteraction() { }
    }
}