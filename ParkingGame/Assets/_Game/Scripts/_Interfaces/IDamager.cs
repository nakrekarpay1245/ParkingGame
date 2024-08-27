namespace _Game.Scripts._Interfaces
{
    /// <summary>
    /// Represents an object that can deal damage.
    /// </summary>
    public interface IDamager
    {
        /// <summary>
        /// Inflicts damage to a target that implements IDamageable.
        /// </summary>
        /// <param name="target">The target to damage.</param>
        /// <param name="damageAmount">The amount of damage to deal.</param>
        void DealDamage(IDamageable target, float damageAmount);
    }
}