using UnityEngine;

namespace _Game.Scripts.Car
{
    /// <summary>
    /// Car configuration settings used for defining key car parameters
    /// such as max speed, acceleration, steering, and braking forces.
    /// These settings are stored as a ScriptableObject for easy configuration and reuse.
    /// </summary>
    [CreateAssetMenu(fileName = "CarConfig", menuName = "Data/CarConfig", order = 0)]
    public class CarConfigSO : ScriptableObject
    {
        [Header("Car Speed Settings")]
        [Tooltip("The maximum speed that the car can reach in km/h.")]
        [SerializeField, Range(20, 190)]
        private int _maxSpeed = 120;

        [Tooltip("The maximum speed the car can reach while in reverse, in km/h.")]
        [SerializeField, Range(10, 120)]
        private int _maxReverseSpeed = 60;

        [Tooltip("How fast the car can accelerate. 1 is slow, 10 is very fast.")]
        [SerializeField, Range(1, 10)]
        private int _accelerationMultiplier = 6;

        [Header("Car Steering Settings")]
        [Tooltip("The maximum angle the tires can reach while steering.")]
        [SerializeField, Range(10, 45)]
        private int _maxSteeringAngle = 30;

        [Tooltip("How fast the steering wheel turns.")]
        [SerializeField, Range(0.1f, 1f)]
        private float _steeringSpeed = 0.5f;

        [Header("Car Braking Settings")]
        [Tooltip("The strength of the wheel brakes.")]
        [SerializeField, Range(100, 600)]
        private int _brakeForce = 350;

        [Tooltip("How fast the car decelerates when not throttling.")]
        [SerializeField, Range(1, 10)]
        private int _decelerationMultiplier = 1;

        [Tooltip("How much grip the car loses when the handbrake is used.")]
        [SerializeField, Range(1, 10)]
        private int _handbrakeDriftMultiplier = 5;

        [Header("Car Physics Settings")]
        [Tooltip("The center of mass of the car's body, used for adjusting stability.")]
        [SerializeField]
        private Vector3 _bodyMassCenter;

        // Properties exposing the encapsulated fields

        public int MaxSpeed => _maxSpeed;
        public int MaxReverseSpeed => _maxReverseSpeed;
        public int AccelerationMultiplier => _accelerationMultiplier;
        public int MaxSteeringAngle => _maxSteeringAngle;
        public float SteeringSpeed => _steeringSpeed;
        public int BrakeForce => _brakeForce;
        public int DecelerationMultiplier => _decelerationMultiplier;
        public int HandbrakeDriftMultiplier => _handbrakeDriftMultiplier;
        public Vector3 BodyMassCenter => _bodyMassCenter;
    }
}