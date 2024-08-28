using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using _Game.UI;

namespace _Game.Management
{
    /// <summary>
    /// Manages the main menu of the game, handling scene transitions, animations, and exit game functionality.
    /// Also handles scaling and visibility of the game name text.
    /// </summary>
    public class MenuManager : MonoBehaviour
    {
        [Header("UI Elements")]
        [Tooltip("The menu buttons container that will be scaled down before scene transition.")]
        [SerializeField] private GameObject _menuButtons;

        [Tooltip("The current car point object that will be scaled down before scene transition.")]
        [SerializeField] private GameObject _currentCarPoint;

        [Tooltip("The game name text that will be scaled down before scene transition.")]
        [SerializeField] private GameObject _gameNameText;

        [Header("Animation Settings")]
        [Tooltip("Duration for scaling down the UI elements.")]
        [SerializeField, Range(0.1f, 2.0f)] private float _scaleDownDuration = 0.5f;

        [Tooltip("Ease type for the scale down animation.")]
        [SerializeField] private Ease _scaleDownEase = Ease.InBack;

        [Tooltip("Ease type for the scale up animation.")]
        [SerializeField] private Ease _scaleUpEase = Ease.OutBack;

        [Header("Exit Game Confirmation")]
        [Tooltip("The exit confirmation dialog UI.")]
        [SerializeField] private GameObject _quitMenu;

        [Tooltip("Duration for showing and hiding the exit confirmation dialog.")]
        [SerializeField, Range(0.1f, 2.0f)] private float _quitMenuAnimationDuration = 0.3f;

        private bool _isQuitMenuOpen = false;

        [Header("Buttons")]
        [Tooltip("The start button that triggers the StartGame function.")]
        [SerializeField] private CustomButton _startButton;

        [Tooltip("Button to open the quit menu.")]
        [SerializeField] private CustomButton _openQuitMenuButton;

        [Tooltip("Button to close the quit menu.")]
        [SerializeField] private CustomButton _closeQuitMenuButton;

        [Tooltip("Button to quit the game.")]
        [SerializeField] private CustomButton _quitButton;

        private void Awake()
        {
            Initialize();
            SetupButtonListeners();
        }

        /// <summary>
        /// Initializes the state of UI elements.
        /// </summary>
        private void Initialize()
        {
            _quitMenu.transform.localScale = Vector3.zero;
            _quitMenu.SetActive(false);

            _gameNameText.transform.localScale = Vector3.zero;
            _menuButtons.transform.localScale = Vector3.zero;
            _currentCarPoint.transform.localScale = Vector3.zero;

            Sequence sequence = DOTween.Sequence();

            // Scale down the menu buttons, car point, and game name at the same time
            sequence.Append(_menuButtons.transform.DOScale(Vector3.one, _scaleDownDuration).SetEase(_scaleDownEase));
            sequence.Join(_currentCarPoint.transform.DOScale(Vector3.one, _scaleDownDuration).SetEase(_scaleDownEase));
            sequence.Join(_gameNameText.transform.DOScale(Vector3.one, _scaleDownDuration).SetEase(_scaleDownEase));
        }

        /// <summary>
        /// Sets up listeners for the buttons.
        /// </summary>
        private void SetupButtonListeners()
        {
            _startButton.onButtonDown.AddListener(StartGame);
            _openQuitMenuButton.onButtonDown.AddListener(ToggleQuitMenu);
            _closeQuitMenuButton.onButtonDown.AddListener(ToggleQuitMenu);
            _quitButton.onButtonDown.AddListener(ExitGame);
        }

        /// <summary>
        /// Starts the game scene after scaling down the UI elements.
        /// </summary>
        public void StartGame()
        {
            Sequence sequence = DOTween.Sequence();

            // Scale down the menu buttons, car point, and game name at the same time
            sequence.Append(_menuButtons.transform.DOScale(Vector3.zero, _scaleDownDuration).SetEase(_scaleDownEase));
            sequence.Join(_currentCarPoint.transform.DOScale(Vector3.zero, _scaleDownDuration).SetEase(_scaleDownEase));
            sequence.Join(_gameNameText.transform.DOScale(Vector3.zero, _scaleDownDuration).SetEase(_scaleDownEase));

            // After the scaling down animation completes, load the game scene
            sequence.OnComplete(() => SceneManager.LoadScene("GameScene"));
        }

        /// <summary>
        /// Toggles the exit confirmation dialog with animations, while also hiding or showing the menu buttons,
        /// car point, and game name text.
        /// </summary>
        public void ToggleQuitMenu()
        {
            if (_isQuitMenuOpen)
            {
                // Close quit menu and show UI elements
                Sequence closeSequence = DOTween.Sequence();
                closeSequence.Append(_quitMenu.transform.DOScale(Vector3.zero, _quitMenuAnimationDuration).SetEase(_scaleDownEase))
                    .OnComplete(() => _quitMenu.SetActive(false));

                // Show the menu buttons, car point, and game name text with scaling
                closeSequence.Append(_menuButtons.transform.DOScale(Vector3.one, _scaleDownDuration).SetEase(_scaleUpEase));
                closeSequence.Join(_currentCarPoint.transform.DOScale(Vector3.one, _scaleDownDuration).SetEase(_scaleUpEase));
                closeSequence.Join(_gameNameText.transform.DOScale(Vector3.one, _scaleDownDuration).SetEase(_scaleUpEase));

                _isQuitMenuOpen = false;
            }
            else
            {
                // Hide UI elements and open quit menu
                Sequence openSequence = DOTween.Sequence();
                openSequence.Append(_menuButtons.transform.DOScale(Vector3.zero, _scaleDownDuration).SetEase(_scaleDownEase));
                openSequence.Join(_currentCarPoint.transform.DOScale(Vector3.zero, _scaleDownDuration).SetEase(_scaleDownEase));
                openSequence.Join(_gameNameText.transform.DOScale(Vector3.zero, _scaleDownDuration).SetEase(_scaleDownEase));

                openSequence.AppendCallback(() => _quitMenu.SetActive(true));
                openSequence.Append(_quitMenu.transform.DOScale(Vector3.one, _quitMenuAnimationDuration).SetEase(_scaleUpEase));

                _isQuitMenuOpen = true;
            }
        }

        /// <summary>
        /// Exits the game application.
        /// </summary>
        public void ExitGame()
        {
            Debug.Log("Exiting Game...");
            Application.Quit();
        }
    }
}