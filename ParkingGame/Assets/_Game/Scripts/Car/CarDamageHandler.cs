using _Game._Abstracts;
using _Game.Management;
using UnityEngine;
using DG.Tweening; // For animations with DOTween

namespace _Game.Car
{
    /// <summary>
    /// Manages the health system for the vehicle, allowing it to take damage, heal, and trigger a death sequence.
    /// Inherits from the AbstractDamageableBase to handle health-related functionality.
    /// </summary>
    public class CarDamageHandler : AbstractDamageableBase
    {
        [Header("Level Management")]
        [Tooltip("The manager responsible for handling level-related events.")]
        [SerializeField]
        private LevelManager _levelManager;

        /// <summary>
        /// Initialize the car's health and register the service.
        /// </summary>
        private void Awake()
        {
            _levelManager = ServiceLocator.Get<LevelManager>();
            ServiceLocator.Register(this);
        }

        /// <summary>
        /// Applies damage to the car and checks for death. Invokes the health changed event.
        /// </summary>
        /// <param name="damageAmount">The amount of damage to apply.</param>
        public override void TakeDamage(int damageAmount)
        {
            if (_isDead) return; // If already dead, ignore further damage

            base.TakeDamage(damageAmount); // Call base method to reduce health

            if (_health <= 0)
            {
                Die();
            }
        }

        /// <summary>
        /// Handles the car's death sequence, triggering level failure and an explosion animation.
        /// </summary>
        public override void Die()
        {
            if (_isDead) return; // Prevent multiple death triggers

            _isDead = true;
            Debug.Log("Car Destroyed");

            // Optional: DOTween for explosion or destruction effects
            transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
            {
                _levelManager.FailLevel(); // Trigger level failure after death animation
            });
        }

        /// <summary>
        /// Heals the car by a specified amount, ensuring health does not exceed the maximum.
        /// </summary>
        /// <param name="healAmount">The amount of health to restore.</param>
        public override void Heal(int healAmount)
        {
            if (_isDead) return; // Cannot heal if dead

            base.Heal(healAmount); // Heal the car and trigger health update
        }
    }
}