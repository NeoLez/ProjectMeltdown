using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Root
{
    public class PackageVisual : MonoBehaviour
    {
        [SerializeField] private Canvas displayCanvas;
        [SerializeField] private TMP_Text m_Text;
        [SerializeField] private Image m_Image;
        [SerializeField] private string format = "{0}$";
        [SerializeField] private float canvasHeight;

        Camera _playerCamera;

        private void Start()
        {
            _playerCamera = Camera.main;
            SetCanvasVisibility(false);
            SetCanvasHeight();
        }
        private void LateUpdate()
        {
            if (!displayCanvas.isActiveAndEnabled) return;

            FollowCamera();
        }

        public void SetDisplayValue(float value)
        {
            m_Text.text = string.Format(format, value);
        }

        public void SetCanvasVisibility(bool enable) //TODO-agregarle algun fade in fade out con el alfa
        {
            displayCanvas.enabled = enable;
        }

        public void SetPackageCondition(float currentAmount, float maxAmount)
        {
            m_Image.fillAmount = currentAmount / maxAmount;
        }

        public void FollowCamera()
        {
            Vector3 distance = displayCanvas.transform.position - _playerCamera.transform.position;
            displayCanvas.transform.rotation = Quaternion.LookRotation(distance);

        }
        private void SetCanvasHeight()
        {
            Vector3 targetPosition = transform.position;
            targetPosition.y += canvasHeight;

            displayCanvas.transform.position = targetPosition;
        }

    }
}
