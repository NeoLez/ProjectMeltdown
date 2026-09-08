using UnityEngine;

namespace Root
{
    [RequireComponent (typeof (PackageVisual))]
    public class DeliveryPackageItem : PhysicalItem
    {
        public PackageData PackageData;

        [SerializeField] private PackageItemSo packageData;
        [SerializeField] private PackageClimateConditionsSO packageConditions;
        //[SerializeField] private GameObject[] packageStates;
        [SerializeField] PackageVisual _visuals;

        [HideInInspector]
        [SerializeField] private float damageMultiplier;

        private float _currentDurability;
        private int _currentValue;

        private bool _isInAffectionZone;
        private bool _timerHasEnded;
        private float _timer;
        private float _timerDuration;

        public PackageItemSo GetSO() => packageData;

        private void Start()
        {
            if(_visuals!=null)
            {
                _visuals = GetComponent<PackageVisual>();
            }
            //SetTimerDuration();

            PackageData.SetPackageOwner(this);
        }


        public void InitializePackageData(string id, int currentPrice, float currentDurability)
        {
            _currentDurability = currentDurability;

            _currentValue = currentPrice;

            PackageData = new PackageData(id, currentPrice, currentDurability);
            _visuals.SetDisplayValue(currentPrice);
        }

        public void SetPackageData(PackageData newData)
        {
            PackageData = newData;

            RefreshUI();
        }

        private void RefreshUI()
        {
            _visuals.SetDisplayValue(PackageData.Price);
        }

        private void Update()
        {
            if (!_isInAffectionZone) return;
            DrainLife();
        }

        public override void ShowFeedback(bool canShow)
        {
            base.ShowFeedback(canShow);

            _visuals.SetCanvasVisibility(canShow);      
        }

        #region Not Finished
        public void DrainLife()
        {
            if (!HasTimerEnded())
            {
                StartDrainingLife();
            }
            else
            {
                AffectLifeSpawn();
            }
        }

        //en base a la condicion, que se le vaya descontando un porcentaje
        private void AffectLifeSpawn()
        {
            if (_currentDurability <= 0)
            {
                KillPackage();
                return;
            }

            _currentDurability -= Mathf.Abs(damageMultiplier * Time.deltaTime);

            //Debug.Log(_currentLife);
            if (HasTimerEnded())
            {
                StartDrainingLife();
                return;
            }
            //aca hacer un switch dependiendo del estad, pueden ser 3

            //hacer el total dividido la vida del paquete
        }
        //ponerle valor a cada paquete en base a su condiconde vida útil
        private void AffectValue()
        {
            //sacar un porcentaje total de la vida, si se va disminuyendo, restarle un valor minimo en lo posible (balancear)
            //que el visualizador se vaya actualizando
            _visuals.SetDisplayValue(_currentValue);
        }

        private void StartDrainingLife()
        {
            if (_timer > _timerDuration)
            {
                _timerHasEnded = true;
                _timer = 0;
                return;
            }

            _timer += Time.deltaTime;
        }

        public bool HasTimerEnded()
        {
            return _timer <= 0 && _timerHasEnded;
        }

        private void KillPackage()
        {
            Debug.Log("perdio el paquete mucho valor");
        }

        public void IsInAffectionZone(bool state)
        {
            _isInAffectionZone = state;
        }

        public void SetTimerDuration()
        {
            _timerDuration = packageConditions.DamageCooldown;
        }
        #endregion

        public int GetPrice()
        {
            return _currentValue;
        }

        public float GetDurabilityState()
        {
            return _currentDurability;
        }

    }

    [System.Serializable]
    public class PackageData
    {
        private const string glyphs = "abcdefghijklmnopqrstuvwxyz0123456789";

        public string Id;
        public int Price;
        public float Durability;
        private DeliveryPackageItem _package;

        private string _packageID;
        private int _generatedPrice;

        public string PackageID => _packageID;
        public PackageData(string id, int price, float durability)
        {
            Id = id;
            Price = price;
            Durability = durability;
        }

        public void SetPackageOwner(DeliveryPackageItem package)
        {
            _package = package;
        }

        public int GeneratePackgePrice()
        {
            return _generatedPrice = Random.Range(_package.GetSO().MinPriceValue, _package.GetSO().MaxPriceValue);
        }

        public int GetGeneratedPrice()
        {
            return _generatedPrice;
        }

        public string GenerateUniqueID()
        {
            int charAmount = 8;
            for (int i = 0; i < charAmount; i++)
            {
                _packageID += glyphs[Random.Range(0, glyphs.Length)];
            }
            return _packageID;
        }
    }

}
