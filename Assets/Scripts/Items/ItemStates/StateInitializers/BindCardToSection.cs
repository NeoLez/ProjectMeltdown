using UnityEngine;

namespace Root.Factories {
    [CreateAssetMenu(menuName = "Items/Initializers/Bind Keycard To Section")]
    public class BindKeycardToSection : ItemStateInitializer<KeycardState> {
        protected override void ApplyTo(KeycardState state, in ItemSpawnContext ctx) {
            if (ctx.Section == null) {
                Debug.LogError("Keycard spawned outside a MapSection", state.ItemSo);
                return; }

            if (!ctx.Section.TryGetComponent<StationAccessController>(out var controller)) {
                Debug.LogError("Keycard spawned outside a station", state.ItemSo);
                return;
            }
            controller.SubscribeKeycard(state);
        }
    }
}