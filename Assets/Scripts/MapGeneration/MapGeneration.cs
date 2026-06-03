using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Root {
    public class MapGeneration : MonoBehaviour {
        private List<MapSection> IncomingSections = new();
        private List<MapSection> PastSections = new();
        [SerializeField] private int LoadedSectionCount;
        
        [SerializeField] private Transform root;
        [SerializeField] private Train train;
        [SerializeField] public Transform itemRoot;

        [SerializeField] private int mapHeight;
        [SerializeField] private int mapWidth;

        [SerializeField] private SectionGeneratorSO tunnelGeneratorSo;
        [SerializeField] private SectionGeneratorSO startGeneratorSo;
        [SerializeField] private SectionGeneratorSO stationGeneratorSo;
        [SerializeField] private SectionGeneratorSO abandonedStationGeneratorSo;
        [SerializeField] private SectionGeneratorSO tunnelForkGeneratorSo;
        [SerializeField] private SectionGeneratorSO tunnelJoinGeneratorSo;

        public class MapGenerationContext
        {
            public MapPointsGen.Node currentNode;
            public MapPointsGen.Node lastNode;
        }
        MapGenerationContext _context;

        private SectionGeneratorSO GetGeneratorFromFeatureEnum(MapPointsGen.Feature feature) {
            switch (feature) {
                case MapPointsGen.Feature.TUNNEL_FORK:
                    return tunnelForkGeneratorSo;
                case MapPointsGen.Feature.TUNNEL_JOIN:
                    return tunnelJoinGeneratorSo;
                case MapPointsGen.Feature.START:
                    return startGeneratorSo;
                case MapPointsGen.Feature.STATION:
                    return stationGeneratorSo;
                case MapPointsGen.Feature.ABANDONED_STATION:
                    return abandonedStationGeneratorSo;
                case MapPointsGen.Feature.TUNNEL:
                    return tunnelGeneratorSo;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private List<TrainPathWaypoint> _waypoints = new();
        private int waypointLowestIndex = 0;
        private MapPointsGen.Map map;
        
        private void Awake() {
            GameManager.MapGeneration = this;
            _rebaseCounter = countUntilRebase;
        }

        private void Start() {
            MapPointsGen.Map m = new(mapHeight, mapWidth);
            _context = new();
            _context.currentNode = m.nodes[Random.Range(0, mapHeight), 0];
            Debug.Log(m.ToString());
            
            _sectionGeneratorSo = GetGeneratorFromFeatureEnum(_context.currentNode.feature);
            _sectionGeneratorSo.Initialize(_context);
            
            UpdateSections();
        }
        

        public SectionGeneratorSO _sectionGeneratorSo;
        private MapSection section;
        
        private bool CreateRandom() {
            if(!_sectionGeneratorSo.CanGenerate() && !_sectionGeneratorSo.HasFinished()) return false;
            
            if (_sectionGeneratorSo.HasFinished())
            {
                _context.lastNode = _context.currentNode;
                _context.currentNode =  _sectionGeneratorSo.GetNextNode();
                _sectionGeneratorSo = GetGeneratorFromFeatureEnum(_context.currentNode.feature);
                Debug.Log(_context.currentNode.feature);
                _sectionGeneratorSo.Initialize(_context);
            }
            
            section = _sectionGeneratorSo.Create();

            section.transform.parent = root;
            Transform end = IncomingSections.Count != 0 ? IncomingSections[IncomingSections.Count-1].end : transform;
            section.transform.position = end.position;
            section.transform.rotation = end.rotation;
            section.OnTrainCompleted += TrainReachedSectionEnd;
            IncomingSections.Add(section);
            
            _waypoints.AddRange(section.Waypoints);
            return true;
        }

        private void Update() {
            UpdateSections();
        }

        private void UpdateSections() {
            while (IncomingSections.Count < LoadedSectionCount && CreateRandom()) {
                    
            }

            while (PastSections.Count > LoadedSectionCount) {
                RemovePastSection();
            }
        }

        public void TrainReachedSectionEnd(bool updateAlert) {
            if(updateAlert)
                train.AlertSystem.SetNextAlert();
            var section = IncomingSections[0];
            IncomingSections.RemoveAt(0);
            PastSections.Add(section);
            
            HandleRebase();
        }

        private void RemovePastSection() {
            int amountToRemove = PastSections[0].Waypoints.Count;
            _waypoints.RemoveRange(0, amountToRemove);
            waypointLowestIndex += amountToRemove;
            PastSections[0].Remove();
            PastSections.RemoveAt(0);
        }

        public TrainPathWaypoint GetWaypoint(int index) {
            return _waypoints[index - waypointLowestIndex];
        }
        
        [SerializeField] private int countUntilRebase = 15;
        private int _rebaseCounter;
        private void HandleRebase() {
            if (_rebaseCounter == 0) {
                transform.position += train.transform.position * -1;
                
                _rebaseCounter = countUntilRebase;
            }

            _rebaseCounter--;
        }
    }
}