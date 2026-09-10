using NUnit.Framework;
using UnityEngine;

namespace Root.Managers {
    public abstract class Poolable : MonoBehaviour {
        private Poolable _prefab;
        private bool _wasInitialized;
        
        public void SetPrefab(Poolable prefab) {
            Assert.IsNull(_prefab, "Cannot set prefab of poolable object twice");
            _prefab = prefab;
        }

        public Poolable GetPrefab() {
            return _prefab;
        }
        
        public virtual void TurnOn() {
            gameObject.SetActive(true);
        }

        public virtual void Initialize() {
            Assert.IsFalse(_wasInitialized, "Cannot initialize a poolable object twice");
            _wasInitialized = true;
        }

        public virtual void TurnOff() {
            gameObject.SetActive(false);
        }
    }
}