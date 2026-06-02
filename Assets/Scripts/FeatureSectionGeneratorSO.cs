using System;
using UnityEngine;

namespace Root {
    [CreateAssetMenu(menuName = "SO/SectionGenerator/Generator/FeatureSelectionGenerator")]
    public class FeatureSectionGeneratorSO : SectionGeneratorSO {
        public FeatureSectionGeneratorSettingsSO settings;
        private MapPointsGen.Node node;
        [NonSerialized] private bool hasFinished;
        
        public override void Initialize(MapPointsGen.Node node) {
            this.node = node;
            hasFinished = false;
        }

        public override MapSection Create() {
            if (hasFinished) throw new Exception();
            
            hasFinished = true;
            
            if(settings.addAlert)
                GameManager.Train.AlertSystem.AddAlert(settings.FeatureSectionPrefab.alert);
            
            MapSection obj = Instantiate(settings.FeatureSectionPrefab);
            obj.shouldConsumeAlert = true;
            
            return obj;
        }

        public override bool HasFinished() {
            return hasFinished;
        }

        public override MapPointsGen.Node GetNextNode() {
            return node.OutConnections[0];
        }

        public override bool CanGenerate() {
            return !hasFinished;
        }
    }
}