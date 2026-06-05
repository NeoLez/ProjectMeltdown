using System;
using UnityEngine;

namespace Root {
    public class TunnelForkSectionGeneratorSO : SectionGeneratorSO {
        private MapGeneration.MapGenerationContext _context;
        [SerializeField] private TunnelForkSectionGeneratorSettingsSO settings;
        private bool hasFinished;
        private int straightPartCounter;
        
        public override void Initialize(MapGeneration.MapGenerationContext context) {
            _context = context;
            hasFinished = false;
            straightPartCounter = settings.straightSectionLength;
        }

        public override MapSection Create() {
            if (hasFinished) throw new Exception();
            hasFinished = true;
            
            MapSection obj;

            if (straightPartCounter > 0) {
                straightPartCounter++;
                return Instantiate(settings.StraightRoad);
            }


            throw new NotImplementedException();
            return obj;
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