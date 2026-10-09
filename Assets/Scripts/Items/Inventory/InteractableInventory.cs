using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Root {
    [RequireComponent(typeof(Inventory))]
    public class InteractableInventory : InteractableNormalCamera {

        private MeshFilter[] _mesh;
        [SerializeField] private Mesh[] _meshList;
        private bool _isShaking;
        [SerializeField] private float shakeDuration = 0.4f;
        [SerializeField] private float shakeMagnitude = 0.06f;
        private void Start()
        {
            _mesh = GetComponentsInChildren<MeshFilter>();
        }
        public override void Interact() {
            GameManager.PlayerInventoryUI.OpenInventory(GetComponent<Inventory>());
            VisualFeedback();
        }
        
        //TODO: Move all of this feedback logic to a separate script and call it from here
        private void VisualFeedback()
        {
            if (_meshList.Length > 0)
            {
                foreach (var mesh in _mesh) {
                    mesh.mesh = _meshList[1];
                }
            }
            StartCoroutine(ShakeFeedback());
        }
        private IEnumerator ShakeFeedback()
        {
            _isShaking = true;
            Vector3 basePosition = transform.localPosition;
            float elapsed = 0f;

            while (elapsed < shakeDuration)
            {
                elapsed += Time.deltaTime;
                float damper = 1f - Mathf.Clamp01(elapsed / shakeDuration);
                transform.localPosition = basePosition + Random.insideUnitSphere * shakeMagnitude * damper;
                yield return null;
            }

            transform.localPosition = basePosition;
            _isShaking = false;
        }
    }
}