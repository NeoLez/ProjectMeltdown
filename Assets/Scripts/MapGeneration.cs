using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Root {
    public class MapGeneration : MonoBehaviour {
        [Serializable] private class MapSectionListing {
            public int maxSpeed;
            public MapSection mapSection;
        }
        
        [Serializable] private class FeatureListing {
            public MapPointsGen.Feature feature;
            public MapSection mapSection;
        }

        [SerializeField] private List<MapSectionListing> _sectionListings;
        private Dictionary<int, List<MapSection>> mapSections = new();
        
        [SerializeField] private List<FeatureListing> _featureListings;
        private Dictionary<MapPointsGen.Feature, List<MapSection>> featureDictionary = new();

        private List<MapSection> IncomingSections = new();
        private List<MapSection> PastSections = new();
        [FormerlySerializedAs("SectionCount")] [SerializeField] private int LoadedSectionCount;
        
        public event Action<MapSection> OnAddedPiece;
        [SerializeField] private Transform root;
        [SerializeField] private Train train;


        [SerializeField] private int mapHeight;
        [SerializeField] private int mapWidth;

        private List<TrainPathWaypoint> _waypoints = new();
        private MapPointsGen.Map map;
        private MapPointsGen.Node currentNode;
        
        private void Awake() {
            foreach (var sectionListing in _sectionListings) {
                if (!mapSections.TryGetValue(sectionListing.maxSpeed, out var list)) {
                    list = new();
                    mapSections[sectionListing.maxSpeed] = list;
                }
                list.Add(sectionListing.mapSection);
            }

            foreach (var featureListing in _featureListings) {
                if (!featureDictionary.TryGetValue(featureListing.feature, out var list)) {
                    list = new();
                    featureDictionary[featureListing.feature] = list;
                }
                list.Add(featureListing.mapSection);
            }

            _rebaseCounter = countUntilRebase;
        }

        private void Start() {
            MapPointsGen.Map m = new(mapHeight, mapWidth);
            currentNode = m.nodes[Random.Range(0, mapHeight), 0];
            Debug.Log(m.ToString());
            
            var features = featureDictionary[currentNode.feature];
            nextSectionPrefab = features[Random.Range(0, features.Count)];
            trackSectionCounter = 0;
            currentRepetition = 0;
            //currentNode = currentNode.OutConnections[0];
        }


        [SerializeField] private int amountOfTrackSectionsBetweenFeatures = 8;
        private int trackSectionCounter = 0;
        private int currentRepetition;
        
        private MapSection sectionPrefab;
        private MapSection nextSectionPrefab;
        private void CreateRandom() {
            if (currentRepetition == 0) {
                sectionPrefab = nextSectionPrefab;
                if (trackSectionCounter == amountOfTrackSectionsBetweenFeatures) {
                    var features = featureDictionary[currentNode.feature];
                    nextSectionPrefab = features[Random.Range(0, features.Count)];
                    trackSectionCounter = 0;
                }
                else {
                    var speed = mapSections[mapSections.Keys.ElementAt(Random.Range(0, mapSections.Count))];
                    nextSectionPrefab = speed[Random.Range(0, speed.Count)];
                    trackSectionCounter++;
                }
                
                currentRepetition = Random.Range(sectionPrefab.minRepetition, sectionPrefab.maxRepetition + 1);
                train.AlertSystem.AddAlert(nextSectionPrefab.alert);
                
                HandleRebase();
            }
            currentRepetition--;
            
            var section = Instantiate(sectionPrefab, root);
            Transform end = IncomingSections.Count != 0 ? IncomingSections[IncomingSections.Count-1].end : transform;
            section.transform.position = end.position;
            section.transform.rotation = end.rotation;
            section.OnTrainCompleted += TrainReachedPoint;
            IncomingSections.Add(section);
            OnAddedPiece?.Invoke(section);
            if (currentRepetition == 0) {
                section.OnTrainCompleted += () => {
                    train.AlertSystem.SetNextAlert();
                };
            }
        }

        private void Update() {
            UpdateSections();
        }

        private void UpdateSections() {
            while (IncomingSections.Count < LoadedSectionCount) {
                CreateRandom();    
            }

            while (PastSections.Count > LoadedSectionCount) {
                RemovePastSection();
            }
        }

        private void TrainReachedPoint() {
            var section = IncomingSections[0];
            IncomingSections.RemoveAt(0);
            PastSections.Add(section);
            UpdateSections();
        }

        private void RemovePastSection() {
            PastSections[0].Remove();
            PastSections.RemoveAt(0);
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