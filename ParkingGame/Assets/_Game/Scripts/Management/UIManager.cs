using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening; // Import DOTween
using _Game.Scripts.Car;
using _Game.Scripts._helpers; // Adjust the namespace if needed
using System.Collections.Generic; // For List

namespace _Game.Scripts.UI
{
    /// <summary>
    /// UIManager handles UI elements related to the vehicle, such as displaying the vehicle's speed and health.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        [Header("UI Elements")]
        [Header("Speed Elements")]
        [Tooltip("The TextMeshPro component to display the vehicle's speed.")]
        [SerializeField] private TextMeshProUGUI _speedText;

        [Header("Health Elements")]
        [Tooltip("The prefab representing one segment of health.")]
        [SerializeField] private GameObject _healthSegmentPrefab;

        [Tooltip("The parent object where health segments will be instantiated.")]
        [SerializeField] private Transform _healthContainer;

        [Header("Speed Display Settings")]
        [Tooltip("The format for displaying the speed.")]
        [SerializeField] private string _speedFormat = "{0:0} km/h";

        [Tooltip("Speed transition time in seconds.")]
        [SerializeField, Range(0.1f, 2f)] private float _transitionTime = 0.5f;

        private float _currentSpeed;
        private float _targetSpeed;
        private float _speedVelocity = 0.0f; // Used by SmoothDamp
        private List<GameObject> _healthSegments = new List<GameObject>();

        private void Awake()
        {
            // Subscribe to health changed event
            GlobalBinder.singleton.CarDamageHandler.OnHealthChanged += UpdateHealthBar;

            // Initialize health segments
            InitializeHealthSegments(GlobalBinder.singleton.CarDamageHandler.MaxHealth);
        }

        private void Update()
        {
            // Get the current speed from CarController
            _targetSpeed = GlobalBinder.singleton.CarController.CarSpeed;

            // Smoothly transition the current speed
            _currentSpeed = Mathf.SmoothDamp(_currentSpeed, _targetSpeed, ref _speedVelocity, _transitionTime);

            // Update speed text with the smoothed speed value
            UpdateSpeedText(_currentSpeed);
        }

        /// <summary>
        /// Initializes the health bar by creating health segments based on MaxHealth.
        /// </summary>
        /// <param name="maxHealth">Maximum health of the vehicle.</param>
        private void InitializeHealthSegments(float maxHealth)
        {
            for (int i = 0; i < maxHealth; i++)
            {
                // Instantiate a health segment and add it to the list
                GameObject segment = Instantiate(_healthSegmentPrefab, _healthContainer);
                _healthSegments.Add(segment);
            }
        }

        /// <summary>
        /// Updates the health bar based on the health change event.
        /// </summary>
        /// <param name="currentHealth">The current health of the vehicle.</param>
        /// <param name="maxHealth">The maximum health of the vehicle.</param>
        private void UpdateHealthBar(float currentHealth, float maxHealth)
        {
            // Ensure the health segments are updated based on the current health
            for (int i = 0; i < _healthSegments.Count; i++)
            {
                if (i < currentHealth)
                {
                    _healthSegments[i].SetActive(true); // Activate health segment
                }
                else
                {
                    Image segment = _healthSegments[i].GetComponent<Image>();
                    Sequence sequence = DOTween.Sequence();
                    Color originalColor = segment.color;

                    sequence.Append(segment.DOColor(Color.white, 0.1f)) // White flash
                            .Append(segment.DOColor(originalColor, 0.1f)) // Return to original color
                            .Append(segment.DOColor(Color.white, 0.1f)) // White flash again
                            .Append(segment.DOColor(originalColor, 0.1f)) // Return to original color
                            .AppendInterval(0.2f) // Wait before disappearing
                            .Append(segment.DOFade(0, 0.2f)) // Fade out
                            .OnComplete(() => segment.gameObject.SetActive(false)); // Hide after fade
                }
            }
        }

        /// <summary>
        /// Updates the speed text display.
        /// </summary>
        /// <param name="speed">The current speed of the vehicle.</param>
        private void UpdateSpeedText(float speed)
        {
            float absoluteCarSpeed = Mathf.Abs(speed);
            _speedText.text = string.Format(_speedFormat, Mathf.RoundToInt(absoluteCarSpeed));
        }
    }
}