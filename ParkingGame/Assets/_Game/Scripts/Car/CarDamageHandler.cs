using _Game.Scripts._Abstracts;

namespace _Game.Scripts.Car
{
    /// <summary>
    /// Manages the health and damage system of a car. Inherits from AbstractDamageableBase to handle health management.
    /// </summary>
    public class CarDamageHandler : AbstractDamageableBase
    {
        /// <summary>
        /// Sets the car's health to its maximum value.
        /// </summary>
        protected override void SetHealth()
        {
            base.SetHealth(); // Initialize health using the base method
            // Additional car-specific initialization if needed
        }

        /// <summary>
        /// Applies damage to the car and checks if it should die.
        /// </summary>
        /// <param name="damageAmount">The amount of damage to apply.</param>
        public override void TakeDamage(float damageAmount)
        {
            base.TakeDamage(damageAmount); // Handle damage using the base method
            // Additional car-specific damage handling if needed
        }

        /// <summary>
        /// Triggers the object's death sequence.
        /// </summary>
        public override void Die()
        {
            base.Die();
        }

        /// <summary>
        /// Heals the car by a certain amount, ensuring health does not exceed maximum health.
        /// </summary>
        /// <param name="healAmount">The amount of health to restore.</param>
        public override void Heal(float healAmount)
        {
            base.Heal(healAmount); // Handle healing using the base method
            // Additional car-specific healing if needed
        }
    }
}