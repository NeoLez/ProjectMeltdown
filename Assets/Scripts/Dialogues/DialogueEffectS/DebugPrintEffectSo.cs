using UnityEngine;

namespace Root {
    [CreateAssetMenu(menuName = "Dialogue/Advanced/Effects/DebugPrint")]
    public class DebugPrintEffectSo : DialogueEffectSO {
        [SerializeField] private string textToPrint;
        public override void Execute(DialogueExecutionContext context) {
            Debug.Log(textToPrint);
        }
    }
}