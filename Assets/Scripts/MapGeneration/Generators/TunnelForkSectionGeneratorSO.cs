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
        private bool changeLine;
        
        public override void Initialize(MapGeneration.MapGenerationContext context) {
            _context = context;
            hasFinished = false;
            decisionTaken = false;
            createdFork = false;
        }

        public override MapSection Create() {
            if (hasFinished) throw new Exception();
            
            MapSection obj;
            if (!createdFork) {
                createdFork = true;
                
                obj = Instantiate(settings.ForkSection);
                GameManager.Train.AlertSystem.AddAlert(obj.alert);
                obj.GetWaypoints()[^1].OnTrainReached += () => {
                    decisionTaken = true;
                    GameManager.MapGeneration.UpdateSections();
                };
                return obj;
            }

            Debug.Log("T " + GameManager.Train.forkDecisionSwitch.GetDirection() + " " + settings.forkRight);
            changeLine = !(GameManager.Train.forkDecisionSwitch.GetDirection() ^ settings.forkRight);
            Debug.Log(changeLine);
            obj = changeLine ? Instantiate(settings.ForkOutWaypoints) : Instantiate(settings.ForkStraightWaypoints);
            hasFinished = true;
            
            return obj;
        }

        public override bool HasFinished() {
            return hasFinished;
        }

        public override MapPointsGen.Node GetNextNode() {
            return changeLine ? _context.currentNode.OutConnections[1] :  _context.currentNode.OutConnections[0];
        }

        public override bool CanGenerate() {
            return !hasFinished && decisionTaken;
        }
    }
}