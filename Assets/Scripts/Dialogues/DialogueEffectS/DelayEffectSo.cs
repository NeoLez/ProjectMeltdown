using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace Root {
    [CreateAssetMenu(menuName = "Dialogue/Advanced/Effects/Delay")]
    public class DelayEffectSo : DialogueEffectSO {
        [SerializeField, Expandable] private List<DialogueEffectSO> effects;
        [SerializeField] private float delay;
        public override void Execute(DialogueExecutionContext context) {
            context.NPC.StartCoroutine(Delay(context));
        }

        private IEnumerator Delay(DialogueExecutionContext context) {
            yield return new WaitForSeconds(delay);
            foreach (var effect in effects) {
                effect.Execute(context);
            }
        }
    }
}