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

            int seed = mapSection != null ? mapSection.GetMapSectionSeed() : Random.Range(int.MinValue, int.MaxValue);
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
            if (_initialized)
                state.boundCode = _accessCode;
        }
    }
}