using UnityEngine;

namespace Root
{
    public abstract class EnemyClass : MonoBehaviour
    {
        public abstract void TakeDamage(float damage, float multiplier);

        public virtual void Death() { }
    }
}
