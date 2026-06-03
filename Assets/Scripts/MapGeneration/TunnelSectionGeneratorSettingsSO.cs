using System;
using System.Collections.Generic;
using UnityEngine;

namespace Root {
    [CreateAssetMenu(menuName = "SO/SectionGenerator/Settings/TunnelSectionGeneratorSettings")]
    public class TunnelSectionGeneratorSettingsSO : ScriptableObject{
        [Serializable] public class MapSectionListing {
            public int maxSpeed;
            public MapSection mapSection;
            public int minRepetition;
            public int maxRepetition;
        }
        [SerializeField] private List<MapSectionListing> _sectionListings;
        public Dictionary<int, List<MapSectionListing>> mapSections = new();
        public int maxTrackSections;
        public bool skipNode;
        
        private bool initialized;
        public void Initialize() {
            if (initialized) return;
            initialized = false;
            mapSections.Clear();
            
            foreach (var sectionListing in _sectionListings) {
                if (!mapSections.TryGetValue(sectionListing.maxSpeed, out var list)) {
                    list = new();
                    mapSections[sectionListing.maxSpeed] = list;
                }
                list.Add(sectionListing);
            }
        }
    }
}