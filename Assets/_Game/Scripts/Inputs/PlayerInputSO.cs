using UnityEngine;

namespace _Game.Inputs
{
    /// <summary>
    /// ScriptableObject that holds the state of player inputs for vehicle control.
    /// </summary>
    [CreateAssetMenu(fileName = "PlayerInput", menuName = "Data/PlayerInput")]
    public class PlayerInputSO : ScriptableObject
    {
        [Header("Input States")]
        [Tooltip("True if the player is accelerating.")]
        [SerializeField] private bool _isAccelerating;

        [Tooltip("True if the player is reversing.")]
        [SerializeField] private bool _isReversing;

        [Tooltip("True if the player is turning left.")]
        [SerializeField] private bool _isTurningLeft;

        [Tooltip("True if the player is turning right.")]
        [SerializeField] private bool _isTurningRight;

        [Tooltip("True if the player is applying the handbrake.")]
        [SerializeField] private bool _isHandbraking;

        [Tooltip("True if the player is decelerating.")]
        [SerializeField] private bool _isDecelerating;

        [Tooltip("Steering axis value, used to determine the direction and degree of turning.")]
        [SerializeField][Range(-1f, 1f)] private float _steeringAxis;

        // Public properties for accessing input states
        public bool IsAccelerating { get; set; }
        public bool IsReversing { get; set; }
        public bool IsTurningLeft { get; set; }
        public bool IsTurningRight { get; set; }
        public bool IsHandbraking { get; set; }
        public bool IsDecelerating { get; set; }
        public float SteeringAxis { get; set; }
    }
}