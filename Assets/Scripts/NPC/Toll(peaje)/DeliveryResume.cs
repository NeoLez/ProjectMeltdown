using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Root
{
    public class DeliveryResume : MonoBehaviour
    {
        [SerializeField] private GiveMoneyEffect effect;
        [SerializeField] private NpcToll toll;

        [Header("Delivery Resume UI")]
        [SerializeField] private Canvas ticketCanvas;
        [SerializeField] private VerticalLayoutGroup resumeLayoutGroup;
        [SerializeField] private TMP_Text title;
        [SerializeField] private TMP_Text packagesAccepted;
        [SerializeField] private TMP_Text packagesDelivered;
        [SerializeField] private TMP_Text packagesLeft;
        [SerializeField] private TMP_Text discountForDelivery;
        [SerializeField] private TMP_Text penalizationForMissionFailure;
        [SerializeField] private TMP_Text totalTollPrice;
        private MissionsManager missionManager;

        private string priceLabel = "{0}$";
        private string porcentageLabel = "- {0}%";
        private string multiplyLabel = "x{0}";

        [Header("Packages Discount UI")]
        [SerializeField] private GameObject discountPrefab;
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private RectTransform pivot;

        private void Start()
        {
            missionManager = MissionsManager.Instance;

            effect.OnEffectExcuted += Initialize;

            ticketCanvas.enabled = false;
        }

        public void Initialize()
        {
            string price = string.Format(priceLabel, toll.AmountToRequest.ToString());
            title.text += price;

            packagesAccepted.text = missionManager.PackagesToDeliver().ToString();

            packagesDelivered.text = missionManager.PackagesDelivered().ToString();

            if (missionManager.UndeliveredPackages() > 1)
                packagesLeft.text = missionManager.UndeliveredPackages().ToString();
            else
                packagesLeft.text = missionManager.RemaingUndelivedPackages().ToString();

            if(!IsMissionIncomplete(price))
            {
                discountForDelivery.text = string.Format(porcentageLabel, (effect.allPackagesDeliveredDiscount * 100).ToString());
            }

            /*if (missionManager.PackagesDelivered() < missionManager.PackagesToDeliver())
            {
                int amount = missionManager.PackagesDelivered();

                for (int i = 0; i < amount; i++)
                {
                    GameObject obj = Instantiate(discountPrefab, pivot);
                    TMP_Text discountText = obj.GetComponentInChildren<TMP_Text>();

                    discountText.text = string.Format(porcentageLabel, (effect.discountPerPackage * 100).ToString());

                    RectTransform textRectTransform = obj.GetComponent<RectTransform>();

                    float childHeight = textRectTransform.rect.height;
                    Vector2 currentSize = rectTransform.sizeDelta;

                    float updatedHeight = currentSize.y + childHeight;
                    rectTransform.sizeDelta = new Vector2(currentSize.x, updatedHeight);
                    textRectTransform.anchoredPosition = pivot.anchoredPosition;
                }

                discountForDelivery.text = "";
            }
            else
            {
                discountForDelivery.text = effect.allPackagesDeliveredDiscount.ToString();
            }

            if (missionManager.PackagesToDeliver() == missionManager.UndeliveredPackages())
            {
                penalizationForMissionFailure.text = price + "\r\n";
                penalizationForMissionFailure.text += string.Format(multiplyLabel, missionManager.UndeliveredPackages().ToString());
            }*/

            int amountToPay = Mathf.RoundToInt(effect.CurrentMoney);
            totalTollPrice.text += string.Format(priceLabel, amountToPay.ToString());

            AjustBackgroundHeight();

            ticketCanvas.enabled = true;
        }

        public bool IsMissionIncomplete(string price)
        {
            if (missionManager.PackagesToDeliver() == missionManager.UndeliveredPackages())
            {
                penalizationForMissionFailure.text = price + "\r\n";
                penalizationForMissionFailure.text += string.Format(multiplyLabel, missionManager.UndeliveredPackages().ToString());
                return true;
            }

            if (missionManager.PackagesDelivered() < missionManager.PackagesToDeliver())
            {
                int amount = missionManager.PackagesDelivered();

                for (int i = 0; i < amount; i++)
                {
                    GameObject obj = Instantiate(discountPrefab, pivot);
                    TMP_Text discountText = obj.GetComponentInChildren<TMP_Text>();

                    discountText.text = string.Format(porcentageLabel, (effect.discountPerPackage * 100).ToString());

                    RectTransform textRectTransform = obj.GetComponent<RectTransform>();

                    float childHeight = textRectTransform.rect.height;
                    Vector2 currentSize = rectTransform.sizeDelta;

                    float updatedHeight = currentSize.y + childHeight;
                    rectTransform.sizeDelta = new Vector2(currentSize.x, updatedHeight);
                    textRectTransform.anchoredPosition = pivot.anchoredPosition;
                }

                discountForDelivery.text = "";
                return true;
            }

            return false;
        }

        private void AjustBackgroundHeight()
        {
            RectTransform backgroundRectTransform = GetComponentInChildren<RectTransform>();
            RectTransform verticalRecTransform = resumeLayoutGroup.GetComponent<RectTransform>();
            float layoutTotalHeight = resumeLayoutGroup.padding.top + resumeLayoutGroup.padding.bottom;

            foreach (RectTransform child in verticalRecTransform)
            {
                if (!child.gameObject.activeInHierarchy) continue;

                layoutTotalHeight += child.rect.height + resumeLayoutGroup.spacing;
            }
            float remainingSpace = backgroundRectTransform.rect.height - verticalRecTransform.rect.height;
            layoutTotalHeight += remainingSpace;

            if (layoutTotalHeight > backgroundRectTransform.rect.height)
            {
                //Debug.Log("Se desbordo");
                backgroundRectTransform.sizeDelta = new Vector2(backgroundRectTransform.sizeDelta.x, layoutTotalHeight + resumeLayoutGroup.spacing);
            }

        }

        private void OnDestroy()
        {
            effect.OnEffectExcuted -= Initialize;
        }
    }
}
