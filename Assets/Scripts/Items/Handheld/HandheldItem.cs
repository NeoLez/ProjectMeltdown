using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Root.Handheld {
    public class HandheldItem : MonoBehaviour {
        [NonSerialized] public ItemState state;
        public virtual void Use(InputAction.CallbackContext ctx) {
            Debug.Log("Used" + state.ItemSo.ItemName);
        }
    }
}