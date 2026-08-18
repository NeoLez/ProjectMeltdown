using System;
using UnityEngine;

namespace Root {
    [CreateAssetMenu(menuName = "SO/SectionGenerator/Generator/StationSelectionGenerator")]
    public class StationSectionGeneratorSO : SectionGeneratorSO {
        public StationSectionGeneratorSettingsSO settings;
        private MapGeneration.MapGenerationContext _context;
        [NonSerialized] private bool hasFinished;
        
        public override void Initialize(MapGeneration.MapGenerationContext context) {
            _context = context;
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
                    if(count == 0)
                        GameManager.Train.AlertSystem.AddAlert(settings.StraightRoad.alert);
                    
                    var obj1 = Instantiate(settings.StraightRoad);
                    count++;

                    if (count == settings.safetyStraightRoadLength) {
                        count = 0;
                        stage++;
                        obj1.shouldConsumeAlert = true;
                    }
                    
                    return obj1;
                case 1:
                    Debug.Log(settings);
                    Debug.Log(settings.FeatureSectionPrefab);
                    Debug.Log(settings.FeatureSectionPrefab.GetEntry());
                    MapSection obj2 = Instantiate(settings.FeatureSectionPrefab.GetEntry());
                    obj2.isStation = true;
                    obj2.shouldConsumeAlert = true;
                    GameManager.Train.AlertSystem.AddAlert(obj2.alert);
                    stage++;
                    return obj2;
                case 2:
                    if(count == 0)
                        GameManager.Train.AlertSystem.AddAlert(settings.StraightRoad.alert);
                    
                    var obj3 = Instantiate(settings.StraightRoad);
                    count++;

                    if (count == settings.safetyStraightRoadLength) {
                        hasFinished = true;
                        obj3.shouldConsumeAlert = true;
                    }
                    
                    return obj3;
            }
            throw new Exception();
        }

        public override bool HasFinished() {
            return hasFinished;
        }

        public override MapPointsGen.Node GetNextNode() {
            return _context.currentNode.OutConnections[0];
        }

        public override bool CanGenerate() {
            return !hasFinished;
        }
    }
}