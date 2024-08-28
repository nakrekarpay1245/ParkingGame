using UnityEngine;
using UnityEngine.UI;
using _Game.Data;
using _Game.LevelSystem;
using DG.Tweening;

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
        private Text _resultText;

        [Tooltip("Resume button, for pausing and resuming the game.")]
        [SerializeField]
        private Button _resumeButton;

        [Tooltip("Next level button, appears when the level is completed.")]
        [SerializeField]
        private Button _nextButton;

        [Tooltip("Restart button, appears when the level is failed.")]
        [SerializeField]
        private Button _restartButton;

        private Level _currentLevel;
        private bool _levelCompleted;

        /// <summary>
        /// Initializes the level manager by setting the current level from the game data and starting the level.
        /// </summary>
        private void Awake()
        {
            LoadCurrentLevel();
            //InitializeUI();
            StartLevel();
        }

        /// <summary>
        /// Loads the current level from the GameData ScriptableObject.
        /// </summary>
        private void LoadCurrentLevel()
        {
            _currentLevel = _gameData.CurrentLevel;
            Debug.Log($"Loaded Level: {_currentLevel.name}");
        }

        ///// <summary>
        ///// Initializes the UI elements and sets their initial states.
        ///// </summary>
        //private void InitializeUI()
        //{
        //    _resultScreen.SetActive(false);
        //    _resumeButton.gameObject.SetActive(true);
        //    _nextButton.gameObject.SetActive(false);
        //    _restartButton.gameObject.SetActive(false);
        //}

        /// <summary>
        /// Starts the current level by instantiating necessary objects and resetting the player.
        /// </summary>
        private void StartLevel()
        {
            // Reset player position to (0,0,0) and rotation to identity
            Vector3 startPosition = Vector3.zero;
            Quaternion startRotation = Quaternion.identity;

            // Instantiate or reset the level
            Instantiate(_currentLevel, startPosition, startRotation);

            _levelCompleted = false;
            Debug.Log($"Level {_currentLevel.name} started.");
        }

        ///// <summary>
        ///// Call this method when the level is successfully completed.
        ///// </summary>
        //public void LevelComplete()
        //{
        //    if (_levelCompleted) return;

        //    _levelCompleted = true;
        //    ShowResultScreen("Level Complete!", true);

        //    // Optionally, animate the result screen using DOTween
        //    _resultScreen.transform.DOScale(Vector3.one, 0.5f).From(Vector3.zero).SetEase(Ease.OutBack);
        //}

        ///// <summary>
        ///// Call this method when the level fails.
        ///// </summary>
        //public void LevelFail()
        //{
        //    if (_levelCompleted) return;

        //    _levelCompleted = true;
        //    ShowResultScreen("Level Failed", false);

        //    // Optionally, animate the result screen using DOTween
        //    _resultScreen.transform.DOScale(Vector3.one, 0.5f).From(Vector3.zero).SetEase(Ease.OutBack);
        //}

        ///// <summary>
        ///// Shows the result screen with the appropriate message and buttons based on the outcome.
        ///// </summary>
        ///// <param name="message">The message to display.</param>
        ///// <param name="isSuccess">Whether the level was successfully completed or not.</param>
        //private void ShowResultScreen(string message, bool isSuccess)
        //{
        //    _resultScreen.SetActive(true);
        //    _resultText.text = message;

        //    // Show/hide the appropriate buttons based on the result
        //    _nextButton.gameObject.SetActive(isSuccess);
        //    _restartButton.gameObject.SetActive(!isSuccess);

        //    // Optionally, hide the resume button when the level ends
        //    _resumeButton.gameObject.SetActive(false);
        //}

        ///// <summary>
        ///// Method called when the Next button is pressed. Advances to the next level.
        ///// </summary>
        //public void OnNextButtonPressed()
        //{
        //    _gameData.CurrentLevelIndex++;
        //    LoadCurrentLevel();
        //    InitializeUI();
        //    StartLevel();
        //}

        ///// <summary>
        ///// Method called when the Restart button is pressed. Restarts the current level.
        ///// </summary>
        //public void OnRestartButtonPressed()
        //{
        //    InitializeUI();
        //    StartLevel();
        //}

        ///// <summary>
        ///// Method called when the Resume button is pressed. Resumes the game if paused.
        ///// </summary>
        //public void OnResumeButtonPressed()
        //{
        //    // Implement your resume logic here, such as unpausing the game
        //    Debug.Log("Game resumed.");
        //}
    }
}