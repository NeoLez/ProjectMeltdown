using UnityEngine;

namespace Root {
    [CreateAssetMenu(menuName = "SO/SectionGenerator/Settings/TunnelForkGeneratorSettings")]
    public class TunnelForkSectionGeneratorSettingsSO :  ScriptableObject {
        public MapSection ForkSection;
        public MapSection ForkStraightWaypoints;
        public MapSection ForkOutWaypoints;
        public bool forkRight;
    }
}