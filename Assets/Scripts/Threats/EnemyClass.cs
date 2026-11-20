using System;
using UnityEngine;

namespace Root
{
    public abstract class EnemyClass : MonoBehaviour
    {
        public Action<bool> OnDamaged;
        public abstract void TakeDamage(float damage);

        public virtual void Dead() { }
    }
}
