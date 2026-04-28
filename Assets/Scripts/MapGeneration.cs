using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Root {
    public class MapGeneration : MonoBehaviour {
        [Serializable] private class MapSectionListing {
            public int maxSpeed;
            public MapSection mapSection;
        }

        [SerializeField] private List<MapSectionListing> _sectionListings;
        private Dictionary<int, List<MapSection>> mapSections = new();


        private List<MapSection> IncomingSections = new();
        private List<MapSection> PastSections = new();
        [SerializeField] private int SectionCount;
        [SerializeField] private int minRepetition;
        [SerializeField] private int maxRepetition;
        public event Action<MapSection> OnAddedPiece;
        [SerializeField] private Transform root;
        [SerializeField] private Train train;
        [SerializeField] private List<GameObject> ShitToDelete;
        
        private void Awake() {
            foreach (var sectionListing in _sectionListings) {
                if (!mapSections.TryGetValue(sectionListing.maxSpeed, out var list)) {
                    list = new();
                    mapSections[sectionListing.maxSpeed] = list;
                }
                list.Add(sectionListing.mapSection);
            }
        }

        private void Update() {
            while (IncomingSections.Count < SectionCount) {
                CreateRandom();    
            }

            while (PastSections.Count > SectionCount) {
                RemovePastSection();
            }
        }

        private int currentRepetition;
        private int shit = 15;
        private MapSection sectionPrefab;
        private void CreateRandom() {
            if (currentRepetition == 0) {
                var speed = mapSections[mapSections.Keys.ElementAt(Random.Range(0, mapSections.Count))];
                sectionPrefab = speed[Random.Range(0, speed.Count)];
                currentRepetition = Random.Range(minRepetition, maxRepetition + 1);

                if (shit == 0) {
                    foreach (var shit in ShitToDelete) {
                        Destroy(shit);
                    }
                    Rebase();
                    shit = 2;
                }

                shit--;
            }
            currentRepetition--;
            
            var section = Instantiate(sectionPrefab, root);
            Transform end = IncomingSections.Count != 0 ? IncomingSections[IncomingSections.Count-1].end : transform;
            section.transform.position = end.position;
            section.transform.rotation = end.rotation;
            section.OnTrainCompleted += TrainReachedPoint;
            IncomingSections.Add(section);
            OnAddedPiece?.Invoke(section);
        }

        private void TrainReachedPoint() {
            var section = IncomingSections[0];
            IncomingSections.RemoveAt(0);
            PastSections.Add(section);
        }

        private void RemovePastSection() {
            PastSections[0].Remove();
            PastSections.RemoveAt(0);
        }

        private void Rebase() {
            transform.position += train.transform.position * -1;
        }
    }
}