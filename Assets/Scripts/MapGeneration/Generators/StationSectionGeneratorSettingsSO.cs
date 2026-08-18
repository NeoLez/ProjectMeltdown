using UnityEngine;

namespace Root {
    [CreateAssetMenu(menuName = "SO/SectionGenerator/Settings/StationSectionGeneratorSettings")]
    public class StationSectionGeneratorSettingsSO :  ScriptableObject {
        public SectionPoolSO FeatureSectionPrefab;
        public MapSection StraightRoad;
        public int safetyStraightRoadLength;
    }
}