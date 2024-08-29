using _Game._Interfaces;
using UnityEngine;
using UnityEngine.Events;

namespace _Game._Abstracts
{
    /// <summary>
    /// Abstract base class for objects that have health, can take damage, and can die.
    /// Inherits from IDamageable to implement damage and health management functionalities.
    /// </summary>
    public abstract class AbstractDamageableBase : MonoBehaviour, IDamageable
    {
        [Header("Health Settings")]
        [Tooltip("Current health of the object.")]
        [SerializeField] protected int _health = 5;

        [Tooltip("Maximum health of the object.")]
        [SerializeField] protected int _maxHealth = 5;

        protected bool _isDead = false;

        public UnityAction<int, int> OnHealthChanged;

        public int MaxHealth { get => _maxHealth; set => _maxHealth = value; }

        public virtual void Start()
        {
            SetHealth();
        }

        /// <summary>
        /// Sets the object's health to its maximum value.
        /// </summary>
        protected virtual void SetHealth()
        {
            _health = _maxHealth;
            RaiseHealthChangedEvent(_health, _maxHealth);
        }

        /// <summary>
        /// Applies damage to the object and checks if it should die.
        /// </summary>
        /// <param name="damageAmount">The amount of damage to apply.</param>
        public virtual void TakeDamage(int damageAmount)
        {
            if (!_isDead)
            {
                _health -= damageAmount;

                if (_health <= 0)
                {
                    Die();
                }

                RaiseHealthChangedEvent(_health, _maxHealth);
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
            return !_isDead && _health > 0;
        }

        /// <summary>
        /// Raises the health changed event with the current and maximum health.
        /// </summary>
        /// <param name="health">The current health of the object.</param>
        /// <param name="maxHealth">The maximum health of the object.</param>
        protected virtual void RaiseHealthChangedEvent(int health, int maxHealth)
        {
            OnHealthChanged?.Invoke(health, maxHealth);
        }

        /// <summary>
        /// Heals the object by a certain amount, ensuring health does not exceed maximum health.
        /// This method is implemented as part of the IDamageable interface.
        /// </summary>
        /// <param name="healAmount">The amount of health to restore.</param>
        public virtual void Heal(int healAmount)
        {
            if (_isDead)
            {
                return;
            }

            _health += healAmount;
            if (_health > _maxHealth)
            {
                _health = _maxHealth; // Ensure health does not exceed max health
            }

            RaiseHealthChangedEvent(_health, _maxHealth);
        }
    }
}