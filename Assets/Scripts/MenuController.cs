using UnityEngine;
using UnityEngine.SceneManagement;
namespace Root
{
    public class MenuController : MonoBehaviour
    {
        
        public void Play()
        {
            SceneManager.LoadScene("Train 1");
            Debug.Log("a");
        }

        
        public void Exit()
        {
            Application.Quit();
            Debug.Log("b");
        }
    }
}
