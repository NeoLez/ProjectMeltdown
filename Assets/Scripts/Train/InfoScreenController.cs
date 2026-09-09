using UnityEngine;

namespace Root
{
    public class InfoScreenController : MonoBehaviour
    {
        [SerializeField] private Train train; 
        [SerializeField] private GameObject[] screens;

        private int currentIndex = 0;

        private void Awake() 
        {
            train.OnPowerLost += DisableAllScreens;
            train.OnPowerRestored += UpdateDisplay;
        }

        private void OnDestroy() 
        {
            train.OnPowerLost -= DisableAllScreens;
            train.OnPowerRestored -= UpdateDisplay;
        }

        private void Start()
        {
            UpdateDisplay();
        }

        public void NextInfo()
        {
            currentIndex = (currentIndex + 1) % screens.Length;
            UpdateDisplay();
        }

        public void PreviousInfo()
        {
            currentIndex = (currentIndex - 1 + screens.Length) % screens.Length;
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            if (screens == null || screens.Length == 0)
                return;

            for (int i = 0; i < screens.Length; i++)
            {
                screens[i].SetActive(i == currentIndex);
            }
        }

        private void DisableAllScreens() 
        {
            foreach (var screen in screens)
            {
                if (screen != null)
                    screen.SetActive(false);
            }
        }
    }
}