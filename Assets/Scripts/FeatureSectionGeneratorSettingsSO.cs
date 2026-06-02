using UnityEngine;

namespace Root {
    [CreateAssetMenu(menuName = "SO/SectionGenerator/Settings/FeatureSectionGeneratorSettings")]
    public class FeatureSectionGeneratorSettingsSO :  ScriptableObject {
        public MapSection FeatureSectionPrefab;
    }
}