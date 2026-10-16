using UnityEngine;
using UnityEngine.InputSystem;

namespace Root.Handheld {
    public class HandheldItem : MonoBehaviour {
        public ItemState state;
        public virtual void Use(InputAction.CallbackContext ctx) {
            Debug.Log("Used" + state.ItemSo.ItemName);
        }
    }
}