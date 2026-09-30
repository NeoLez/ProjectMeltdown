using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Assertions;

namespace Root.Managers {
    public abstract class Poolable : MonoBehaviour {
        [SerializeField, ReadOnly] private Poolable _prefab;
        private bool _wasInitialized;
        
        public void SetPrefab(Poolable prefab) {
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
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!UnityEditor.PrefabUtility.IsPartOfPrefabAsset(this))
                return;
            _prefab = this;
        }
#endif
    }
}