using UnityEngine;
using TMPro;
using DG.Tweening;
using _Game.Management;
using _Game.Car;
using System.Collections;
using System.Collections.Generic;
using _Game._helpers.Audios;
using _Game.Data;

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
        [SerializeField, Range(0.1f, 2f)] private float _elementAnimationDuration = 0.5f;
        [SerializeField, Range(0.1f, 2f)] private float _elementAnimationInterval = 0.1f;
        [Tooltip("Ease type for opening UI elements.")]
        [SerializeField] private Ease _openElementAnimationEase = Ease.OutBack;
        [Tooltip("Ease type for closing UI elements.")]
        [SerializeField] private Ease _closeElementAnimationEase = Ease.InBack;

        [Header("Result Screen Delay")]
        [Tooltip("Delay before showing the result screen.")]
        [SerializeField, Range(0.1f, 5f)] private float _resultScreenDelay = 1f;

        [Header("Speed Display")]
        [Tooltip("Text component to display the vehicle's speed.")]
        [SerializeField] private TextMeshProUGUI _speedText;

        [Header("Coin Display")]
        [Tooltip("Text component to display the coin's earned on level.")]
        [SerializeField] private TextMeshProUGUI _coinCountText;
        [SerializeField] private GameObject[] _stars;

        [Header("Health Display")]
        [Tooltip("Prefab for health segments.")]
        [SerializeField] private GameObject _healthSegmentPrefab;
        [Tooltip("Container for health segments.")]
        [SerializeField] private Transform _healthContainer;

        [Header("Effects")]
        [Header("Audio Effects")]
        [Tooltip("The sound play when spawn an obstacle.")]
        [SerializeField] private string _uiSoundKey = "ui";

        private List<GameObject> _healthSegments = new List<GameObject>();
        private float _currentSpeed;
        private float _speedVelocity = 0.0f;

        private CarDamageHandler _carDamageHandler;
        private LevelManager _levelManager;
        private AudioManager _audioManager;

        private void Awake()
        {
            StartCoroutine(InitializeDependencies());
        }

        private void Start()
        {
            _audioManager = ServiceLocator.Get<AudioManager>();

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
            UpdateCarSpeedText();
        }

        private void UpdateCarSpeedText()
        {
            var carController = ServiceLocator.Get<CarController>();

            if (carController != null)
            {
                float targetSpeed = carController.CarSpeed;
                _currentSpeed = Mathf.SmoothDamp(_currentSpeed, targetSpeed, ref _speedVelocity, 0.1f);
                _speedText.text = $"{_currentSpeed:F0}";
            }
            else
            {
                Debug.Log("Car Controller is null");
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
        /// Opens game UI elements with animations.
        /// </summary>
        private void OpenGameUIElements()
        {
            ToggleUIElements(new[] { _carDisplayer, _carHealthDisplayer, _controlButtons, _speedDisplayer }, true, true);
        }

        /// <summary>
        /// Closes game UI elements with animations.
        /// </summary>
        private void CloseGameUIElements()
        {
            ToggleUIElements(new[] { _carDisplayer, _carHealthDisplayer, _controlButtons, _speedDisplayer }, false, true);
        }

        /// <summary>
        /// Hides the result screen and shows game UI elements again.
        /// </summary>
        private void HideResultScreen()
        {
            ToggleUIElements(new[] { _resultText.gameObject, _menuButton.gameObject, _restartButton.gameObject, _nextButton.gameObject, _resultScreen }, false, true);
        }

        /// <summary>
        /// Displays the fail result screen with a message and hides game UI elements.
        /// </summary>
        private void ShowFailResultScreen()
        {
            DOVirtual.DelayedCall(_resultScreenDelay, () =>
            {
                int earnedCoins = _levelManager.CurrentReward;
                int starInLevel = _levelManager.CurrentStars;

                // Display the earned coins
                _coinCountText.text = $"{earnedCoins}";

                // Display the earned stars
                for (int i = 0; i < _stars.Length; i++)
                {
                    _stars[i].SetActive(i < starInLevel);
                }

                GameObject[] elemets = new[] { _resultScreen, _resultText.gameObject, _menuButton.gameObject,
                    _restartButton.gameObject };

                // Show result UI
                DisplayResultScreen(elemets, false);
            });
        }

        /// <summary>
        /// Displays the success result screen with a message and hides game UI elements.
        /// </summary>
        private void ShowSuccessResultScreen()
        {
            DOVirtual.DelayedCall(_resultScreenDelay, () =>
            {
                int earnedCoins = _levelManager.CurrentReward;
                int starInLevel = _levelManager.CurrentStars;

                // Display the earned coins
                _coinCountText.text = $"{earnedCoins}";

                // Display the earned stars
                for (int i = 0; i < _stars.Length; i++)
                {
                    _stars[i].SetActive(i < starInLevel);
                }

                GameObject[] elemets = new[] { _resultScreen, _resultText.gameObject, _menuButton.gameObject, _restartButton.gameObject, _nextButton.gameObject };

                // Show result UI
                DisplayResultScreen(elemets, true);
            });
        }

        /// <summary>
        /// Displays the result screen with the given message and hides game UI elements.
        /// </summary>
        /// <param name="success">Indicates whether the result is a success or failure.</param>
        /// <param name="elements">The UI elements to toggle.</param>
        private void DisplayResultScreen(GameObject[] elements, bool success)
        {
            string message = success ? _levelCompleteMessage : _levelFailMessage;
            _resultText.text = message;

            CloseGameUIElements();

            ToggleUIElements(elements, success, true);
        }

        /// <summary>
        /// Toggles UI elements' active state with scale animations, either sequentially or simultaneously.
        /// </summary>
        /// <param name="elements">The UI elements to toggle.</param>
        /// <param name="isEnabled">Whether to enable or disable the UI elements.</param>
        /// <param name="sequential">Whether to animate elements sequentially (true) or simultaneously (false).</param>
        private void ToggleUIElements(GameObject[] elements, bool isEnabled, bool sequential = false)
        {
            if (sequential)
            {
                for (int i = 0; i < elements.Length; i++)
                {
                    var element = elements[i];
                    float delay = i * _elementAnimationInterval; // Calculate delay based on index and interval

                    if (isEnabled)
                    {
                        element.SetActive(true);
                        element.transform.DOScale(1, _elementAnimationDuration)
                            .SetEase(_openElementAnimationEase)
                            .SetDelay(delay); // Apply delay for staggering

                        DOTween.Sequence()
                            .AppendInterval(delay)
                            .AppendCallback(() => _audioManager.PlaySound(_uiSoundKey)); // Play sound at staggered time
                    }
                    else
                    {
                        element.transform.DOScale(0, _elementAnimationDuration)
                            .SetEase(_closeElementAnimationEase)
                            .SetDelay(delay) // Apply delay for staggering
                            .OnComplete(() => element.SetActive(false));

                        DOTween.Sequence()
                            .AppendInterval(delay)
                            .AppendCallback(() => _audioManager.PlaySound(_uiSoundKey)); // Play sound at staggered time
                    }
                }
            }
            else
            {
                foreach (var element in elements)
                {
                    if (isEnabled)
                    {
                        element.SetActive(true);
                        element.transform.DOScale(1, _elementAnimationDuration)
                            .SetEase(_openElementAnimationEase);
                    }
                    else
                    {
                        element.transform.DOScale(0, _elementAnimationDuration)
                            .SetEase(_closeElementAnimationEase)
                            .OnComplete(() => element.SetActive(false));
                    }

                    _audioManager.PlaySound(_uiSoundKey);
                }
            }
        }

        /// <summary>
        /// Initializes the UI by hiding all game elements and result screen.
        /// </summary>
        private void InitializeUI()
        {
            _resultScreen.SetActive(false);
            _resultText.gameObject.SetActive(false);
            _menuButton.gameObject.SetActive(false);
            _restartButton.gameObject.SetActive(false);
            _nextButton.gameObject.SetActive(false);

            _resultScreen.transform.localScale = Vector3.zero;
            _resultText.transform.localScale = Vector3.zero;
            _menuButton.transform.localScale = Vector3.zero;
            _restartButton.transform.localScale = Vector3.zero;
            _nextButton.transform.localScale = Vector3.zero;

            _carDisplayer.transform.localScale = Vector3.zero;
            _carHealthDisplayer.transform.localScale = Vector3.zero;
            _controlButtons.transform.localScale = Vector3.zero;
            _speedDisplayer.transform.localScale = Vector3.zero;

            _carDisplayer.SetActive(true);
            _carHealthDisplayer.gameObject.SetActive(true);
            _controlButtons.gameObject.SetActive(true);
            _speedDisplayer.gameObject.SetActive(true);
        }

        /// <summary>
        /// Restarts the current level (button action).
        /// </summary>
        private void RestartLevel()
        {
            _levelManager.HandleRestartButtonPressed();
            HideResultScreen();
        }

        /// <summary>
        /// Loads the next level (button action).
        /// </summary>
        private void NextLevel()
        {
            _levelManager.HandleNextButtonPressed();
            HideResultScreen();
        }

        /// <summary>
        /// Loads the menu scene (button action).
        /// </summary>
        private void MenuScene()
        {
            _levelManager.HandleMenuButtonPressed();
            HideResultScreen();
        }
    }
}