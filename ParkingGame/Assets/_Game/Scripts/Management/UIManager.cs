using UnityEngine;
using TMPro;
using _Game.Scripts._helpers;

namespace _Game.Scripts.UI
{
    /// <summary>
    /// UIManager handles UI elements related to the vehicle, such as displaying the vehicle's speed.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        [Header("UI Elements")]
        [Tooltip("The TextMeshPro component to display the vehicle's speed.")]
        [SerializeField] private TextMeshProUGUI _speedText;

        [Header("Speed Display Settings")]
        [Tooltip("The format for displaying the speed.")]
        [SerializeField] private string speedFormat = "{0:0} km/h";

        [Tooltip("Speed transition time in seconds.")]
        [SerializeField, Range(0.1f, 2f)] private float transitionTime = 0.5f;

        private float _currentSpeed;
        private float _targetSpeed;
        private float _speedVelocity = 0.0f; // Used by SmoothDamp

        private void Awake()
        {
            // Ensure CarController is found
            if (GlobalBinder.singleton.CarController == null)
            {
                Debug.LogError("CarController not found in the scene.");
            }
        }

        private void Update()
        {
            if (GlobalBinder.singleton.CarController != null)
            {
                // Get the current speed from CarController
                _targetSpeed = GlobalBinder.singleton.CarController.CarSpeed;

                // Smoothly transition the current speed
                _currentSpeed = Mathf.SmoothDamp(_currentSpeed, _targetSpeed, ref _speedVelocity, transitionTime);

                // Update speed text with the smoothed speed value
                UpdateSpeedText(_currentSpeed);
            }
        }

        /// <summary>
        /// Updates the speed text display.
        /// </summary>
        /// <param name="speed">The current speed of the vehicle.</param>
        private void UpdateSpeedText(float speed)
        {
            float absoluteCarSpeed = Mathf.Abs(speed);
            _speedText.text = string.Format(speedFormat, Mathf.RoundToInt(absoluteCarSpeed));
        }
    }
}