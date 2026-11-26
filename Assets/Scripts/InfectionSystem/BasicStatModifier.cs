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

        private ModifierController _objectiveStat;
        private bool _isActive = false;

        public bool ActiveState => _isActive;
        public bool ChecksCondition { get; private set; }
       
        public void Intialize(ModifierController estadistica)
        {
            _objectiveStat = estadistica;
            _isActive = false;
        }

        public void Evaluate(float currentValue, float algo = 0) //TODO-Make it return a boolean instead
        {
            if (_objectiveStat == null) return;

            float currentPercentage = currentValue;

            bool reachesGoal = condition == ConditionThreshold.LessOrEqualThan
                ? currentPercentage <= percentageThreshold
                : currentPercentage >= percentageThreshold;

            if (reachesGoal && !_isActive)
            {
                _objectiveStat.AddModifier(modifier);
                _isActive = true;
                _objectiveStat.BaseValue = algo;
            }
            else if (!reachesGoal && _isActive)
            {
                _objectiveStat.RemoveModifier(modifier);
                _isActive = false;
            }
            ChecksCondition = reachesGoal;
        }

        public void Evaluate(float currentValue) 
        {
            if (_objectiveStat == null) return;

            float currentPercentage = (float)Math.Round(currentValue, 2);

            bool reachesGoal = condition == ConditionThreshold.LessOrEqualThan
               ? currentPercentage >= percentageThreshold
               : currentPercentage <= percentageThreshold;

            if (!reachesGoal && _isActive) //rever esto
            {
                _objectiveStat.RemoveModifier(modifier);
                _isActive = false;
            }

            ChecksCondition = reachesGoal;
            //Debug.Log(currentPercentage + "!" + percentageThreshold);
        }
    }
}
