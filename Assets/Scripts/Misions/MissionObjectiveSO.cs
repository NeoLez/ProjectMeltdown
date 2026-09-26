using NUnit.Framework;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Localization;

namespace Root
{
    [CreateAssetMenu(fileName = "Mission", menuName = "SO/Missions")]
    public class MissionObjectiveSO : ScriptableObject
    {
        [field: SerializeField] public string Id { get; private set; }
        [field: SerializeField] public string Name { get; private set; }

        [field: SerializeField] public GameObject[] AvailablePackages;

        [Min(1)]
        [field: SerializeField] public int AmountOfPackages { get; private set; }
        //TODO: This needs a rework since the destinations are calculated at runtime
        [field: SerializeField] public string Destination { get; private set; }

        //TODO-Agregar Localization para el Destination y Conditions

    }
}
