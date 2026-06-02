using System;
using UnityEngine;

namespace Root {
    [CreateAssetMenu(menuName = "SO/SectionGenerator/Generator/StationSelectionGenerator")]
    public class StationSectionGeneratorSO : SectionGeneratorSO {
        public StationSectionGeneratorSettingsSO settings;
        private MapPointsGen.Node node;
        [NonSerialized] private bool hasFinished;
        
        public override void Initialize(MapPointsGen.Node node) {
            this.node = node;
            hasFinished = false;
            stage = 0;
            count = 0;
        }

        private int stage;
        private int count;
        public override MapSection Create() {
            if (hasFinished) throw new Exception();

            switch (stage) {
                case 0:
                    var obj1 = Instantiate(settings.StraightRoad);
                    count++;

                    if (count == settings.safetyStraightRoadLength) {
                        count = 0;
                        stage++;
                        obj1.shouldConsumeAlert = true;
                        GameManager.Train.AlertSystem.AddAlert(settings.StraightRoad.alert);
                    }
                    
                    return obj1;
                case 1:
                    MapSection obj2 = Instantiate(settings.FeatureSectionPrefab);
                    obj2.shouldConsumeAlert = true;
                    GameManager.Train.AlertSystem.AddAlert(settings.FeatureSectionPrefab.alert);
                    obj2.shouldConsumeAlert = true;
                    stage++;
                    return obj2;
                case 2:
                    var obj3 = Instantiate(settings.StraightRoad);
                    count++;

                    if (count == settings.safetyStraightRoadLength) {
                        hasFinished = true;
                        obj3.shouldConsumeAlert = true;
                        GameManager.Train.AlertSystem.AddAlert(settings.StraightRoad.alert);
                    }
                    
                    return obj3;
            }
            throw new Exception();
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