using UnityEngine;

namespace Root
{
    [RequireComponent (typeof (PackageVisual))]
    public class PackageController : PhysicalItem
    {
        [SerializeField] private PackageItemSo packageData;
        [SerializeField] private PackageClimateConditionsSO packageConditions;
        //[SerializeField] private GameObject[] packageStates;
        [SerializeField] PackageVisual _visuals;

        [HideInInspector]
        [SerializeField] private float damageMultiplier;

        private float _currentDurability;
        private int _currentValue;

        //ver si agregarle unos multiplicadores por zonas mas "irradiadas" de esa condicion
        //(no olvidar que se pause cuando pausas el juego!!)
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
        }

        public void InitializePackageData(int currentPrice, float currentDurability)
        {
            _currentDurability = currentDurability;

            _currentValue = currentPrice;

            _visuals.SetDisplayValue(currentPrice);
            _visuals.SetPackageCondition(_currentDurability, packageData.PackageDurabilityLevel);
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

            _visuals.SetPackageCondition(_currentDurability, packageData.PackageDurabilityLevel);
            //aca hacer un switch dependiendo del estad, pueden ser 3

            //hacer el total dividido la vida del paquete
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

        public int GetPrice()
        {
            return _currentValue;
        }

        public float GetDurabilityState()
        {
            return _currentDurability;
        }

        //ponerle valor a cada paquete en base a su condiconde vida útil
        private void AffectValue()
        {
            //sacar un porcentaje total de la vida, si se va disminuyendo, restarle un valor minimo en lo posible (balancear)
            //que el visualizador se vaya actualizando
            _visuals.SetDisplayValue(_currentValue);
        }  
    }
}
