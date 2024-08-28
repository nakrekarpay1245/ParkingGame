using UnityEngine;

namespace _Game.Scripts.Car
{
    /// <summary>
    /// Manages the health system for the vehicle.
    /// </summary>
    public class CarDamageHandler : MonoBehaviour
    {
        public event System.Action<float, float> OnHealthChanged;

        [SerializeField] private float _maxHealth = 100f;
        private float _currentHealth;

        public float MaxHealth => _maxHealth;

        private void Awake()
        {
            ServiceLocator.Register(this);
        }

        private void Start()
        {
            _currentHealth = _maxHealth;
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
        }

        public void TakeDamage(float damageAmount)
        {
            _currentHealth -= damageAmount;
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);

            if (_currentHealth <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            Debug.Log("Vehicle Destroyed");
            // Handle death sequence
        }

        public void Heal(float healAmount)
        {
            _currentHealth = Mathf.Min(_currentHealth + healAmount, _maxHealth);
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
        }
    }
}