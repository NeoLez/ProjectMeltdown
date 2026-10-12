using JetBrains.Annotations;

namespace Root.Factories {
    public readonly struct ItemSpawnContext {
        [CanBeNull] public readonly MapSection Section;
        [CanBeNull] public readonly System.Random Random;

        public ItemSpawnContext(MapSection section, System.Random random) {
            Section = section;
            Random = random;
        }
    }
}