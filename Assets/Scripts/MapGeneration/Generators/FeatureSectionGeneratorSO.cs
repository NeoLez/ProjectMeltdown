using System;
using NaughtyAttributes;
using UnityEngine;

namespace Root {
    [CreateAssetMenu(menuName = "SO/SectionGenerator/Generator/FeatureSelectionGenerator")]
    public class FeatureSectionGeneratorSO : SectionGeneratorSO {
        [Expandable, SerializeField] private FeatureSectionGeneratorSettingsSO settings;
        private MapGeneration.MapGenerationContext _context;
        [NonSerialized] private bool hasFinished;
        
        public override void Initialize(MapGeneration.MapGenerationContext context)
        {
            _context = context;
            hasFinished = false;
        }

        public override MapSection Create() {
            if (hasFinished) throw new Exception();
            
            hasFinished = true;
            
            MapSection obj = Instantiate(settings.FeatureSectionPrefab);
            obj.shouldConsumeAlert = true;
            obj.isStation = true;

            if (settings.addAlert)
                GameManager.Train.AlertSystem.AddAlert(settings.FeatureSectionPrefab.alert, obj.GetWaypoints()[0]);
            
            return obj;
        }

        public override bool HasFinished() {
            return hasFinished;
        }

        public override MapPointsGen.Node GetNextNode() {
            if (_context.currentNode.OutConnections.Count == 0) return null;
            return _context.currentNode.OutConnections[0];
        }

        public override bool CanGenerate() {
            return !hasFinished;
        }
    }
}