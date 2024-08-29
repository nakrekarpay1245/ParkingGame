using UnityEngine;
using DG.Tweening;
using _Game.Data;
using _Game.LevelSystem;
using _Game.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

namespace _Game.Management
{
    /// <summary>
    /// Manages the game levels including loading, restarting, and completing levels.
    /// Handles scene transitions and level state events (start, fail, complete).
    /// </summary>
    public class LevelManager : MonoBehaviour
    {
        [Header("Game Data")]
        [Tooltip("Reference to the game data ScriptableObject.")]
        [SerializeField] private GameData _gameData;

        [Header("Scene Transition Settings")]
        [Tooltip("Duration for scene transitions in seconds.")]
        [SerializeField, Range(0.1f, 5f)] private float _sceneChangeDuration = 1f;

        // Events for level state changes
        public UnityAction OnLevelStart;
        public UnityAction OnLevelFail;
        public UnityAction OnLevelComplete;

        private Level _currentLevel;
        private bool _levelCompleted;

        private void Awake()
        {
            RegisterServices();
        }

        private void Start()
        {
            InitializeLevel();
        }

        /// <summary>
        /// Registers the LevelManager as a service using a Service Locator.
        /// </summary>
        private void RegisterServices()
        {
            ServiceLocator.Register(this);
        }

        /// <summary>
        /// Initializes and loads the current level based on the GameData ScriptableObject.
        /// </summary>
        private void InitializeLevel()
        {
            LoadCurrentLevel();
            StartLevel();
        }

        /// <summary>
        /// Loads the current level from the GameData ScriptableObject and logs it.
        /// </summary>
        private void LoadCurrentLevel()
        {
            _currentLevel = _gameData.CurrentLevel;
            Debug.Log($"Loaded Level: {_currentLevel.name}");
        }

        /// <summary>
        /// Starts the loaded level by instantiating it and triggering the OnLevelStart event.
        /// </summary>
        private void StartLevel()
        {
            Instantiate(_currentLevel, Vector3.zero, Quaternion.identity);
            _levelCompleted = false;
            Debug.Log($"Level {_currentLevel.name} started.");
            OnLevelStart?.Invoke();  // Notify subscribers that the level has started
        }

        /// <summary>
        /// Marks the level as completed and triggers the OnLevelComplete event.
        /// Prevents multiple triggers if the level is already completed.
        /// </summary>
        public void CompleteLevel()
        {
            if (_levelCompleted) return;

            _levelCompleted = true;
            _gameData.CurrentLevelIndex++;
            Debug.Log("Level Completed!");
            OnLevelComplete?.Invoke();
        }

        /// <summary>
        /// Marks the level as failed and triggers the OnLevelFail event.
        /// Prevents multiple triggers if the level is already failed/completed.
        /// </summary>
        public void FailLevel()
        {
            if (_levelCompleted) return;

            _levelCompleted = true;
            Debug.Log("Level Failed!");
            OnLevelFail?.Invoke();  // Notify subscribers that the level has failed
        }

        /// <summary>
        /// Handles the next button press by updating the current level index and loading the next level.
        /// </summary>
        public void HandleNextButtonPressed()
        {
            Debug.Log("Next button pressed.");
            TransitionToScene("GameScene");
        }

        /// <summary>
        /// Handles the restart button press by reloading the current level.
        /// </summary>
        public void HandleRestartButtonPressed()
        {
            Debug.Log("Restart button pressed.");
            TransitionToScene("GameScene");
        }

        /// <summary>
        /// Handles the menu button press by transitioning to the menu scene after a delay.
        /// </summary>
        public void HandleMenuButtonPressed()
        {
            Debug.Log("Menu button pressed.");
            TransitionToScene("MenuScene");
        }

        /// <summary>
        /// Transitions to the specified scene using a delayed call for smooth transitions.
        /// </summary>
        /// <param name="sceneName">The name of the scene to transition to.</param>
        private void TransitionToScene(string sceneName)
        {
            DOVirtual.DelayedCall(_sceneChangeDuration, () => SceneManager.LoadScene(sceneName));
        }
    }
}