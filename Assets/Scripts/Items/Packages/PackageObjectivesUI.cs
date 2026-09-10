using System.Collections;
using TMPro;
using UnityEngine;

namespace Root
{
    public class PackageObjectivesUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text texts;
        [SerializeField] private Canvas ui;

        //cambiar que con una tecla podes activar y dasativar este canvas, que cuando se complete la tarea que se tache o
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
