using System;
using UnityEngine;

namespace Root {
    public class Battery : MonoBehaviour {
        [SerializeField] private Transform energyVisuals;
        [SerializeField] private float visualsScale;
        public float energy;
        public float maxEnergy;

        private void Update() {
            energyVisuals.localScale = new Vector3(1, energy / maxEnergy * visualsScale, 1);
            energyVisuals.localPosition = - Vector3.up * energy / maxEnergy * visualsScale;
        }
    }
}