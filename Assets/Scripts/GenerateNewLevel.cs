using UnityEngine;

namespace Root
{
    public class GenerateNewLevel : MonoBehaviour
    {
        [SerializeField] private LoadingScreen loadingScreen;

        private void Start()
        {
            loadingScreen = LoadingScreen.Instance;
        }

        private void OnTriggerEnter(Collider other)
        {
            if(loadingScreen != null)
            {
                loadingScreen.LoadScene("Train 1");
                GameManager.seed = new System.Random(GameManager.seed).Next();
            }
        }
    }
}
