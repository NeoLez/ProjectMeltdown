using UnityEngine;

namespace Root
{
    public class GenerateNewLevel : MonoBehaviour
    {
        [SerializeField] private LoadingScreen loadingScreen;

        private void Start()
        {
            loadingScreen = GameObject.FindAnyObjectByType<LoadingScreen>();
        }

        private void OnTriggerEnter(Collider other)
        {
            loadingScreen.LoadScene("Train 1");
            GameManager.seed = new System.Random(GameManager.seed).Next();
        }
    }
}
