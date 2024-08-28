using UnityEngine;
using _Game._Interfaces;

namespace _Game._Abstracts
{
    public abstract class AbstractDamagerBase : MonoBehaviour, IDamager
    {
        /// <summary>
        /// Deals damage to a target that implements IDamageable.
        /// </summary>
        /// <param name="target">The target to damage.</param>
        /// <param name="damageAmount">The amount of damage to deal.</param>
        public abstract void DealDamage(IDamageable target, float damageAmount);
    }
}