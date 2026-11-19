using System;
using UnityEngine;

namespace Root
{
    public enum ConditionThreshold
    {
        LessOrEqualThan, 
        HigherOrEqualThan
    }

    [System.Serializable]
    public class BasicStatModifier
    {
        [Range(0f, 1f)]
        public float percentageThreshold = 0.3f;
        public ConditionThreshold condition = ConditionThreshold.LessOrEqualThan;
        public InfectionModifier modifier;

        private ModifierController _estadisticaObjetivo;
        private bool _isActive = false;

        public bool ActiveState => _isActive;
        public bool ChecksCondition { get; private set; }
       
        public void Intialize(ModifierController estadistica)
        {
            _estadisticaObjetivo = estadistica;
            _isActive = false;
        }

        public void Evaluate(float currentValue)
        {
            if (_estadisticaObjetivo == null) return;

            //float currentPercentage = (currentValue / maxValue) * 100;
            float currentPercentage = currentValue;

            bool reachesGoal = condition == ConditionThreshold.LessOrEqualThan
                ? currentPercentage <= percentageThreshold
                : currentPercentage >= percentageThreshold;

            ChecksCondition = reachesGoal;
            if (reachesGoal && !_isActive)
            {
                _estadisticaObjetivo.AddModifier(modifier);
                _isActive = true;
            }
            else if (!reachesGoal && _isActive)
            {
                _estadisticaObjetivo.RemoveModifier(modifier);
                _isActive = false;
            }

        }


    }
}
