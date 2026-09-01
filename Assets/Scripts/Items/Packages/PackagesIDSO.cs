using UnityEngine;

namespace Root
{
    [CreateAssetMenu(menuName = "Items/Package Item", fileName = "Package")]
    public class PackagesIDSO : ScriptableObject
    {
        public string PackageID => _packageID;

        private string _packageID;

        private const string glyphs = "abcdefghijklmnopqrstuvwxyz0123456789";

        public bool IsValid => string.IsNullOrEmpty(_packageID);
        public string GenerateUniqueID()
        {
            int charAmount = Random.Range(0, 8);
            for (int i = 0; i < charAmount; i++)
            {
                _packageID += glyphs[Random.Range(0, glyphs.Length)];
            }
            return _packageID;
        }

    }
}
