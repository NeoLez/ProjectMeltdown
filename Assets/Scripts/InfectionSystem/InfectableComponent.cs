using System;
using UnityEngine;

namespace Root
{
    public class InfectableComponent : MonoBehaviour, IInfectionable
    {
        public virtual void Infect(float currentValue) { }

        public virtual void RevertInfection(float currentValue) { }

        public float ApplyModifiers(float currentValue, Modifier[] customModifiers, float value)
        {
            for (int i = 0; i < customModifiers.Length; i++)
            {
                customModifiers[i].modifier.Evaluate(currentValue, value);

                if (!customModifiers[i].modifier.ChecksCondition) break;

                value = customModifiers[i].modifier.ActiveState ? value = customModifiers[i].controller.FinalValue : value = customModifiers[i].controller.BaseValue;
            }
            return value;
        }

        public float RevertModifiers(float currentValue, Modifier[] customModifiers, float value)
        {
            for (int i = 0; i < customModifiers.Length; i++)
            {
                customModifiers[i].modifier.Evaluate(currentValue);

                customModifiers[i].controller.ModifiedValue = value;
                if (!customModifiers[i].modifier.ChecksCondition) break;

                value = customModifiers[i].modifier.ActiveState ? value = customModifiers[i].controller.RestoreValue : value = customModifiers[i].controller.BaseValue;
            }
            return value;
        }

    }
}
