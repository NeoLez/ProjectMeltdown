using UnityEngine;

namespace Root {
    [CreateAssetMenu(menuName = "Dialogue/Advanced/Effects/DebugPrint")]
    public class DebugPrintEffectSo : DialogueEffectSO {
        [SerializeField] private string textToPrint;

        public override bool CanExecute(DialogueExecutionContext context = null)
        {
            return true;
        }

        public override void Execute(DialogueExecutionContext context) {
            Debug.Log(textToPrint);
        }
    }
}