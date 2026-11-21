using NaughtyAttributes;
using System;
using System.Collections.Generic;
using Timers;
using TMPro;
using UnityEngine;
using static Root.MapPointsGen;
using Logger = Root.Log.Logger;
using LogType = Root.Log.LogType;

namespace Root {
    public class MapGeneration : MonoBehaviour {
        private List<MapSection> IncomingSections = new();
        private List<MapSection> PastSections = new();
        [SerializeField] private int LoadedSectionCount;
        
        [SerializeField] private Transform root;
        [SerializeField] private Train train;

        public event Action OnBeforeRebase;
        
        [SerializeField] private int mapHeight;
        [SerializeField] private int mapWidth;

        [SerializeField] private SectionGeneratorSO tunnelGeneratorSo;
        [SerializeField] private SectionGeneratorSO startGeneratorSo;
        [SerializeField] private SectionGeneratorSO endGeneratorSo;
        [SerializeField] private SectionGeneratorSO stationGeneratorSo;
        [SerializeField] private SectionGeneratorSO shopStationGeneratorSo;
        [SerializeField] private SectionGeneratorSO chargeStationGeneratorSo;
        [SerializeField] private SectionGeneratorSO deliveryStationGeneratorSo;
        [SerializeField] private SectionGeneratorSO abandonedStationGeneratorSo;
        [SerializeField] private SectionGeneratorSO tunnelForkLeftGeneratorSo;
        [SerializeField] private SectionGeneratorSO tunnelForkRightGeneratorSo;
        [SerializeField] private SectionGeneratorSO tunnelJoinGeneratorSo;

        public class MapGenerationContext
        {
            public Node currentNode;
            public Node lastNode;
            public int currentSeed;
        }
        MapGenerationContext _context;

        public Node GetCurrentNode() {
            return _context.currentNode;
        }

        private SectionGeneratorSO GetGeneratorFromFeatureEnum(Feature feature) {
            switch (feature) {
                case Feature.TUNNEL_FORK_RIGHT:
                    return tunnelForkRightGeneratorSo;
                case Feature.TUNNEL_FORK_LEFT:
                    return tunnelForkLeftGeneratorSo;
                case Feature.TUNNEL_JOIN:
                    return tunnelJoinGeneratorSo;
                case Feature.START:
                    return startGeneratorSo;
                case Feature.STATION:
                    return stationGeneratorSo;
                case Feature.ABANDONED_STATION:
                    return abandonedStationGeneratorSo;
                case Feature.TUNNEL:
                    return tunnelGeneratorSo;
                case Feature.END:
                    return endGeneratorSo;
                case Feature.CHARGER:
                    return chargeStationGeneratorSo;
                case Feature.SHOP:
                    return shopStationGeneratorSo;
                case Feature.DELIVERY:
                    return deliveryStationGeneratorSo;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private List<TrainPathWaypoint> _waypoints = new();
        private int waypointLowestIndex = 0;
        public Map map;
        
        private void Awake() {
            _rebaseCounter = countUntilRebase;
            
            GameManager.MapGeneration = this;
            map = new(mapHeight, mapWidth, GameManager.seed);
            _context = new();
            _context.currentNode = map.GetNode(map.StartPosition.x, map.StartPosition.y);
            Logger.Log(map.ToString(), LogType.WorldGen);
        }

        private void Start() {
            RenderStationName();
            _sectionGeneratorSo = GetGeneratorFromFeatureEnum(_context.currentNode.feature);
            _sectionGeneratorSo.Initialize(_context);
            
            UpdateSections();
        }
        

        private SectionGeneratorSO _sectionGeneratorSo;
        private MapSection section;
        private int mapSectionID;
        private bool _hasReachedMapEnd;
        
        private bool CreateRandom() {
            if(!(_hasReachedMapEnd || _sectionGeneratorSo.CanGenerate() || _sectionGeneratorSo.HasFinished())) return false;
            
            if (_sectionGeneratorSo.HasFinished()) {
                var nextNode = _sectionGeneratorSo.GetNextNode();
                if (nextNode == null) {
                    _hasReachedMapEnd = true;
                    section.isStation = true;
                    return false;
                }

                _context.lastNode = _context.currentNode;
                _context.currentNode = nextNode ;
                RenderStationName();
                _context.currentSeed = GameManager.seed + SeedUtils.Combine(new [] {_context.currentNode.height, _context.currentNode.dist});
                mapSectionID = 0;
                _sectionGeneratorSo = GetGeneratorFromFeatureEnum(_context.currentNode.feature);
                Logger.Log($"{_context.currentNode.feature} {_context.currentNode.height} {_context.currentNode.dist}", LogType.WorldGen);
                _sectionGeneratorSo.Initialize(_context);
            }

            section = _sectionGeneratorSo.Create();
            section.Initialize(_context.currentNode, mapSectionID);
            mapSectionID++;

            section.transform.parent = root;
            Transform end = IncomingSections.Count != 0 ? IncomingSections[^1].end : (PastSections.Count != 0 ? PastSections[0].end : transform);
            section.transform.position = end.position;
            section.transform.rotation = end.rotation;
            section.OnTrainCompleted += TrainReachedSectionEnd;
            IncomingSections.Add(section);
            
            _waypoints.AddRange(section.GetWaypoints());
            return true;
        }

        private void Update() {
            UpdateSections();
        }

        public void UpdateSections() {
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
            int amountToRemove = PastSections[0].GetWaypoints().Count;
            _waypoints.RemoveRange(0, amountToRemove);
            waypointLowestIndex += amountToRemove;
            PastSections[0].Remove();
            PastSections.RemoveAt(0);
        }

        public TrainPathWaypoint GetWaypoint(int index) {
            if (_waypoints.Count <= index - waypointLowestIndex)
                return null;
            return _waypoints[index - waypointLowestIndex];
        }
        
        [SerializeField] private int countUntilRebase = 15;
        private int _rebaseCounter;
        private void HandleRebase() {
            if (_rebaseCounter == 0) {
                Logger.Log("Rebasing", LogType.WorldGen);
                if (train.IsStopped()) {
                    train.trainPosition.transform.position = Vector3.zero;
                }
                OnBeforeRebase?.Invoke();
                transform.position += train.trainPosition.position * -1;
                
                _rebaseCounter = countUntilRebase;
            }

            _rebaseCounter--;
        }
        public MapSection GetNextStation() // Pr�xima estaci�n en el recorrido, null si no hay
        {
            foreach (var section in IncomingSections)
            {
                if (section.isStation)
                    return section;
            }

            return null;
        }

        public bool IsTrainInStation() // True si el tren est� actualmente en una estaci�n
        {
            return IncomingSections.Count > 0 && IncomingSections[0].isStation;
        }

        [SerializeField] private RenderTexture stationNameRenderTexture;
        [SerializeField] private Canvas stationNameCanvas;
        [SerializeField] private TMP_Text stationNameText;
        private void RenderStationName()
        {
            stationNameText.text = $"{GetCurrentNode().line}{GetCurrentNode().dist}";
            stationNameCanvas.enabled = true;
            OneShotRenderSystem.Instance.Render(stationNameRenderTexture);
            stationNameCanvas.enabled = false;
        }

        [Button]
        private void PrintSeed() {
            Debug.Log(GameManager.seed);
        }
    }
}