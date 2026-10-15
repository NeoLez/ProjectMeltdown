using System.Collections.Generic;
using UnityEngine;

namespace Root
{
    public class StationAccessController : MonoBehaviour
    {
        [SerializeField] private List<StationDoor> doors = new();
        [SerializeField] private List<KeycardState> keycards = new();
        [SerializeField] private bool autoCollect = true;
        [SerializeField] private MapSection mapSection;
        private int _accessCode;
        private bool _initialized;

        private void Start()
        {
            if (autoCollect)
                doors.AddRange(GetComponentsInChildren<StationDoor>(true));
            if (mapSection == null) {
                Debug.LogError("Map Section not set", this);
                return;
            }
            
            int seed = mapSection.GetMapSectionSeed();
            _accessCode = new System.Random(seed).Next(100000, 999999);

            foreach (StationDoor door in doors)
                if (door != null) door.SetStationCode(_accessCode);
            
            foreach (var keycard in keycards) {
                keycard.boundCode = _accessCode;
            }

            _initialized = true;
        }

        public void SubscribeKeycard(KeycardState state) {
            keycards.Add(state);
            if (_initialized) {
                state.boundCode = _accessCode;
                state.targetNode = mapSection.Node;
            }
        }
    }
}