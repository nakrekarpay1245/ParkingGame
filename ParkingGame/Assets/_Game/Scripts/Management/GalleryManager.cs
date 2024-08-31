using UnityEngine;
using TMPro;
using _Game.Data;
using _Game.Car;
using _Game.UI;
using UnityEngine.UI;
using DG.Tweening;
using _Game.Management;
using System.Collections;

/// <summary>
/// Manages the car gallery, updates UI elements based on car stats, and handles car selection and purchasing logic.
/// </summary>
public class GalleryManager : MonoBehaviour
{
    [Header("Game Data")]
    [Tooltip("Reference to the game data ScriptableObject.")]
    [SerializeField] private GameData _gameData;

    [Header("UI Components")]
    [Tooltip("Speed bar and text displaying the selected car's speed.")]
    [SerializeField] private Image _speedBar;
    [SerializeField] private TextMeshProUGUI _speedText;

    [Tooltip("Acceleration bar and text displaying the selected car's acceleration.")]
    [SerializeField] private Image _accelerationBar;
    [SerializeField] private TextMeshProUGUI _accelerationText;

    [Tooltip("Deceleration bar and text displaying the selected car's deceleration.")]
    [SerializeField] private Image _decelerationBar;
    [SerializeField] private TextMeshProUGUI _decelerationText;

    [Tooltip("Steering angle bar and text displaying the selected car's steering angle.")]
    [SerializeField] private Image _steeringBar;
    [SerializeField] private TextMeshProUGUI _steeringText;

    [Tooltip("Brake force bar and text displaying the selected car's braking power.")]
    [SerializeField] private Image _brakeBar;
    [SerializeField] private TextMeshProUGUI _brakeText;

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

    [Header("Max Values for Stats")]
    [SerializeField] private float _maxSpeed = 200f;
    [SerializeField] private float _maxAcceleration = 10f;
    [SerializeField] private float _maxDeceleration = 10f;
    [SerializeField] private float _maxSteering = 45f;
    [SerializeField] private float _maxBrakeForce = 600f;
    [SerializeField] private float _maxDriftMultiplier = 10f;

    [Header("Car Display Settings")]
    [Tooltip("The position where the car model will be instantiated.")]
    [SerializeField] private Transform _carPoint;

    [Header("Animation Settings")]
    [Tooltip("Duration for scaling UI elements during animations.")]
    [SerializeField, Range(0.1f, 2.0f)] private float _scaleChangeDuration = 0.5f;

    [Tooltip("Ease type for the scale-down animation.")]
    [SerializeField] private Ease _scaleDownEase = Ease.InBack;

    [Tooltip("Ease type for the scale-up animation.")]
    [SerializeField] private Ease _scaleUpEase = Ease.OutBack;

    private CarConfigSO _currentCarConfig;
    private int _currentCarIndex;
    private GameObject _currentCarModelInstance;
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

        SetupPurchaseAndSelectionButtons();
    }

    /// <summary>
    /// Coroutine that waits for dependencies such as the EconomyManager to be initialized.
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
    /// Updates the gallery based on the currently selected car configuration and displays the appropriate UI data.
    /// </summary>
    private void UpdateGallery()
    {
        var carConfig = _gameData.CarList[_currentCarIndex].CarConfig;
        var carModelPrefab = _gameData.CarList[_currentCarIndex].CarModelPrefab;
        var isPurchased = _gameData.CarList[_currentCarIndex].IsPurchased;
        var carPrice = _gameData.CarList[_currentCarIndex].Price;

        if (_currentCarConfig != carConfig)
        {
            _currentCarConfig = carConfig;
            HandleCarModelSwitch(carModelPrefab);
        }

        UpdateCarStatsUI();
        UpdateButtons(isPurchased, carPrice);
    }

    /// <summary>
    /// Handles switching the car model in the display, animating the transition using DOTween.
    /// </summary>
    private void HandleCarModelSwitch(GameObject newCarModelPrefab)
    {
        if (_currentCarModelInstance != null)
        {
            var previousCarModel = _currentCarModelInstance;
            _currentCarModelInstance = null;

            Sequence sequence = DOTween.Sequence();
            sequence.Append(previousCarModel.transform.DOScale(0, _scaleChangeDuration).SetEase(_scaleDownEase));
            sequence.AppendCallback(() => Destroy(previousCarModel));
            sequence.Play();
        }

        if (newCarModelPrefab != null)
        {
            _currentCarModelInstance = Instantiate(newCarModelPrefab, _carPoint.position, _carPoint.rotation);
            _currentCarModelInstance.transform.localScale = Vector3.zero;
            _currentCarModelInstance.transform.DOScale(1, _scaleChangeDuration).SetEase(_scaleUpEase);
            _currentCarModelInstance.transform.parent = _carPoint;
        }
    }

    /// <summary>
    /// Updates the car stats (e.g., speed, acceleration) displayed in the UI.
    /// </summary>
    private void UpdateCarStatsUI()
    {
        UpdateBarAndText(_speedBar, _speedText, _currentCarConfig.MaxSpeed, _maxSpeed, $"{_currentCarConfig.MaxSpeed} km/h");
        UpdateBarAndText(_accelerationBar, _accelerationText, _currentCarConfig.AccelerationMultiplier, _maxAcceleration, $"{_currentCarConfig.AccelerationMultiplier}");
        UpdateBarAndText(_decelerationBar, _decelerationText, _currentCarConfig.DecelerationMultiplier, _maxDeceleration, $"{_currentCarConfig.DecelerationMultiplier}");
        UpdateBarAndText(_steeringBar, _steeringText, _currentCarConfig.MaxSteeringAngle, _maxSteering, $"{_currentCarConfig.MaxSteeringAngle}°");
        UpdateBarAndText(_brakeBar, _brakeText, _currentCarConfig.BrakeForce, _maxBrakeForce, $"{_currentCarConfig.BrakeForce} N");
        UpdateBarAndText(_driftBar, _driftText, _currentCarConfig.HandbrakeDriftMultiplier, _maxDriftMultiplier, $"{_currentCarConfig.HandbrakeDriftMultiplier}");
    }

    /// <summary>
    /// Updates the UI bars and text based on a car's stat value.
    /// </summary>
    private void UpdateBarAndText(Image bar, TextMeshProUGUI text, float currentValue, float maxValue, string displayText)
    {
        bar.fillAmount = currentValue / maxValue;
        text.text = displayText;
    }

    /// <summary>
    /// Updates the purchase and selection button visibility based on the car's status.
    /// </summary>
    private void UpdateButtons(bool isPurchased, int carPrice)
    {
        if (!isPurchased)
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

    /// <summary>
    /// Shows the previous car in the gallery.
    /// </summary>
    private void ShowPreviousCar()
    {
        _currentCarIndex = (_currentCarIndex - 1 + _gameData.CarList.Count) % _gameData.CarList.Count;
        UpdateGallery();
    }

    /// <summary>
    /// Shows the next car in the gallery.
    /// </summary>
    private void ShowNextCar()
    {
        _currentCarIndex = (_currentCarIndex + 1) % _gameData.CarList.Count;
        UpdateGallery();
    }

    /// <summary>
    /// Sets up the purchase and selection button listeners.
    /// </summary>
    private void SetupPurchaseAndSelectionButtons()
    {
        _purchaseCarButton.onButtonDown.RemoveAllListeners();
        _purchaseCarButton.onButtonDown.AddListener(PurchaseCar);

        _selectCarButton.onButtonDown.RemoveAllListeners();
        _selectCarButton.onButtonDown.AddListener(SelectCar);
    }

    /// <summary>
    /// Attempts to purchase the current car if the player can afford it.
    /// </summary>
    private void PurchaseCar()
    {
        int carPrice = _gameData.CarList[_currentCarIndex].Price;
        if (_economyManager.CanAfford(carPrice))
        {
            _economyManager.SpendCoins(carPrice);
            _gameData.BuyCar(_currentCarIndex);
            UpdateButtons(true, carPrice);
        }
    }

    /// <summary>
    /// Selects the current car as the player's choice.
    /// </summary>
    private void SelectCar()
    {
        _gameData.SelectCar(_currentCarIndex);
        UpdateButtons(true, 0);
    }
}