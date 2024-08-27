using _Game.Scripts._Interfaces;
using UnityEngine;
using UnityEngine.Events;

namespace _Game.Scripts._Abstracts
{
    /// <summary>
    /// Abstract base class for objects that have health, can take damage, and can die.
    /// Inherits from IDamageable to implement damage and health management functionalities.
    /// </summary>
    public abstract class AbstractDamageableBase : MonoBehaviour, IDamageable
    {
        [Header("Health Settings")]
        [Tooltip("Current health of the object.")]
        [SerializeField] private float health = 5f;

        [Tooltip("Maximum health of the object.")]
        [SerializeField] private float maxHealth = 5f;

        protected bool _isDead = false;

        public UnityAction<float, float> OnHealthChanged;

        public float MaxHealth { get => maxHealth; set => maxHealth = value; }

        public virtual void Start()
        {
            SetHealth();
        }

        /// <summary>
        /// Sets the object's health to its maximum value.
        /// </summary>
        protected virtual void SetHealth()
        {
            health = maxHealth;
            RaiseHealthChangedEvent(health, maxHealth);
        }

        /// <summary>
        /// Applies damage to the object and checks if it should die.
        /// </summary>
        /// <param name="damageAmount">The amount of damage to apply.</param>
        public virtual void TakeDamage(float damageAmount)
        {
            if (!_isDead)
            {
                health -= damageAmount;

                if (health <= 0)
                {
                    Die();
                }

                RaiseHealthChangedEvent(health, maxHealth);
            }
        }

        /// <summary>
        /// Triggers the object's death sequence.
        /// </summary>
        public virtual void Die()
        {
            if (!_isDead)
            {
                _isDead = true;
                // Additional death logic can be added here
            }
        }

        /// <summary>
        /// Checks if the object is still alive.
        /// </summary>
        /// <returns>True if the object is alive; otherwise, false.</returns>
        public bool IsAlive()
        {
            return !_isDead && health > 0;
        }

        /// <summary>
        /// Raises the health changed event with the current and maximum health.
        /// </summary>
        /// <param name="health">The current health of the object.</param>
        /// <param name="maxHealth">The maximum health of the object.</param>
        protected virtual void RaiseHealthChangedEvent(float health, float maxHealth)
        {
            OnHealthChanged?.Invoke(health, maxHealth);
        }

        /// <summary>
        /// Heals the object by a certain amount, ensuring health does not exceed maximum health.
        /// This method is implemented as part of the IDamageable interface.
        /// </summary>
        /// <param name="healAmount">The amount of health to restore.</param>
        public virtual void Heal(float healAmount)
        {
            if (_isDead)
            {
                return;
            }

            health += healAmount;
            if (health > maxHealth)
            {
                health = maxHealth; // Ensure health does not exceed max health
            }

            RaiseHealthChangedEvent(health, maxHealth);
        }
    }
}