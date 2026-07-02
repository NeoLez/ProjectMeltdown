using Root.Controller;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Root
{
    public class MenuController : MonoBehaviour
    {
        [SerializeField] private GameObject pantallaMenu;
        [SerializeField] private GameObject pantallaClases;

        private void Awake() {
            MouseHandler.ClearListAndSetToDefault();
            MouseHandler.RequestControl(CursorLockMode.None, true, this);
        }

        private void Start()
        {
            pantallaMenu.SetActive(true);
            pantallaClases.SetActive(false);
        }

        public void Play()
        {
            pantallaMenu.SetActive(false);
            pantallaClases.SetActive(true);
        }

        public void Volver()
        {
            pantallaClases.SetActive(false);
            pantallaMenu.SetActive(true);
        }

        private bool loadingScene;
        public void SelectClass(int classNumber) {
            if (loadingScene) return;
            
            loadingScene = true;
            GameManager.VeryUglyKitNumber = classNumber;
            var op = SceneManager.LoadSceneAsync("Train 1");
            op.allowSceneActivation = true;
            MouseHandler.ClearListAndSetToDefault();
        }

        public void Exit()
        {
            Application.Quit();
        }
    }
}