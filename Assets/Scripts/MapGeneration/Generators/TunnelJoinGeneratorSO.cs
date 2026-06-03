using System;
using UnityEngine;

namespace Root {
    [CreateAssetMenu(menuName = "SO/SectionGenerator/Generator/TunnelJoinGenerator")]
    public class TunnelJoinGeneratorSO : SectionGeneratorSO {
        public TunnelJoinGeneratorSettingsSO settings;
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
            
            MapSection obj;

            if (_context.currentNode.height > _context.lastNode.height)
                obj = Instantiate(settings.joinFromRightToMain);
            else if (_context.currentNode.height < _context.lastNode.height)
                obj = Instantiate(settings.joinFromLeftToMain);
            else if (_context.currentNode.height < _context.currentNode.InConnections[1].height)
                obj = Instantiate(settings.straightEntryOnRight);
            else
                obj = Instantiate(settings.straightEntryOnLeft);
            
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