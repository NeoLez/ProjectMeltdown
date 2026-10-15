using UnityEngine;

namespace Root.Factories {
    [CreateAssetMenu(menuName = "Items/Initializers/Bind Blue Keycard")]
    public class BindBlueKeycardToSection : ItemStateInitializer<KeycardState> {
        protected override void ApplyTo(KeycardState state, in ItemSpawnContext ctx) {
            if (ctx.Section == null) {
                Debug.LogError("Keycard spawned outside a MapSection", state.ItemSo);
                return; }

            var nodes = ctx.Section.Node.GetNodesThatMatch((node, _) => {
                return node.feature == MapPointsGen.Feature.TUNNEL_JOIN;
            }, 10);

            if (nodes.Count == 0) {
                Debug.LogWarning("No matching features found", this);
            }

            state.targetNode = nodes[0].Item1;
        }
    }
}