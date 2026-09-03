using System.Collections;
using TMPro;
using UnityEngine;

namespace Root
{
    public class PackageObjectivesUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text texts;
        [SerializeField] private Canvas ui;

        public void ChangeUi(string text)
        {
            texts.text = text;
        }

        public void ChangeCanvas(bool enable)
        {
            ui.enabled = enable;
        }

    }
}
