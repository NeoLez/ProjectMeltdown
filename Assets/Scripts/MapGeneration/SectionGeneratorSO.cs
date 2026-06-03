using UnityEngine;

namespace Root {
    public abstract class SectionGeneratorSO : ScriptableObject {
        public abstract void Initialize(MapGeneration.MapGenerationContext node);
        public abstract MapSection Create();
        public abstract bool HasFinished();
        public abstract MapPointsGen.Node GetNextNode();
        public abstract bool CanGenerate();
    }
}