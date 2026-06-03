using System;
using System.Linq;
using UnityEngine;

namespace Root {
    [CreateAssetMenu(menuName = "SO/SectionGenerator/Generator/TunnelSectionGenerator")]
    public class TunnelSectionGeneratorSO : SectionGeneratorSO {
        public TunnelSectionGeneratorSettingsSO settings;
        private MapGeneration.MapGenerationContext _context;
        [NonSerialized] private bool hasFinished;
        
        public override void Initialize(MapGeneration.MapGenerationContext context) {
            _context = context;
            hasFinished = false;
            currentRepetition = 0;
            trackSectionsCreated = 0;
            currentSection = null;
            
            settings.Initialize();
        }
        
        private int currentRepetition;
        private MapSection currentSection;
        private int trackSectionsCreated;
        public override MapSection Create() {
            if (hasFinished) throw new Exception();

            if (currentRepetition == 0) {
                var rand = UnityEngine.Random.Range(0, settings.mapSections.Count);
                var key = settings.mapSections.Keys.ElementAt(rand);
                var speed = settings.mapSections[key];
                TunnelSectionGeneratorSettingsSO.MapSectionListing nextSectionListing = speed[UnityEngine.Random.Range(0, speed.Count)];
                currentRepetition = UnityEngine.Random.Range(nextSectionListing.minRepetition, nextSectionListing.maxRepetition + 1);
                currentSection = nextSectionListing.mapSection;
                GameManager.Train.AlertSystem.AddAlert(currentSection.alert);
                trackSectionsCreated++;
            }
            currentRepetition--;

            MapSection obj = Instantiate(currentSection);
            
            if (currentRepetition == 0) {
                obj.shouldConsumeAlert = true;
                if (trackSectionsCreated >= settings.maxTrackSections) {
                    hasFinished = true;
                }
            }
            
            return obj;
        }

        public override bool HasFinished() {
            return hasFinished;
        }

        public override MapPointsGen.Node GetNextNode() {
            return settings.skipNode ? _context.currentNode : _context.currentNode.OutConnections[0];
        }

        public override bool CanGenerate() {
            return !hasFinished;
        }
    }
}