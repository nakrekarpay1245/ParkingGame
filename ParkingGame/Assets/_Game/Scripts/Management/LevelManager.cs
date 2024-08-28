using UnityEngine;
using _Game.Data;
using _Game.LevelSystem;
using DG.Tweening;
using TMPro;
using _Game.UI;
using UnityEngine.SceneManagement;

namespace _Game.Management
{
    public class LevelManager : MonoBehaviour
    {
        [Header("Game Data")]
        [Tooltip("Reference to the game data ScriptableObject")]
        [SerializeField]
        private GameData _gameData;

        [Header("UI Elements")]
        [Tooltip("Result screen that shows after level completion or failure.")]
        [SerializeField]
        private GameObject _resultScreen;

        [Tooltip("Text field to show the level result message.")]
        [SerializeField]
        private TextMeshProUGUI _resultText;

        [Tooltip("Menu button, for going to the menu scene.")]
        [SerializeField]
        private CustomButton _menuButton;

        [Tooltip("Next level button, appears when the level is completed.")]
        [SerializeField]
        private CustomButton _nextButton;

        [Tooltip("Restart button, appears when the level is failed.")]
        [SerializeField]
        private CustomButton _restartButton;

        [Tooltip("Car displayer UI element.")]
        [SerializeField]
        private GameObject _carDisplayer;

        [Tooltip("Car health displayer UI element.")]
        [SerializeField]
        private GameObject _carHealthDisplayer;

        [Tooltip("Control buttons UI element.")]
        [SerializeField]
        private GameObject _controlButtons;

        [Tooltip("Speed displayer UI element.")]
        [SerializeField]
        private GameObject _speedDisplayer;

        [Header("Level Result Messages")]
        [Tooltip("Message to display when the level is completed successfully.")]
        [SerializeField]
        private string _levelCompleteMessage = "Level Complete!";

        [Tooltip("Message to display when the level fails.")]
        [SerializeField]
        private string _levelFailMessage = "Level Failed";

        [Header("DOTween Animation Settings")]
        [Tooltip("Duration for the result screen scaling animation.")]
        [SerializeField]
        private float _resultScreenAnimationDuration = 0.5f;

        [Tooltip("Duration for the text and buttons animation.")]
        [SerializeField]
        private float _elementAnimationDuration = 0.3f;

        [Tooltip("The scale that the result screen will animate to.")]
        [SerializeField]
        private Vector3 _resultScreenScale = Vector3.one;

        [Tooltip("Ease type for the result screen animation.")]
        [SerializeField]
        private Ease _resultScreenEase = Ease.OutBack;

        private Level _currentLevel;
        private bool _levelCompleted;

        /// <summary>
        /// Initializes the level manager by setting the current level from the game data and starting the level.
        /// </summary>
        private void Awake()
        {
            ServiceLocator.Register(this);
            SetButtonEvents();
            InitializeUI();
            LoadCurrentLevel();
            StartLevel();
        }

        private void SetButtonEvents()
        {
            // Button event subscriptions
            _menuButton.onButtonDown.AddListener(OnMenuButtonPressed);
            _restartButton.onButtonDown.AddListener(OnRestartButtonPressed);
            _nextButton.onButtonDown.AddListener(OnNextButtonPressed);
        }

        /// <summary>
        /// Loads the current level from the GameData ScriptableObject.
        /// </summary>
        private void LoadCurrentLevel()
        {
            _currentLevel = _gameData.CurrentLevel;
            Debug.Log($"Loaded Level: {_currentLevel.name}");
        }

        /// <summary>
        /// Initializes the UI elements and sets their initial states.
        /// </summary>
        private void InitializeUI()
        {
            // Set initial scale to zero for all UI elements that will be animated
            _resultScreen.transform.localScale = Vector3.zero;
            _resultText.transform.localScale = Vector3.zero;
            _menuButton.transform.localScale = Vector3.zero;
            _nextButton.transform.localScale = Vector3.zero;
            _restartButton.transform.localScale = Vector3.zero;

            _carDisplayer.transform.localScale = Vector3.zero;
            _carHealthDisplayer.transform.localScale = Vector3.zero;
            _controlButtons.transform.localScale = Vector3.zero;
            _speedDisplayer.transform.localScale = Vector3.zero;

            // Initially hide result screen and related buttons
            _resultScreen.SetActive(false);
            _menuButton.gameObject.SetActive(true);
            _nextButton.gameObject.SetActive(false);
            _restartButton.gameObject.SetActive(false);
        }

        /// <summary>
        /// Starts the current level by instantiating necessary objects and resetting the player.
        /// </summary>
        private void StartLevel()
        {
            Vector3 startPosition = Vector3.zero;
            Quaternion startRotation = Quaternion.identity;

            Instantiate(_currentLevel, startPosition, startRotation);

            // Animate the game UI elements
            AnimateGameUIElements(true);

            _levelCompleted = false;
            Debug.Log($"Level {_currentLevel.name} started.");
        }

        /// <summary>
        /// Animates the game-related UI elements (car displayer, health, control buttons, etc.)
        /// </summary>
        /// <param name="animateIn">True to animate in (scale up), false to animate out (scale down).</param>
        private void AnimateGameUIElements(bool animateIn)
        {
            float targetScale = animateIn ? 1f : 0f;

            _carDisplayer.transform.DOScale(targetScale, _elementAnimationDuration).SetEase(Ease.OutBack);
            _carHealthDisplayer.transform.DOScale(targetScale, _elementAnimationDuration).SetEase(Ease.OutBack);
            _controlButtons.transform.DOScale(targetScale, _elementAnimationDuration).SetEase(Ease.OutBack);
            _speedDisplayer.transform.DOScale(targetScale, _elementAnimationDuration).SetEase(Ease.OutBack);
        }

        /// <summary>
        /// Call this method when the level is successfully completed.
        /// </summary>
        public void LevelComplete()
        {
            if (_levelCompleted) return;

            _levelCompleted = true;
            AnimateGameUIElements(false);
            ShowResultScreen(_levelCompleteMessage, true);
        }

        /// <summary>
        /// Call this method when the level fails.
        /// </summary>
        public void LevelFail()
        {
            if (_levelCompleted) return;

            _levelCompleted = true;
            AnimateGameUIElements(false);
            ShowResultScreen(_levelFailMessage, false);
        }

        /// <summary>
        /// Shows the result screen with the appropriate message and buttons based on the outcome.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <param name="isSuccess">Whether the level was successfully completed or not.</param>
        private void ShowResultScreen(string message, bool isSuccess)
        {
            _resultScreen.SetActive(true);

            // Create a DOTween sequence for the animations
            Sequence sequence = DOTween.Sequence();

            // Scale down the game UI elements
            sequence.AppendCallback(() =>
            {
                AnimateGameUIElements(false);
            });

            // Scale the result screen first
            sequence.Append(_resultScreen.transform.DOScale(_resultScreenScale,
                _resultScreenAnimationDuration).From(Vector3.zero).SetEase(_resultScreenEase));

            // Show the result text after the screen scaling is complete
            sequence.AppendCallback(() =>
            {
                _resultText.text = message;
                _resultText.transform.localScale = Vector3.zero;
            });

            // Show the buttons after the text animation is complete
            sequence.AppendCallback(() =>
            {
                _nextButton.gameObject.SetActive(true);
                _restartButton.gameObject.SetActive(true);
            });

            sequence.Append(_resultText.transform.DOScale(Vector3.one, _elementAnimationDuration)
                                                  .SetEase(Ease.OutBack));

            sequence.Append(_menuButton.transform.DOScale(Vector3.one, _elementAnimationDuration)
                                                 .From(Vector3.zero).SetEase(Ease.OutBack));

            sequence.Join(_nextButton.transform.DOScale(Vector3.one, _elementAnimationDuration)
                                                 .From(Vector3.zero).SetEase(Ease.OutBack));

            sequence.Join(_restartButton.transform.DOScale(Vector3.one, _elementAnimationDuration)
                                                  .From(Vector3.zero).SetEase(Ease.OutBack));
        }

        /// <summary>
        /// Handles the logic for the "Next" button to load the next level.
        /// </summary>
        public void OnNextButtonPressed()
        {
            _gameData.CurrentLevelIndex++;

            SceneManager.LoadScene("GameScene");
        }

        /// <summary>
        /// Handles the logic for the "Restart" button to restart the current level.
        /// </summary>
        public void OnRestartButtonPressed()
        {
            SceneManager.LoadScene("GameScene");
        }

        /// <summary>
        /// Handles the logic for the "Menu" button to load the menu scene.
        /// </summary>
        public void OnMenuButtonPressed()
        {
            Debug.Log("Menu button pressed.");

            // Create a DOTween sequence for the animations
            Sequence sequence = DOTween.Sequence();

            sequence.Append(_resultText.transform.DOScale(Vector3.zero, _elementAnimationDuration)
                                                  .SetEase(Ease.InBack));

            sequence.Join(_menuButton.transform.DOScale(Vector3.zero, _elementAnimationDuration)
                                                 .From(Vector3.one)
                                                 .SetEase(Ease.InBack));

            sequence.Join(_nextButton.transform.DOScale(Vector3.zero, _elementAnimationDuration)
                                                 .From(Vector3.one)
                                                 .SetEase(Ease.InBack));

            sequence.Join(_restartButton.transform.DOScale(Vector3.zero, _elementAnimationDuration)
                                                  .From(Vector3.one)
                                                  .SetEase(Ease.InBack));

            // Scale the result screen last
            sequence.Append(_resultScreen.transform.DOScale(Vector3.zero, _resultScreenAnimationDuration)
                                                    .From(Vector3.one)
                                                    .SetEase(_resultScreenEase));

            sequence.AppendCallback(() =>
            {
                SceneManager.LoadScene("MenuScene");
            });
        }
    }
}