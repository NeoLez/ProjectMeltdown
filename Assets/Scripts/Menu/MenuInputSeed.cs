using TMPro;
using UnityEngine;
using Random = System.Random;

namespace Root.Menu {
    public class MenuInputSeed : MonoBehaviour {
        [SerializeField] public TMP_InputField inputField;

        public void SetSeed() {
            GameManager.seed = TextToSeed(inputField.text);
        }

        private int TextToSeed(string text) {
            int hash = 23;
        
            foreach (char c in text) {
                hash = hash * 31 + c; 
            }
        
            return hash;
        }

        private void OnDestroy() {
            if (inputField.text == string.Empty) {
                GameManager.seed = new Random((int)Time.time).Next();
            }
        }
    }
}