using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using _Game.UI;
using _Game.Car;
using System.Collections;
using _Game.Data;
using TMPro;

namespace _Game.Management
{
    /// <summary>
    /// Manages the main menu of the game, handling scene transitions, animations, gallery opening/closing, and exit game functionality.
    /// </summary>
    public class MenuManager : MonoBehaviour
    {
        [Header("Game Data")]
        [Tooltip("Reference to the game data ScriptableObject.")]
        [SerializeField] private GameData _gameData;

        [Header("UI Elements")]
        [Tooltip("The menu buttons container that will be scaled down before scene transition.")]
        [SerializeField] private GameObject _menuButtons;

        [Tooltip("The current car point object that will be scaled down before scene transition.")]
        [SerializeField] private GameObject _currentCarPoint;

        [Tooltip("The game name text that will be scaled down before scene transition.")]
        [SerializeField] private GameObject _gameNameText;

        [Tooltip("Gallery UI that will be opened/closed when the gallery buttons are pressed.")]
        [SerializeField] private GameObject _galleryUI;

        //[Tooltip("The gallery manager that will be activated/deactivated when the gallery is opened/closed.")]
        //[SerializeField] private GalleryManager _galleryManager;

        [Header("Animation Settings")]
        [Tooltip("Duration for scaling down the UI elements.")]
        [SerializeField, Range(0.1f, 2.0f)] private float _scaleChangeDuration = 0.5f;

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

        [Tooltip("Button to open the gallery UI.")]
        [SerializeField] private CustomButton _galleryButton;

        [Tooltip("Button to close the gallery UI.")]
        [SerializeField] private CustomButton _closeGalleryButton;

        [Header("Displayers")]
        [Tooltip("The displayer displaying the coin amount.")]
        [SerializeField] private TextMeshProUGUI _coinAmountText;

        [Header("Insufficient Funds Warning")]
        [Tooltip("Popup panel that shows a warning when funds are insufficient.")]
        [SerializeField] private RectTransform _warningPanel;
        [Tooltip("CanvasGroup component for fading the warning panel.")]
        [SerializeField] private CanvasGroup _warningCanvasGroup;
        [Header("Insufficient Funds Animation Settings")]
        [Tooltip("Duration of the shake and color change animation when funds are insufficient.")]
        [SerializeField, Range(0.1f, 2f)] private float _insufficientFundsDuration = 0.5f;
        [Tooltip("Scale factor for the money text during the insufficient funds animation.")]
        [SerializeField, Range(1f, 2f)] private float _textShakeScale = 1.2f;
        [Tooltip("Duration of the warning popup scale animation.")]
        [SerializeField, Range(0.1f, 2f)] private float _warningScaleDuration = 0.3f;
        [Tooltip("Duration of the warning popup fade animation.")]
        [SerializeField, Range(0.1f, 2f)] private float _warningFadeDuration = 0.5f;

        // Private Variables
        private EconomyManager _economyManager;

        private void Awake()
        {
            StartCoroutine(InitializeDependencies());

            Initialize();
            SetupButtonListeners();

            UpdateCoinDisplayer();
        }

        private void Start()
        {
            SetEvents();
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
        /// Initializes the state of UI elements.
        /// </summary>
        private void Initialize()
        {
            _quitMenu.transform.localScale = Vector3.zero;
            _quitMenu.SetActive(false);

            _galleryUI.SetActive(false); // Ensure Gallery UI starts inactive
            //_galleryManager.gameObject.SetActive(false); // Ensure GalleryManager starts inactive

            _gameNameText.transform.localScale = Vector3.zero;
            _menuButtons.transform.localScale = Vector3.zero;
            _currentCarPoint.transform.localScale = Vector3.zero;

            _warningPanel.transform.localScale = Vector3.zero;
            _warningCanvasGroup.alpha = 0;

            Sequence sequence = DOTween.Sequence();

            // Scale down the menu buttons, car point, and game name at the same time
            sequence.Append(_menuButtons.transform.DOScale(Vector3.one, _scaleChangeDuration).SetEase(_scaleUpEase));
            sequence.Join(_currentCarPoint.transform.DOScale(Vector3.one, _scaleChangeDuration).SetEase(_scaleUpEase));
            sequence.Join(_gameNameText.transform.DOScale(Vector3.one, _scaleChangeDuration).SetEase(_scaleUpEase));
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
            _galleryButton.onButtonDown.AddListener(OpenGallery);
            _closeGalleryButton.onButtonDown.AddListener(CloseGallery); // Listener for closing gallery
        }

        /// <summary>
        /// Sets up button event handlers.
        /// </summary>
        private void SetEvents()
        {
            if (_economyManager == null)
            {
                _economyManager = ServiceLocator.Get<EconomyManager>();
            }

            _economyManager.OnCoinAmountChanged += UpdateCoinDisplayer;
            _economyManager.OnCoinAmountInsufficient += ShowInsufficientFundsWarning;
        }

        private void UpdateCoinDisplayer()
        {
            _coinAmountText.text = _gameData.Coins.ToString();
        }

        /// <summary>
        /// Displays a warning animation when the player has insufficient funds.
        /// Ensures the animation restarts properly if called consecutively.
        /// </summary>
        public void ShowInsufficientFundsWarning()
        {
            // Kill any ongoing animations on the money text and warning panel
            _coinAmountText.rectTransform.DOKill();
            _coinAmountText.DOKill();
            _warningPanel.DOKill();
            _warningCanvasGroup.DOKill();

            // Animate money text to shake and turn red
            _coinAmountText.rectTransform.DOShakeScale(_insufficientFundsDuration, _textShakeScale - 1f)
                .OnStart(() => _coinAmountText.color = Color.red)
                .OnComplete(() => _coinAmountText.color = Color.yellow);

            // Animate the warning panel with a scale and fade effect
            _warningPanel.DOScale(Vector3.one, _warningScaleDuration).SetEase(Ease.OutBack);
            _warningCanvasGroup.DOFade(1f, _warningFadeDuration).OnComplete(() =>
            {
                // Fade out after a delay
                DOVirtual.DelayedCall(1f, () =>
                {
                    _warningCanvasGroup.DOFade(0f, _warningFadeDuration);
                    _warningPanel.DOScale(Vector3.zero, _warningScaleDuration).SetEase(Ease.InBack);
                });
            });
        }

        /// <summary>
        /// Starts the game scene after scaling down the UI elements.
        /// </summary>
        public void StartGame()
        {
            Sequence sequence = DOTween.Sequence();

            // Scale down the menu buttons, car point, and game name at the same time
            sequence.Append(_menuButtons.transform.DOScale(Vector3.zero, _scaleChangeDuration).SetEase(_scaleDownEase));
            sequence.Join(_currentCarPoint.transform.DOScale(Vector3.zero, _scaleChangeDuration).SetEase(_scaleDownEase));
            sequence.Join(_gameNameText.transform.DOScale(Vector3.zero, _scaleChangeDuration).SetEase(_scaleDownEase));

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
                closeSequence.Append(_menuButtons.transform.DOScale(Vector3.one, _scaleChangeDuration).SetEase(_scaleUpEase));
                closeSequence.Join(_currentCarPoint.transform.DOScale(Vector3.one, _scaleChangeDuration).SetEase(_scaleUpEase));
                closeSequence.Join(_gameNameText.transform.DOScale(Vector3.one, _scaleChangeDuration).SetEase(_scaleUpEase));

                _isQuitMenuOpen = false;
            }
            else
            {
                // Hide UI elements and open quit menu
                Sequence openSequence = DOTween.Sequence();
                openSequence.Append(_menuButtons.transform.DOScale(Vector3.zero, _scaleChangeDuration).SetEase(_scaleDownEase));
                openSequence.Join(_currentCarPoint.transform.DOScale(Vector3.zero, _scaleChangeDuration).SetEase(_scaleDownEase));
                openSequence.Join(_gameNameText.transform.DOScale(Vector3.zero, _scaleChangeDuration).SetEase(_scaleDownEase));

                openSequence.AppendCallback(() => _quitMenu.SetActive(true));
                openSequence.Append(_quitMenu.transform.DOScale(Vector3.one, _quitMenuAnimationDuration).SetEase(_scaleUpEase));

                _isQuitMenuOpen = true;
            }
        }

        /// <summary>
        /// Opens the gallery UI after scaling down the main menu UI.
        /// </summary>
        public void OpenGallery()
        {
            Sequence sequence = DOTween.Sequence();

            // Scale down the menu buttons, car point, and game name
            sequence.Append(_menuButtons.transform.DOScale(Vector3.zero, _scaleChangeDuration).SetEase(_scaleDownEase));
            sequence.Join(_currentCarPoint.transform.DOScale(Vector3.zero, _scaleChangeDuration).SetEase(_scaleDownEase));
            sequence.Join(_gameNameText.transform.DOScale(Vector3.zero, _scaleChangeDuration).SetEase(_scaleDownEase));

            // After the scaling down animation completes, show the gallery UI
            sequence.OnComplete(() =>
            {
                _galleryUI.SetActive(true);
                //_galleryManager.gameObject.SetActive(true);
                sequence.Append(_galleryUI.transform.DOScale(Vector3.one, _scaleChangeDuration).SetEase(_scaleUpEase));
            });
        }

        /// <summary>
        /// Closes the gallery UI and reopens the main menu UI with animations.
        /// </summary>
        public void CloseGallery()
        {
            Sequence sequence = DOTween.Sequence();

            // Scale down the gallery UI
            sequence.Append(_galleryUI.transform.DOScale(Vector3.zero, _scaleChangeDuration).SetEase(_scaleDownEase));

            // After the gallery is closed, reactivate the main menu UI
            sequence.OnComplete(() =>
            {
                // Rescale the main menu UI
                _menuButtons.transform.DOScale(Vector3.one, _scaleChangeDuration).SetEase(_scaleUpEase);
                _currentCarPoint.transform.DOScale(Vector3.one, _scaleChangeDuration).SetEase(_scaleUpEase);
                _gameNameText.transform.DOScale(Vector3.one, _scaleChangeDuration).SetEase(_scaleUpEase);

                _galleryUI.SetActive(false);
                //_galleryManager.gameObject.SetActive(false);
            });
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