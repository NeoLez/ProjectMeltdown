using System;
using UnityEngine;

namespace Root {
    [CreateAssetMenu(menuName = "SO/SectionGenerator/Generator/TunnelJoinGenerator")]
    public class TunnelJoinGeneratorSO : SectionGeneratorSO {
        public TunnelJoinGeneratorSettingsSO settings;
        private MapGeneration.MapGenerationContext _context;
        [NonSerialized] private bool hasFinished;
        public int budget;
        
        public override void Initialize(MapGeneration.MapGenerationContext context)
        {
            _context = context;
            hasFinished = false;
            budget = settings.budget;
        }
        
        public override MapSection Create() {
            if (hasFinished) throw new Exception();

            MapSection obj = Instantiate(settings.join);
            budget--;
            
            return obj;
        }

        public override bool HasFinished() {
            return hasFinished;
        }

        public override MapPointsGen.Node GetNextNode() {
            return _context.currentNode.OutConnections[0];
        }

        public override bool CanGenerate() {
            return !hasFinished && budget > 0;
        }
    }
}