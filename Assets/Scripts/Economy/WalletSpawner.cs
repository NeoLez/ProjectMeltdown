using UnityEngine;

namespace Root
{
    public class WalletSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject walletPrefab;
        [SerializeField] private Transform walletAnchor;

        private GameObject _walletInstance;

        private void Start()
        {
            if (walletPrefab == null || walletAnchor == null)
                return;

            _walletInstance = Instantiate(
                walletPrefab,
                walletAnchor.position,
                walletAnchor.rotation,
                walletAnchor);
        }
    }
}