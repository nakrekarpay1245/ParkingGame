using UnityEngine;
using TMPro;
using DG.Tweening;
using System.Collections.Generic;
using _Game.Scripts.Car;

namespace _Game.Scripts.UI
{
    /// <summary>
    /// Manages UI elements like speed display and health bar for the vehicle.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        [Header("UI Elements")]
        [Tooltip("Text component for displaying the vehicle's speed.")]
        [SerializeField] private TextMeshProUGUI _speedText;

        [Tooltip("Prefab for representing a health segment.")]
        [SerializeField] private GameObject _healthSegmentPrefab;

        [Tooltip("Container for holding health segment instances.")]
        [SerializeField] private Transform _healthContainer;

        private List<GameObject> _healthSegments = new List<GameObject>();
        private float _currentSpeed;
        private float _speedVelocity = 0.0f;
        private CarDamageHandler _carDamageHandler;

        private void Start()
        {
            _carDamageHandler = ServiceLocator.Get<CarDamageHandler>();

            if (_carDamageHandler != null)
            {
                _carDamageHandler.OnHealthChanged += UpdateHealthBar;
                InitializeHealthSegments(_carDamageHandler.MaxHealth);
            }
        }

        private void Update()
        {
            var carController = ServiceLocator.Get<CarController>();

            if (carController != null)
            {
                float targetSpeed = carController.CarSpeed;
                _currentSpeed = Mathf.SmoothDamp(_currentSpeed, targetSpeed, ref _speedVelocity, 0.5f);
                UpdateSpeedText(_currentSpeed);
            }
        }

        private void InitializeHealthSegments(float maxHealth)
        {
            for (int i = 0; i < maxHealth; i++)
            {
                GameObject segment = Instantiate(_healthSegmentPrefab, _healthContainer);
                _healthSegments.Add(segment);
            }
        }

        private void UpdateHealthBar(float currentHealth, float maxHealth)
        {
            for (int i = 0; i < _healthSegments.Count; i++)
            {
                if (i < currentHealth)
                {
                    _healthSegments[i].SetActive(true);
                }
                else
                {
                    var segment = _healthSegments[i].GetComponent<UnityEngine.UI.Image>();
                    DOTween.Sequence()
                        .Append(segment.DOColor(Color.white, 0.1f))
                        .Append(segment.DOFade(0f, 0.2f))
                        .OnComplete(() => segment.gameObject.SetActive(false));
                }
            }
        }

        private void UpdateSpeedText(float speed)
        {
            _speedText.text = $"{Mathf.Abs(speed):0} km/h";
        }
    }
}
