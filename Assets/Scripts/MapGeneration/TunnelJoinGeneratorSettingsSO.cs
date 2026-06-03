using System;
using System.Collections.Generic;
using UnityEngine;

namespace Root {
    [CreateAssetMenu(menuName = "SO/SectionGenerator/Settings/TunnelJoinGeneratorSettings")]
    public class TunnelJoinGeneratorSettingsSO : ScriptableObject
    {
        public MapSection join;
        public int budget;
    }
}