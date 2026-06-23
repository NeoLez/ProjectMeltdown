using System.Collections.Generic;
using UnityEngine;

namespace SoundSystem
{
    public static class FootstepMaterialDatabase
    {

        public static readonly Dictionary<MaterialType, FootstepMaterialSO> Dictionary = new();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void LoadThings()
        {
            Dictionary[MaterialType.None] = null;
            Dictionary[MaterialType.Gritty] = Resources.Load<FootstepMaterialSO>("Footsteps/GrittySteps");
            Dictionary[MaterialType.Solid] = Resources.Load<FootstepMaterialSO>("Footsteps/SolidSteps");
            Dictionary[MaterialType.Wet] = Resources.Load<FootstepMaterialSO>("Footsteps/WetSteps");
        }
    }
}