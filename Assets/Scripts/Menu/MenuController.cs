using Root.Controller;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Localization.Settings;

namespace Root
{
    public class MenuController : MonoBehaviour
    {
        [SerializeField] private GameObject pantallaMenu;
        [SerializeField] private GameObject panelOpciones;
        [SerializeField] private GameObject pantallaClases;
        [SerializeField] private GameObject panelInfoClase;

        [SerializeField] private TMP_Text nombreClase;
        [SerializeField] private TMP_Text descripcionClase;
        [SerializeField] private TMP_Text comienzaCon;

        private int selectedClass;
        private bool loadingScene;

        private void Awake()
        {
            MouseHandler.ClearListAndSetToDefault();
            MouseHandler.RequestControl(CursorLockMode.None, true, this);
        }

        private void Start()
        {
            pantallaMenu.SetActive(true);
            pantallaClases.SetActive(false);
            panelInfoClase.SetActive(false);

            LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
        }

        private void OnDestroy()
        {
            LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
        }

        public void Play()
        {
            pantallaMenu.SetActive(false);
            pantallaClases.SetActive(true);
            panelInfoClase.SetActive(false);
        }

        public void AbrirOpciones()
        {
            pantallaMenu.SetActive(false);
            panelOpciones.SetActive(true);
        }

        public void CerrarOpciones()
        {
            panelOpciones.SetActive(false);
            pantallaMenu.SetActive(true);
        }

        public void Volver()
        {
            pantallaClases.SetActive(false);
            pantallaMenu.SetActive(true);
            panelInfoClase.SetActive(false);
        }

        public void SelectClass(int classNumber)
        {
            selectedClass = classNumber;

            panelInfoClase.SetActive(true);

            UpdateClassInfo();
        }

        private void UpdateClassInfo()
        {
            if (selectedClass < 0)
                return;

            switch (selectedClass)
            {
                case 0:
                    nombreClase.text = LocalizationSettings.StringDatabase.GetLocalizedString(
                        "Classes",
                        "class_mechanic_name"
                    );

                    descripcionClase.text = LocalizationSettings.StringDatabase.GetLocalizedString(
                        "Classes",
                        "class_mechanic_description"
                    );

                    comienzaCon.text = LocalizationSettings.StringDatabase.GetLocalizedString(
                        "Classes",
                        "class_mechanic_starts_with"
                    );
                    break;

                case 1:
                    nombreClase.text = LocalizationSettings.StringDatabase.GetLocalizedString(
                        "Classes",
                        "class_electrician_name"
                    );

                    descripcionClase.text = LocalizationSettings.StringDatabase.GetLocalizedString(
                        "Classes",
                        "class_electrician_description"
                    );

                    comienzaCon.text = LocalizationSettings.StringDatabase.GetLocalizedString(
                        "Classes",
                        "class_electrician_starts_with"
                    );
                    break;

                case 2:
                    nombreClase.text = LocalizationSettings.StringDatabase.GetLocalizedString(
                        "Classes",
                        "class_tycoon_name"
                    );

                    descripcionClase.text = LocalizationSettings.StringDatabase.GetLocalizedString(
                        "Classes",
                        "class_tycoon_description"
                    );

                    comienzaCon.text = LocalizationSettings.StringDatabase.GetLocalizedString(
                        "Classes",
                        "class_tycoon_starts_with"
                    );
                    break;
            }
        }

        private void OnLocaleChanged(UnityEngine.Localization.Locale locale)
        {
            if (panelInfoClase.activeSelf)
            {
                UpdateClassInfo();
            }
        }

        public void ConfirmPlay()
        {
            if (loadingScene)
                return;

            loadingScene = true;

            GameManager.VeryUglyKitNumber = selectedClass;

            var op = SceneManager.LoadSceneAsync("Train 1");
            MouseHandler.ClearListAndSetToDefault();
            op.allowSceneActivation = true;
        }

        public void Exit()
        {
            Application.Quit();
        }
    }
}