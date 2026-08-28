using TMPro;
using UnityEngine;

namespace Root
{
    public class EmergencyButtonIndicator : MonoBehaviour
    {
        [SerializeField] private TMP_Text discText;
        [SerializeField] private TMP_Text usageText;
        [SerializeField] DiscSlot discSlot;
        private BrakeDiscItem _breakDisc;

        public void Update()
        {
            if (discSlot.GetBreakDisc() == null)
            {
                discText.text = "There is no emergency disc inserted";
                usageText.enabled = false;
                return;
            }

            discText.text = "Emergency disc inserted";

            _breakDisc = discSlot.GetBreakDisc();

            if(_breakDisc != null)
            {
                usageText.enabled = true;
                usageText.text = "Usage Left: "+ _breakDisc.GetDiscUsage().ToString();
            }            
        }
    }
}
