using System.Collections.Generic;
using UnityEngine;

namespace Root
{
    [System.Serializable]
    public class ModifierController
    {
        [SerializeField] private float baseValue;
        private readonly List<InfectionModifier> activeModifiers = new List<InfectionModifier>();
        private float _modifiedValue;

        public float BaseValue
        {
            get => baseValue;
            set => baseValue = value;
        }

        public float ModifiedValue
        {
            get => _modifiedValue;
            set => _modifiedValue = value;
        }

        public float FinalValue
        {
            get
            {
                float flatSum = 0f;
                float percentageSum = 0f;

                for (int i = 0; i < activeModifiers.Count; i++)
                {
                    if (activeModifiers[i].type == TipoModificador.Flat)
                        flatSum += activeModifiers[i].value;
                    else
                        percentageSum += activeModifiers[i].value;
                }

                return (BaseValue + flatSum) * (1f + percentageSum);
            }
        }

        public float RestoreValue
        {
            get
            {
                float flatSum = 0f;
                float percentageSum = 0f;

                for (int i = 0; i < activeModifiers.Count; i++)
                {
                    if (activeModifiers[i].type == TipoModificador.Flat)
                        flatSum += -activeModifiers[i].value;
                    else
                        percentageSum += activeModifiers[i].value;
                }

                return (ModifiedValue + flatSum) * (1f + percentageSum);
            }
        }


        public void AddModifier(InfectionModifier mod)
        {
            if (!activeModifiers.Contains(mod))
                activeModifiers.Add(mod);
        }

        public void RemoveModifier(InfectionModifier mod)
        {
            activeModifiers.Remove(mod);
        }

    }

}
