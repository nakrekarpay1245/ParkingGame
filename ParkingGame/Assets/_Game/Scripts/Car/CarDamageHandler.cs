using _Game._Abstracts;
using _Game.Management;
using UnityEngine;

namespace _Game.Car
{
    /// <summary>
    /// Manages the health system for the vehicle.
    /// </summary>
    public class CarDamageHandler : AbstractDamageableBase
    {
        private LevelManager _levelManager;

        private void Awake()
        {
            ServiceLocator.Register(this);
        }

        public override void Start()
        {
            _levelManager = ServiceLocator.Get<LevelManager>();

            _health = _maxHealth;
            OnHealthChanged?.Invoke(_health, _maxHealth);
        }

        public override void TakeDamage(int damageAmount)
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
            _levelManager.FailLevel();
        }

        public override void Heal(int healAmount)
        {
            _health = Mathf.Min(_health + healAmount, _maxHealth);
            OnHealthChanged?.Invoke(_health, _maxHealth);
        }
    }
}