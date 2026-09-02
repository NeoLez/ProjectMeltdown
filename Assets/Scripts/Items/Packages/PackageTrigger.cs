using System.Collections;
using TMPro;
using UnityEngine;

namespace Root
{
    public class PackageTrigger : MonoBehaviour
    {
        [SerializeField] TMP_Text texts;
        [SerializeField] Canvas ui;

        public void ChangeUi(string text)
        {
            texts.text = text;
        }

        public void ChangeCanvas(bool enable)
        {
            ui.enabled = enable;
        }

        IEnumerator FadeDelay()
        {
            yield return new WaitForSeconds(2f);
        }
    }
}
