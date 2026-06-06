using System;
using UnityEngine;

namespace Root {
    [CreateAssetMenu(menuName = "SO/SectionGenerator/Generator/TunnelForkGenerator")]
    public class TunnelForkSectionGeneratorSO : SectionGeneratorSO {
        private MapGeneration.MapGenerationContext _context;
        [SerializeField] private TunnelForkSectionGeneratorSettingsSO settings;
        private bool hasFinished;
        private bool phase;
        private bool decisionTaken;
        private bool createdFork;
        
        public override void Initialize(MapGeneration.MapGenerationContext context) {
            _context = context;
            hasFinished = false;
            decisionTaken = false;
            createdFork = false;
        }

        public override MapSection Create() {
            if (hasFinished) throw new Exception();
            
            MapSection obj;
            Debug.Log("a");
            if (!createdFork) {
                Debug.Log("b");
                createdFork = true;
                
                obj = Instantiate(settings.ForkSection);
                GameManager.Train.AlertSystem.AddAlert(obj.alert);
                obj.GetWaypoints()[^1].OnTrainReached += () => {
                    decisionTaken = true;
                    GameManager.MapGeneration.UpdateSections();
                };
                return obj;
            }

            
            if (GameManager.Train.forkDecisionSwitch.GetDirection()) {
                Debug.Log("c");
                obj = settings.forkRight ? Instantiate(settings.ForkOutWaypoints) : Instantiate(settings.ForkStraightWaypoints);
            }
            else {
                Debug.Log("d");
                obj = settings.forkRight ? Instantiate(settings.ForkStraightWaypoints) : Instantiate(settings.ForkOutWaypoints);
            }
            hasFinished = true;
            
            return obj;
        }

        public override bool HasFinished() {
            return hasFinished;
        }

        public override MapPointsGen.Node GetNextNode() {
            return _context.currentNode.OutConnections[0];
        }

        public override bool CanGenerate() {
            return !hasFinished && decisionTaken;
        }
    }
}