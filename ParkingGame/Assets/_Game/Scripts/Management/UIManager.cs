using UnityEngine;
using TMPro;
using DG.Tweening;
using _Game.Management;
using _Game.Car;
using System.Collections;
using System.Collections.Generic;

namespace _Game.UI
{
    /// <summary>
    /// Manages the UI elements including health, speed display, result screens, and their associated animations.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        [Header("Result Screen Elements")]
        [Tooltip("Result screen that appears after level completion or failure.")]
        [SerializeField] private GameObject _resultScreen;
        [Tooltip("Text field to display the result message.")]
        [SerializeField] private TextMeshProUGUI _resultText;
        [Tooltip("Menu button.")]
        [SerializeField] private CustomButton _menuButton;
        [Tooltip("Next level button.")]
        [SerializeField] private CustomButton _nextButton;
        [Tooltip("Restart button.")]
        [SerializeField] private CustomButton _restartButton;

        [Header("Game UI Elements")]
        [Tooltip("Car displayer UI element.")]
        [SerializeField] private GameObject _carDisplayer;
        [Tooltip("Car health displayer UI element.")]
        [SerializeField] private GameObject _carHealthDisplayer;
        [Tooltip("Control buttons UI element.")]
        [SerializeField] private GameObject _controlButtons;
        [Tooltip("Speed displayer UI element.")]
        [SerializeField] private GameObject _speedDisplayer;

        [Header("Messages")]
        [Tooltip("Message to display when the level is completed.")]
        [SerializeField] private string _levelCompleteMessage = "Level Complete!";
        [Tooltip("Message to display when the level fails.")]
        [SerializeField] private string _levelFailMessage = "Level Failed";

        [Header("DOTween Animation Settings")]
        [Tooltip("Duration for element animations.")]
        [SerializeField, Range(0.1f, 2f)] private float _elementAnimationDuration = 0.3f;
        [Tooltip("Ease type for opening UI elements.")]
        [SerializeField] private Ease _openElementAnimationEase = Ease.OutBounce;
        [Tooltip("Ease type for closing UI elements.")]
        [SerializeField] private Ease _closeElementAnimationEase = Ease.InBounce;

        [Header("Speed Display")]
        [Tooltip("Text component to display the vehicle's speed.")]
        [SerializeField] private TextMeshProUGUI _speedText;

        [Header("Health Display")]
        [Tooltip("Prefab for health segments.")]
        [SerializeField] private GameObject _healthSegmentPrefab;
        [Tooltip("Container for health segments.")]
        [SerializeField] private Transform _healthContainer;

        private List<GameObject> _healthSegments = new List<GameObject>();
        private float _currentSpeed;
        private float _speedVelocity = 0.0f;

        private CarDamageHandler _carDamageHandler;
        private LevelManager _levelManager;

        private void Awake()
        {
            StartCoroutine(InitializeDependencies());
        }

        private void Start()
        {
            InitializeUI();
            SetButtonEvents();
        }

        /// <summary>
        /// Coroutine that waits for dependencies to be registered before proceeding.
        /// </summary>
        private IEnumerator InitializeDependencies()
        {
            while (_carDamageHandler == null || _levelManager == null)
            {
                _carDamageHandler = ServiceLocator.Get<CarDamageHandler>();
                _levelManager = ServiceLocator.Get<LevelManager>();

                if (_carDamageHandler != null)
                {
                    _carDamageHandler.OnHealthChanged += UpdateHealthBar;
                    InitializeHealthSegments(_carDamageHandler.MaxHealth);
                }

                if (_levelManager != null)
                {
                    _levelManager.OnLevelStart += OpenGameUIElements;
                    _levelManager.OnLevelFail += ShowFailResultScreen;
                    _levelManager.OnLevelComplete += ShowSuccessResultScreen;
                }

                yield return null;
            }
        }

        /// <summary>
        /// Sets up button event handlers.
        /// </summary>
        private void SetButtonEvents()
        {
            _menuButton.onButtonDown.AddListener(HideResultScreen);
            _menuButton.onButtonDown.AddListener(MenuScene);
            _restartButton.onButtonDown.AddListener(RestartLevel);
            _nextButton.onButtonDown.AddListener(NextLevel);
        }

        private void Update()
        {
            var carController = ServiceLocator.Get<CarController>();

            if (carController != null)
            {
                float targetSpeed = carController.CarSpeed;
                _currentSpeed = Mathf.SmoothDamp(_currentSpeed, targetSpeed, ref _speedVelocity, 0.1f);
                _speedText.text = $"{_currentSpeed:F0} km/h";
            }
        }

        /// <summary>
        /// Initializes the health segments based on the car's maximum health.
        /// </summary>
        /// <param name="maxHealth">The maximum health of the car.</param>
        private void InitializeHealthSegments(int maxHealth)
        {
            foreach (Transform child in _healthContainer)
            {
                Destroy(child.gameObject);
            }

            _healthSegments.Clear();

            for (int i = 0; i < maxHealth; i++)
            {
                var segment = Instantiate(_healthSegmentPrefab, _healthContainer);
                _healthSegments.Add(segment);
            }
        }

        /// <summary>
        /// Updates the health bar UI based on the car's current health.
        /// </summary>
        /// <param name="health">The current health of the car.</param>
        private void UpdateHealthBar(int health, int maxHealth)
        {
            for (int i = 0; i < _healthSegments.Count; i++)
            {
                _healthSegments[i].SetActive(i < health);
            }
        }

        /// <summary>
        /// Shows game UI elements (e.g., car displayer, control buttons) with an animation.
        /// </summary>
        private void OpenGameUIElements()
        {
            ToggleUIElements(new[] { _carDisplayer, _carHealthDisplayer, _controlButtons, _speedDisplayer }, true);
        }

        /// <summary>
        /// Hides the result screen and shows game UI elements again.
        /// </summary>
        private void HideResultScreen()
        {
            _resultScreen.SetActive(false);
            ToggleUIElements(new[] { _carDisplayer, _carHealthDisplayer, _controlButtons, _speedDisplayer }, true);
        }

        /// <summary>
        /// Displays the fail result screen with a message and hides game UI elements.
        /// </summary>
        private void ShowFailResultScreen()
        {
            DisplayResultScreen(_levelFailMessage);
        }

        /// <summary>
        /// Displays the success result screen with a message and hides game UI elements.
        /// </summary>
        private void ShowSuccessResultScreen()
        {
            DisplayResultScreen(_levelCompleteMessage);
        }

        /// <summary>
        /// Displays the result screen with the given message and hides game UI elements.
        /// </summary>
        /// <param name="message">The message to display on the result screen.</param>
        private void DisplayResultScreen(string message)
        {
            _resultText.text = message;
            _resultScreen.SetActive(true);
            ToggleUIElements(new[] { _carDisplayer, _carHealthDisplayer, _controlButtons, _speedDisplayer }, false);
        }

        /// <summary>
        /// Toggles UI elements' active state with scale animations.
        /// </summary>
        /// <param name="elements">The UI elements to toggle.</param>
        /// <param name="isEnabled">Whether to enable or disable the UI elements.</param>
        private void ToggleUIElements(GameObject[] elements, bool isEnabled)
        {
            foreach (var element in elements)
            {
                if (isEnabled)
                {
                    element.SetActive(true);
                    element.transform.DOScale(1, _elementAnimationDuration).SetEase(_openElementAnimationEase);
                }
                else
                {
                    element.transform.DOScale(0, _elementAnimationDuration).SetEase(_closeElementAnimationEase)
                        .OnComplete(() => element.SetActive(false));
                }
            }
        }

        /// <summary>
        /// Initializes the UI by hiding all game elements and result screen.
        /// </summary>
        private void InitializeUI()
        {
            _resultScreen.SetActive(false);
            ToggleUIElements(new[] { _carDisplayer, _carHealthDisplayer, _controlButtons, _speedDisplayer }, false);
        }

        /// <summary>
        /// Restarts the current level (button action).
        /// </summary>
        private void RestartLevel()
        {
            _levelManager.HandleRestartButtonPressed();
        }

        /// <summary>
        /// Loads the next level (button action).
        /// </summary>
        private void NextLevel()
        {
            _levelManager.HandleNextButtonPressed();
        }

        /// <summary>
        /// Loads the menu scene (button action).
        /// </summary>
        private void MenuScene()
        {
            _levelManager.HandleMenuButtonPressed();
        }
    }
}