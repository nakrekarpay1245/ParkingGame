using _Game.UI;
using UnityEngine;

namespace _Game.Inputs
{
    /// <summary>
    /// Handles player input and updates the PlayerInput ScriptableObject accordingly.
    /// </summary>
    public class InputHandler : MonoBehaviour
    {
        [Header("Player Input Scriptable Object")]
        [Tooltip("Reference to the PlayerInput ScriptableObject that stores player input states.")]
        [SerializeField] private PlayerInputSO playerInput;

        [Header("Touch Controls")]
        [Tooltip("Check if touch controls are set up")]
        [SerializeField] private bool _touchControlsSetup = false;
        [SerializeField] private CustomButton _throttleButton;
        [SerializeField] private CustomButton _reverseButton;
        [SerializeField] private CustomButton _turnLeftButton;
        [SerializeField] private CustomButton _turnRightButton;
        [SerializeField] private CustomButton _handbrakeButton;

        private bool _gamePaused;

        private void Start()
        {
            if (_throttleButton != null && _reverseButton != null && _turnLeftButton != null &&
                _turnRightButton != null && _handbrakeButton != null)
            {
                _touchControlsSetup = true;
            }
            else
            {
                _touchControlsSetup = false;
                Debug.LogWarning("Some of the buttons are null");
            }
        }

        private void Update()
        {
            HandleInput();
        }

        /// <summary>
        /// Processes player input and updates the PlayerInput ScriptableObject.
        /// </summary>
        private void HandleInput()
        {
            bool isAccelerating = Input.GetKey(KeyCode.W) || (_touchControlsSetup && _throttleButton.ButtonPressed);
            bool isReversing = Input.GetKey(KeyCode.S) || (_touchControlsSetup && _reverseButton.ButtonPressed);
            bool isTurningLeft = Input.GetKey(KeyCode.A) || (_touchControlsSetup && _turnLeftButton.ButtonPressed);
            bool isTurningRight = Input.GetKey(KeyCode.D) || (_touchControlsSetup && _turnRightButton.ButtonPressed);
            bool isHandbraking = Input.GetKey(KeyCode.Space) || (_touchControlsSetup && _handbrakeButton.ButtonPressed);

            playerInput.IsAccelerating = isAccelerating /*&& !_gamePaused*/;
            playerInput.IsReversing = isReversing /*&& !_gamePaused*/;
            playerInput.IsTurningLeft = isTurningLeft /*&& !_gamePaused*/;
            playerInput.IsTurningRight = isTurningRight /*&& !_gamePaused*/;
            playerInput.IsHandbraking = isHandbraking /*&& !_gamePaused*/;
        }

        //private void Pause()
        //{
        //    _gamePaused = true;
        //}

        //private void Play()
        //{
        //    _gamePaused = false;
        //}

        //private void OnEnable()
        //{
        //    GameStateManager.Singleton.OnGameStateChanged += OnGameStateChanged;
        //}

        //private void OnDisable()
        //{
        //    GameStateManager.Singleton.OnGameStateChanged -= OnGameStateChanged;
        //}

        //public void OnGameStateChanged(GameState newGameState)
        //{
        //    if (newGameState == GameState.Gameplay)
        //    {
        //        Play();
        //    }
        //    else
        //    {
        //        Pause();
        //    }
        //}
    }
}