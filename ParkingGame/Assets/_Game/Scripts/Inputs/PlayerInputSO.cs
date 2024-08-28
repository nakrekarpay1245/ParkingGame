using UnityEngine;

namespace _Game.Inputs
{
    [CreateAssetMenu(fileName = "PlayerInput", menuName = "Data/PlayerInput")]
    public class PlayerInputSO : ScriptableObject
    {
        [SerializeField]
        private bool _isAccelerating;
        [SerializeField]
        private bool _isReversing;
        [SerializeField]
        private bool _isTurningLeft;
        [SerializeField]
        private bool _isTurningRight;
        [SerializeField]
        private bool _isHandbraking;
        [SerializeField]
        private bool _isDecelerating;
        [SerializeField]
        private float _steeringAxis;

        public bool IsAccelerating { get => _isAccelerating; set => _isAccelerating = value; }
        public bool IsReversing { get => _isReversing; set => _isReversing = value; }
        public bool IsTurningLeft { get => _isTurningLeft; set => _isTurningLeft = value; }
        public bool IsTurningRight { get => _isTurningRight; set => _isTurningRight = value; }
        public bool IsHandbraking { get => _isHandbraking; set => _isHandbraking = value; }
        public bool IsDecelerating { get => _isDecelerating; set => _isDecelerating = value; }
        public float SteeringAxis { get => _steeringAxis; set => _steeringAxis = value; }
    }
}