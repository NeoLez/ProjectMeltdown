using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Root
{
    [RequireComponent (typeof (PackageVisual))]
    public class DeliveryPackageItem : PhysicalItem
    {
        [FormerlySerializedAs("packageData")] [SerializeField] private PackageItemGenerationDataSo packageDataGenerator;
        [SerializeField] private PackageClimateConditionsSo packageConditions;
        //[SerializeField] private GameObject[] packageStates;
        //[SerializeField] PackageVisual _visuals;

        [HideInInspector]
        [SerializeField] private float damageMultiplier;

        private float _currentDurability;

        private bool _isInAffectionZone;
        private bool _timerHasEnded;
        private float _timer;
        private float _timerDuration;

        private void Start()
        {
            //if(_visuals!=null)
            //{
            //    _visuals = GetComponent<PackageVisual>();
            //}
            //SetTimerDuration();
        }


        //TODO: Deterministic number generation and maybe a way to set the packageDataGenerator from the outside so it can be changed at runtime?
        public void InitializePackageData()
        {
            var item = State;
            
            _currentDurability = Random.Range(packageDataGenerator.MinDurability, packageDataGenerator.MaxDurability);
            item.durability = _currentDurability;

            item.price = Random.Range(packageDataGenerator.MinPriceValue, packageDataGenerator.MaxPriceValue);
        }

        private void Update()
        {
            if (!_isInAffectionZone) return;
            DrainLife();
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
        //ponerle valor a cada paquete en base a su condiconde vida �til
        private void AffectValue()
        {
            //sacar un porcentaje total de la vida, si se va disminuyendo, restarle un valor minimo en lo posible (balancear)
            //que el visualizador se vaya actualizando
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
            return State.price;
        }

        public float GetDurabilityState()
        {
            return _currentDurability;
        }

        public PackageItemState State => itemState as PackageItemState;
        protected override bool IsStateTypeValid(ItemState state) {
            return state is PackageItemState;
        } 
    }
}
