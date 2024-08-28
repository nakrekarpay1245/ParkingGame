using _Game.Scripts._Abstracts;
using UnityEngine;

namespace _Game.Scripts.Car
{
    /// <summary>
    /// Manages the health system for the vehicle.
    /// </summary>
    public class CarDamageHandler : AbstractDamageableBase
    {
        private void Awake()
        {
            ServiceLocator.Register(this);
        }

        public override void Start()
        {
            _health = _maxHealth;
            OnHealthChanged?.Invoke(_health, _maxHealth);
        }

        public override void TakeDamage(float damageAmount)
        {
            _health -= damageAmount;
            OnHealthChanged?.Invoke(_health, _maxHealth);

            if (_health <= 0)
            {
                Die();
            }
        }

        public override void Die()
        {
            Debug.Log("Car Destroyed");
            // Handle death sequence
        }

        public override void Heal(float healAmount)
        {
            _health = Mathf.Min(_health + healAmount, _maxHealth);
            OnHealthChanged?.Invoke(_health, _maxHealth);
        }
    }
}