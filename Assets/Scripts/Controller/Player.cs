using System;
using UnityEngine;

namespace Root {
    public class Player : MonoBehaviour {
        private void Awake() {
            GameManager.Player = this;
        }
    }
}