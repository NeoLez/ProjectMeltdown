using UnityEngine;

namespace Root {
    [CreateAssetMenu(menuName = "SO/SectionGenerator/Settings/FeatureSectionGeneratorSettings")]
    public class TunnelForkSectionGeneratorSettingsSO :  ScriptableObject {
        public MapSection ForkSection;
        public MapSection TunnelForkRight;
        public MapSection TunnelForkLeft;
        public MapSection StraightRoad;
        public int straightSectionLength;
        public int curveSectionLength;
    }
}