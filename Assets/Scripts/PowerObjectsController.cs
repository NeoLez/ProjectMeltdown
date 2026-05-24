using System.Collections.Generic;
using UnityEngine;

namespace Root
{
    public class PowerObjectsController : MonoBehaviour
    {
        [SerializeField] private Train train;
        [SerializeField] private List<GameObject> objectsToDisable;
        [SerializeField] private List<Button> buttonsToLock;
        [SerializeField] private List<Button> buttonsToIgnore;

        private void Awake()
        {
            train.OnPowerLost += DisablePower;
            train.OnPowerRestored += EnablePower;
        }

        private void Start()
        {
            DisablePower();
        }

        private void OnDestroy()
        {
            train.OnPowerLost -= DisablePower;
            train.OnPowerRestored -= EnablePower;
        }

        private void DisablePower()
        {
            foreach (var obj in objectsToDisable)
            {
                if (obj != null)
                    obj.SetActive(false);
            }

            foreach (var button in buttonsToLock)
            {
                if (button != null && !buttonsToIgnore.Contains(button))
                {
                    button.Lock();
                }
            }
        }

        private void EnablePower()
        {
            foreach (var obj in objectsToDisable)
            {
                if (obj != null)
                    obj.SetActive(true);
            }

            foreach (var button in buttonsToLock)
            {
                if (button != null && !buttonsToIgnore.Contains(button))
                {
                    button.Unlock();
                }
            }
        }
    }
}