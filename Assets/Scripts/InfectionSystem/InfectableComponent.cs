using System;
using UnityEngine;

namespace Root
{
    public class InfectableComponent : MonoBehaviour, IInfectionable
    {
        public virtual void Infect(float currentValue)
        {           
        }
    }
}
