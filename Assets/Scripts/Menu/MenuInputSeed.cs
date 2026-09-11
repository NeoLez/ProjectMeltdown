using Timers;
using TMPro;
using UnityEngine;
using Random = System.Random;

namespace Root.Menu {
    public class MenuInputSeed : MonoBehaviour {
        [SerializeField] public TMP_InputField inputField;

        public void SetSeed() {
            GameManager.seed = SeedUtils.TextToSeed(inputField.text);
        }

        private void OnDestroy() {
            if (inputField.text == string.Empty) {
                GameManager.seed = new Random((int)Time.time).Next();
            }
        }
    }
}