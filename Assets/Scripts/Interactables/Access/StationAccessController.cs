using System.Collections.Generic;
using UnityEngine;

namespace Root
{
    public class StationAccessController : MonoBehaviour
    {
        [SerializeField] private List<StationDoor> doors = new List<StationDoor>();
        [SerializeField] private bool autoCollect = true;
        [SerializeField] private MapSection mapSection;

        private void Start()
        {
            if (autoCollect)
                doors.AddRange(GetComponentsInChildren<StationDoor>(true));

            int seed = mapSection != null ? mapSection.GetMapSectionSeed() : Random.Range(int.MinValue, int.MaxValue);
            int code = new System.Random(seed).Next(100000, 999999);

            foreach (StationDoor door in doors)
                if (door != null) door.SetStationCode(code);
        }
    }
}