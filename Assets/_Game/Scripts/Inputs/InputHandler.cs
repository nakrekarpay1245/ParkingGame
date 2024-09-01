using _Game.UI;
using UnityEngine;

namespace _Game.Inputs
{
    /// <summary>
    /// Handles player input for both keyboard and touch controls and updates the PlayerInput ScriptableObject.
    /// Ensures that the input system is modular and easy to extend.
    /// </summary>
    public class InputHandler : MonoBehaviour
    {
        [Header("Player Input Configuration")]
        [Tooltip("ScriptableObject to store the player's input states.")]
        [SerializeField] private PlayerInputSO playerInput;

        [Header("Touch Controls Setup")]
        [Tooltip("Indicates if touch controls are configured.")]
        [SerializeField] private bool _touchControlsSetup = false;

        [Tooltip("Button used for accelerating.")]
        [SerializeField] private CustomButton _throttleButton;

        [Tooltip("Button used for reversing.")]
        [SerializeField] private CustomButton _reverseButton;

        [Tooltip("Button used for turning left.")]
        [SerializeField] private CustomButton _turnLeftButton;

        [Tooltip("Button used for turning right.")]
        [SerializeField] private CustomButton _turnRightButton;

        [Tooltip("Button used for handbraking.")]
        [SerializeField] private CustomButton _handbrakeButton;

        private void Start()
        {
            _touchControlsSetup = AreTouchControlsConfigured();
        }

        private void Update()
        {
            UpdateInputStates();
        }

        /// <summary>
        /// Verifies if all touch controls are properly configured.
        /// </summary>
        /// <returns>True if all touch controls are assigned; otherwise, false.</returns>
        private bool AreTouchControlsConfigured()
        {
            return _throttleButton != null &&
                   _reverseButton != null &&
                   _turnLeftButton != null &&
                   _turnRightButton != null &&
                   _handbrakeButton != null;
        }

        /// <summary>
        /// Updates the player's input states by checking both keyboard and touch inputs.
        /// </summary>
        private void UpdateInputStates()
        {
            playerInput.IsAccelerating = GetInput(KeyCode.W, _throttleButton);
            playerInput.IsReversing = GetInput(KeyCode.S, _reverseButton);
            playerInput.IsTurningLeft = GetInput(KeyCode.A, _turnLeftButton);
            playerInput.IsTurningRight = GetInput(KeyCode.D, _turnRightButton);
            playerInput.IsHandbraking = GetInput(KeyCode.Space, _handbrakeButton);
        }

        /// <summary>
        /// Checks if a specific keyboard key is pressed or if a touch control button is being pressed.
        /// </summary>
        /// <param name="key">The keyboard key to check.</param>
        /// <param name="button">The corresponding touch control button.</param>
        /// <returns>True if the key or button is pressed; otherwise, false.</returns>
        private bool GetInput(KeyCode key, CustomButton button)
        {
            return Input.GetKey(key) || (_touchControlsSetup && button != null && button.ButtonPressed);
        }
    }
}