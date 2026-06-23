using UnityEngine;
using UnityEngine.SceneManagement;

namespace Root
{
    public class MenuController : MonoBehaviour
    {
        [SerializeField] private GameObject pantallaMenu;
        [SerializeField] private GameObject pantallaClases;
        [SerializeField] private UnityEngine.UI.Button[] classButtons;

        private int _selectedClass = 0;

        private void Start()
        {
            pantallaMenu.SetActive(true);
            pantallaClases.SetActive(false);

            SelectClass(0);
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

        public void SelectClass(int classNumber)
        {
            _selectedClass = classNumber;

            if (classButtons == null)
                return;

            for (int i = 0; i < classButtons.Length; i++)
            {
                if (classButtons[i] != null)
                {
                    classButtons[i].interactable = i != _selectedClass;
                }
            }
        }

        public void Confirmar()
        {
            GameManager.VeryUglyKitNumber = _selectedClass;
            SceneManager.LoadScene("Train 1");
        }

        public void Exit()
        {
            Application.Quit();
        }
    }
}