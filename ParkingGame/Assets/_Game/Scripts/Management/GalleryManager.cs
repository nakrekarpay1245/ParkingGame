using UnityEngine;
using TMPro;
using _Game.Data;
using _Game.Car;
using _Game.UI;
using UnityEngine.UI;
using DG.Tweening;
using _Game.Management;
using System.Collections;

public class GalleryManager : MonoBehaviour
{
    [Header("Game Data")]
    [Tooltip("Reference to the game data ScriptableObject.")]
    [SerializeField] private GameData _gameData;

    [Header("UI Components")]
    [Header("Speed")]
    [Tooltip("Speed bar and text displaying the selected car's speed.")]
    [SerializeField] private Image _speedBar;
    [SerializeField] private TextMeshProUGUI _speedText;

    [Header("Acceleration")]
    [Tooltip("Acceleration bar and text displaying the selected car's acceleration.")]
    [SerializeField] private Image _accelerationBar;
    [SerializeField] private TextMeshProUGUI _accelerationText;

    [Header("Deceleration")]
    [Tooltip("Deceleration bar and text displaying the selected car's deceleration.")]
    [SerializeField] private Image _decelerationBar;
    [SerializeField] private TextMeshProUGUI _decelerationText;

    [Header("Steering")]
    [Tooltip("Steering angle bar and text displaying the selected car's steering.")]
    [SerializeField] private Image _steeringBar;
    [SerializeField] private TextMeshProUGUI _steeringText;

    [Header("Brake")]
    [Tooltip("Brake force bar and text displaying the selected car's braking power.")]
    [SerializeField] private Image _brakeBar;
    [SerializeField] private TextMeshProUGUI _brakeText;

    [Header("Drift")]
    [Tooltip("Handbrake drift multiplier bar and text displaying the selected car's drift factor.")]
    [SerializeField] private Image _driftBar;
    [SerializeField] private TextMeshProUGUI _driftText;

    [Header("Car Change Buttons")]
    [Tooltip("Button for selecting the previous car.")]
    [SerializeField] private CustomButton _previousCarButton;

    [Tooltip("Button for selecting the next car.")]
    [SerializeField] private CustomButton _nextCarButton;

    [Tooltip("Button for purchasing the car.")]
    [SerializeField] private CustomButton _purchaseCarButton;

    [Tooltip("Button for selecting the car.")]
    [SerializeField] private CustomButton _selectCarButton;

    [Header("Max Values for Bars")]
    [SerializeField] private float _maxSpeed = 200f;
    [SerializeField] private float _maxAcceleration = 10f;
    [SerializeField] private float _maxDeceleration = 10f;
    [SerializeField] private float _maxSteering = 45f;
    [SerializeField] private float _maxBrakeForce = 600f;
    [SerializeField] private float _maxDriftMultiplier = 10f;

    [Header("Car Model Display")]
    [Tooltip("The position where the car model will be instantiated.")]
    [SerializeField] private Transform _carDisplayPosition;

    [Header("Animation Settings")]
    [Tooltip("Duration for scaling down the UI elements.")]
    [SerializeField, Range(0.1f, 2.0f)] private float _scaleChangeDuration = 0.5f;

    [Tooltip("Ease type for the scale down animation.")]
    [SerializeField] private Ease _scaleDownEase = Ease.InBack;

    [Tooltip("Ease type for the scale up animation.")]
    [SerializeField] private Ease _scaleUpEase = Ease.OutBack;

    // Private Variables
    private CarConfigSO _currentCarConfig;
    private int _currentCarIndex;
    public GameObject _currentCarModelInstance;

    private EconomyManager _economyManager;

    private void Awake()
    {
        StartCoroutine(InitializeDependencies());
    }

    private void Start()
    {
        _currentCarIndex = _gameData.SelectedCarIndex;

        UpdateGallery();

        _previousCarButton.onButtonDown.AddListener(ShowPreviousCar);
        _nextCarButton.onButtonDown.AddListener(ShowNextCar);

        // Clear previous listeners
        _purchaseCarButton.onButtonDown.RemoveAllListeners();
        _purchaseCarButton.onButtonDown.AddListener(PurchaseCar);

        _selectCarButton.onButtonDown.RemoveAllListeners();
        _selectCarButton.onButtonDown.AddListener(SelectCar);
    }

    /// <summary>
    /// Coroutine that waits for dependencies to be registered before proceeding.
    /// </summary>
    private IEnumerator InitializeDependencies()
    {
        while (_economyManager == null)
        {
            _economyManager = ServiceLocator.Get<EconomyManager>();
            yield return null;
        }
    }

    /// <summary>
    /// Updates the gallery with the current selected car's data.
    /// </summary>
    private void UpdateGallery()
    {
        CarConfigSO carConfig = _gameData.CarList[_currentCarIndex].CarConfig;
        GameObject carModelPrefab = _gameData.CarList[_currentCarIndex].CarModelPrefab;

        Debug.Log("Config: " + carConfig.name);
        Debug.Log("Config: " + carModelPrefab.name);

        if (_currentCarConfig != carConfig)
        {
            _currentCarConfig = carConfig;

            // Destroy previous car model if exists
            if (_currentCarModelInstance != null)
            {
                GameObject previousCarModel = _currentCarModelInstance;
                _currentCarModelInstance = null;
                Debug.Log("Previous: " + previousCarModel.name);

                Sequence sequence = DOTween.Sequence();
                sequence.Append(previousCarModel.transform.DOScale(0, _scaleChangeDuration)
                    .SetEase(_scaleDownEase));
                sequence.AppendCallback(() => Destroy(previousCarModel));
                sequence.Play();
            }

            // Instantiate the new car model
            if (carModelPrefab != null)
            {
                _currentCarModelInstance = Instantiate(carModelPrefab, _carDisplayPosition.position,
                    _carDisplayPosition.rotation);

                _currentCarModelInstance.transform.localScale = Vector3.zero;

                _currentCarModelInstance.transform.DOScale(1, _scaleChangeDuration)
                    .SetEase(_scaleUpEase);

                Debug.Log("Instantiate car model Index: " + _currentCarIndex);
                Debug.Log("Instantiate car model: " + _currentCarModelInstance.name);
            }
        }

        // Update speed
        UpdateBarAndText(_speedBar, _speedText, _currentCarConfig.MaxSpeed, _maxSpeed,
            $"{_currentCarConfig.MaxSpeed} km/h");

        // Update acceleration
        UpdateBarAndText(_accelerationBar, _accelerationText, _currentCarConfig.AccelerationMultiplier,
            _maxAcceleration, $"{_currentCarConfig.AccelerationMultiplier}");

        // Update deceleration
        UpdateBarAndText(_decelerationBar, _decelerationText, _currentCarConfig.DecelerationMultiplier,
            _maxDeceleration, $"{_currentCarConfig.DecelerationMultiplier}");

        // Update steering angle
        UpdateBarAndText(_steeringBar, _steeringText, _currentCarConfig.MaxSteeringAngle, _maxSteering,
            $"{_currentCarConfig.MaxSteeringAngle}°");

        // Update brake force
        UpdateBarAndText(_brakeBar, _brakeText, _currentCarConfig.BrakeForce, _maxBrakeForce,
            $"{_currentCarConfig.BrakeForce} N");

        // Update handbrake drift multiplier
        UpdateBarAndText(_driftBar, _driftText, _currentCarConfig.HandbrakeDriftMultiplier,
            _maxDriftMultiplier, $"{_currentCarConfig.HandbrakeDriftMultiplier}");

        UpdateButtons();
    }

    /// <summary>
    /// Helper function to update UI bars and text values based on the current car config.
    /// </summary>
    private void UpdateBarAndText(Image bar, TextMeshProUGUI text, float currentValue, float maxValue,
        string displayText)
    {
        float normalizedValue = currentValue / maxValue;
        bar.fillAmount = normalizedValue;
        text.text = displayText;
    }

    /// <summary>
    /// Updates the button states based on the car's purchase and selection status.
    /// </summary>
    private void UpdateButtons()
    {
        var currentCar = _gameData.CarList[_currentCarIndex];

        if (!currentCar.IsPurchased)
        {
            _purchaseCarButton.gameObject.SetActive(true);
            _selectCarButton.gameObject.SetActive(false);
        }
        else if (_currentCarIndex != _gameData.SelectedCarIndex)
        {
            _purchaseCarButton.gameObject.SetActive(false);
            _selectCarButton.gameObject.SetActive(true);
        }
        else
        {
            _purchaseCarButton.gameObject.SetActive(false);
            _selectCarButton.gameObject.SetActive(false);
        }
    }

    private void ShowPreviousCar()
    {
        _currentCarIndex = (_currentCarIndex - 1 + _gameData.CarList.Count) % _gameData.CarList.Count;

        Debug.Log("previous: " + _currentCarIndex);

        UpdateGallery();
    }

    private void ShowNextCar()
    {
        _currentCarIndex = (_currentCarIndex + 1) % _gameData.CarList.Count;

        Debug.Log("Next: " + _currentCarIndex);

        UpdateGallery();
    }

    /// <summary>
    /// Attempts to purchase the current car.
    /// </summary>
    private void PurchaseCar()
    {
        int carPrice = _gameData.CarList[_currentCarIndex].Price;
        bool isCarCanBeAfford = _economyManager.CanAfford(carPrice);
        if (isCarCanBeAfford)
        {
            _economyManager.SpendCoins(carPrice);
            _gameData.BuyCar(_currentCarIndex);
            UpdateButtons();
        }
    }

    /// <summary>
    /// Selects the current car.
    /// </summary>
    private void SelectCar()
    {
        _gameData.SelectCar(_currentCarIndex);
        UpdateButtons();
    }
}