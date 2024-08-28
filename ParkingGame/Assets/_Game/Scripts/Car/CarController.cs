using System;
using UnityEngine;
using _Game.Inputs;
using _Game.Cameras;
using _Game._helpers.Audios;

namespace _Game.Car
{
    public class CarController : MonoBehaviour
    {
        //CAR SETUP
        [Header("Car Configuration")]
        [Tooltip("Reference to the CarConfig ScriptableObject which contains all car parameters.")]
        [SerializeField]
        private CarConfigSO _carConfig;

        //CONTROLS
        [Header("Car Input Settings")]
        [Tooltip("ScriptableObject that holds player input events.")]
        [SerializeField]
        private PlayerInputSO _playerInput;

        //WHEELS
        [Header("WHEELS")]
        [SerializeField]
        private GameObject _frontLeftWheelMesh;
        [SerializeField]
        private WheelCollider _frontLeftWheelCollider;
        [Space(10)]
        [SerializeField]
        private GameObject _frontRightWheelMesh;
        [SerializeField]
        private WheelCollider _frontRightWheelCollider;
        [Space(10)]
        [SerializeField]
        private GameObject _backLeftWheelMesh;
        [SerializeField]
        private WheelCollider _backLeftWheelCollider;
        [Space(10)]
        [SerializeField]
        private GameObject _backRightWheelMesh;
        [SerializeField]
        private WheelCollider _backRightWheelCollider;

        //PARTICLE SYSTEMS

        [Space(20)]
        [Header("EFFECTS")]
        // The following particle systems are used as tire smoke when the car drifts.
        [SerializeField]
        private ParticleSystem _backLeftWheelSmokeParticle;
        [SerializeField]
        private ParticleSystem _backRightWheelSmokeParticle;

        [Space(10)]
        // The following trail renderers are used as tire skids when the car loses traction.
        [SerializeField]
        private TrailRenderer _backLeftWheelTrail;
        [SerializeField]
        private TrailRenderer _backRightWheelTrail;

        //SOUNDS
        [Space(20)]
        [Header("Sounds")]
        //The following variable lets you to set up sounds for your car such as the car engine or tire screech sounds.
        [SerializeField]
        private AudioSource _carEngineSound; // This variable stores the sound of the car engine.
        [SerializeField]
        private AudioSource _tireScreechSound; // This variable stores the sound of the tire screech (when the car is drifting).
        private float _initialCarEngineSoundPitch; // Used to store the initial pitch of the car engine sound.

        //CAR DATA
        private float _carSpeed; // Used to store the speed of the car.
        public float CarSpeed { get => _carSpeed; set => _carSpeed = value; }
        private bool _isDrifting; // Used to know whether the car is drifting or not.
        private bool _isTractionLocked; // Used to know whether the traction of the car is locked or not.

        //PRIVATE VARIABLES

        /*
        IMPORTANT: The following variables should not be modified manually since their values are automatically given via script.
        */
        private Rigidbody _carRigidbody; // Stores the car's rigidbody.
        private float _steeringAxis; // Used to know whether the steering wheel has reached the maximum value. It goes from -1 to 1.
        private float throttleAxis; // Used to know whether the throttle has reached the maximum value. It goes from -1 to 1.
        private float driftingAxis;
        private float localVelocityZ;
        private float localVelocityX;
        private bool _deceleratingCar;

        /*
        The following variables are used to store information about sideways friction of the wheels (such as
        extremumSlip,extremumValue, asymptoteSlip, asymptoteValue and stiffness). We change this values to
        make the car to start drifting.
        */
        private WheelFrictionCurve _fronLeftWheelFriction;
        private float _fronLeftWheelExtremumSlip;
        private WheelFrictionCurve _frontRightWheelFriction;
        private float _frontRightWheelExtremumSlip;
        private WheelFrictionCurve _backLeftWheelFriction;
        private float _backLeftWheelExtremumSlip;
        private WheelFrictionCurve _backRightWheelFriction;
        private float _backRightWheelExtremumSlip;

        // Start is called before the first frame update
        void Start()
        {
            CarFollowCameraController carFollowCameraController = ServiceLocator.Get<CarFollowCameraController>();
            carFollowCameraController.SetCarTransform(transform);

            //In this part, we set the 'carRigidbody' value with the Rigidbody attached to this
            //gameObject. Also, we define the center of mass of the car with the Vector3 given
            //in the inspector.
            _carRigidbody = GetComponent<Rigidbody>();
            _carRigidbody.centerOfMass = _carConfig.BodyMassCenter;

            //Initial setup to calculate the drift value of the car. This part could look a bit
            //complicated, but do not be afraid, the only thing we're doing here is to save the default
            //friction values of the car wheels so we can set an appropiate drifting value later.
            _fronLeftWheelFriction = new WheelFrictionCurve();
            _fronLeftWheelFriction.extremumSlip = _frontLeftWheelCollider.sidewaysFriction.extremumSlip;
            _fronLeftWheelExtremumSlip = _frontLeftWheelCollider.sidewaysFriction.extremumSlip;
            _fronLeftWheelFriction.extremumValue = _frontLeftWheelCollider.sidewaysFriction.extremumValue;
            _fronLeftWheelFriction.asymptoteSlip = _frontLeftWheelCollider.sidewaysFriction.asymptoteSlip;
            _fronLeftWheelFriction.asymptoteValue = _frontLeftWheelCollider.sidewaysFriction.asymptoteValue;
            _fronLeftWheelFriction.stiffness = _frontLeftWheelCollider.sidewaysFriction.stiffness;
            _frontRightWheelFriction = new WheelFrictionCurve();
            _frontRightWheelFriction.extremumSlip = _frontRightWheelCollider.sidewaysFriction.extremumSlip;
            _frontRightWheelExtremumSlip = _frontRightWheelCollider.sidewaysFriction.extremumSlip;
            _frontRightWheelFriction.extremumValue = _frontRightWheelCollider.sidewaysFriction.extremumValue;
            _frontRightWheelFriction.asymptoteSlip = _frontRightWheelCollider.sidewaysFriction.asymptoteSlip;
            _frontRightWheelFriction.asymptoteValue = _frontRightWheelCollider.sidewaysFriction.asymptoteValue;
            _frontRightWheelFriction.stiffness = _frontRightWheelCollider.sidewaysFriction.stiffness;
            _backLeftWheelFriction = new WheelFrictionCurve();
            _backLeftWheelFriction.extremumSlip = _backLeftWheelCollider.sidewaysFriction.extremumSlip;
            _backLeftWheelExtremumSlip = _backLeftWheelCollider.sidewaysFriction.extremumSlip;
            _backLeftWheelFriction.extremumValue = _backLeftWheelCollider.sidewaysFriction.extremumValue;
            _backLeftWheelFriction.asymptoteSlip = _backLeftWheelCollider.sidewaysFriction.asymptoteSlip;
            _backLeftWheelFriction.asymptoteValue = _backLeftWheelCollider.sidewaysFriction.asymptoteValue;
            _backLeftWheelFriction.stiffness = _backLeftWheelCollider.sidewaysFriction.stiffness;
            _backRightWheelFriction = new WheelFrictionCurve();
            _backRightWheelFriction.extremumSlip = _backRightWheelCollider.sidewaysFriction.extremumSlip;
            _backRightWheelExtremumSlip = _backRightWheelCollider.sidewaysFriction.extremumSlip;
            _backRightWheelFriction.extremumValue = _backRightWheelCollider.sidewaysFriction.extremumValue;
            _backRightWheelFriction.asymptoteSlip = _backRightWheelCollider.sidewaysFriction.asymptoteSlip;
            _backRightWheelFriction.asymptoteValue = _backRightWheelCollider.sidewaysFriction.asymptoteValue;
            _backRightWheelFriction.stiffness = _backRightWheelCollider.sidewaysFriction.stiffness;

            // We save the initial pitch of the car engine sound.
            _carEngineSound = transform.Find("Sounds").transform.Find("CarEngineSound")
                .GetComponent<AudioSource>();

            _tireScreechSound = transform.Find("Sounds").transform.Find("TireScreechSound")
                .GetComponent<AudioSource>();

            if (_carEngineSound != null)
            {
                _initialCarEngineSoundPitch = _carEngineSound.pitch;
            }
            else
            {
                Debug.Log("Sounds is null");
            }
            // We invoke 2 methods inside this script. CarSpeedUI() changes the text of the UI object that stores
            // the speed of the car and CarSounds() controls the engine and drifting sounds. Both methods are invoked
            // in 0 seconds, and repeatedly called every 0.1 seconds.

            InvokeRepeating("CarSounds", 0f, 0.1f);

            if (_backLeftWheelSmokeParticle != null)
            {
                _backLeftWheelSmokeParticle.Stop();
            }
            if (_backRightWheelSmokeParticle != null)
            {
                _backRightWheelSmokeParticle.Stop();
            }
            if (_backLeftWheelTrail != null)
            {
                _backLeftWheelTrail.emitting = false;
            }
            if (_backRightWheelTrail != null)
            {
                _backRightWheelTrail.emitting = false;
            }
        }

        // Update is called once per frame
        void Update()
        {
            //CAR DATA
            // We determine the speed of the car.
            _carSpeed = (2 * Mathf.PI * _frontLeftWheelCollider.radius * _frontLeftWheelCollider.rpm * 60) / 1000;
            // Save the local velocity of the car in the x axis. Used to know if the car is drifting.
            localVelocityX = transform.InverseTransformDirection(_carRigidbody.velocity).x;
            // Save the local velocity of the car in the z axis. Used to know if the car is going forward or backwards.
            localVelocityZ = transform.InverseTransformDirection(_carRigidbody.velocity).z;

            //CAR PHYSICS
            /*
            The next part is regarding to the car controller. First, it checks if the user wants to use touch controls (for
            mobile devices) or analog input controls (WASD + Space).

            The following methods are called whenever a certain key is pressed. For example, in the first 'if' we call the
            method GoForward() if the user has pressed W.

            In this part of the code we specify what the car needs to do if the user presses W (throttle), S (reverse),
            A (turn left), D (turn right) or Space bar (handbrake).
            */
            if (_playerInput.IsAccelerating)
            {
                CancelInvoke("DecelerateCar");
                _deceleratingCar = false;
                GoForward();
            }
            if (_playerInput.IsReversing)
            {
                CancelInvoke("DecelerateCar");
                _deceleratingCar = false;
                GoReverse();
            }
            if (_playerInput.IsTurningLeft)
            {
                TurnLeft();
            }
            if (_playerInput.IsTurningRight)
            {
                TurnRight();
            }
            if (_playerInput.IsHandbraking)
            {
                CancelInvoke("DecelerateCar");
                _deceleratingCar = false;
                Handbrake();
            }
            if (!_playerInput.IsHandbraking)
            {
                RecoverTraction();
            }
            if (!_playerInput.IsAccelerating && !_playerInput.IsReversing)
            {
                ThrottleOff();
            }
            if (!_playerInput.IsReversing && !_playerInput.IsAccelerating && !_playerInput.IsHandbraking && !_deceleratingCar)
            {
                InvokeRepeating("DecelerateCar", 0f, 0.1f);
                _deceleratingCar = true;
            }
            if (!_playerInput.IsTurningLeft && !_playerInput.IsTurningRight && _steeringAxis != 0f)
            {
                ResetSteeringAngle();
            }

            // We call the method AnimateWheelMeshes() in order to match the wheel collider movements with the 3D meshes of the wheels.
            AnimateWheelMeshes();
        }

        // This method controls the car sounds. For example, the car engine will sound slow when the car speed is low because the
        // pitch of the sound will be at its lowest point. On the other hand, it will sound fast when the car speed is high because
        // the pitch of the sound will be the sum of the initial pitch + the car speed divided by 100f.
        // Apart from that, the tireScreechSound will play whenever the car starts drifting or losing traction.
        public void CarSounds()
        {
            try
            {
                if (_carEngineSound != null)
                {
                    float engineSoundPitch = _initialCarEngineSoundPitch + (Mathf.Abs(_carRigidbody.velocity.magnitude) / 25f);
                    _carEngineSound.pitch = engineSoundPitch;
                }
                if ((_isDrifting) || (_isTractionLocked && Mathf.Abs(_carSpeed) > 12f))
                {
                    if (!_tireScreechSound.isPlaying)
                    {
                        _tireScreechSound.Play();
                    }
                }
                else if ((!_isDrifting) && (!_isTractionLocked || Mathf.Abs(_carSpeed) < 12f))
                {
                    _tireScreechSound.Stop();
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex);
            }
        }

        //
        //STEERING METHODS
        //

        //The following method turns the front car wheels to the left. The speed of this movement will depend on the steeringSpeed variable.
        public void TurnLeft()
        {
            _steeringAxis = _steeringAxis - (Time.deltaTime * 10f * _carConfig.SteeringSpeed);
            if (_steeringAxis < -1f)
            {
                _steeringAxis = -1f;
            }
            var steeringAngle = _steeringAxis * _carConfig.MaxSteeringAngle;
            _frontLeftWheelCollider.steerAngle = Mathf.Lerp(_frontLeftWheelCollider.steerAngle, steeringAngle, _carConfig.SteeringSpeed);
            _frontRightWheelCollider.steerAngle = Mathf.Lerp(_frontRightWheelCollider.steerAngle, steeringAngle, _carConfig.SteeringSpeed);
        }

        //The following method turns the front car wheels to the right. The speed of this movement will depend on the steeringSpeed variable.
        public void TurnRight()
        {
            _steeringAxis = _steeringAxis + (Time.deltaTime * 10f * _carConfig.SteeringSpeed);
            if (_steeringAxis > 1f)
            {
                _steeringAxis = 1f;
            }
            var steeringAngle = _steeringAxis * _carConfig.MaxSteeringAngle;
            _frontLeftWheelCollider.steerAngle = Mathf.Lerp(_frontLeftWheelCollider.steerAngle, steeringAngle, _carConfig.SteeringSpeed);
            _frontRightWheelCollider.steerAngle = Mathf.Lerp(_frontRightWheelCollider.steerAngle, steeringAngle, _carConfig.SteeringSpeed);
        }

        //The following method takes the front car wheels to their default position (rotation = 0). The speed of this movement will depend
        // on the steeringSpeed variable.
        public void ResetSteeringAngle()
        {
            if (_steeringAxis < 0f)
            {
                _steeringAxis = _steeringAxis + (Time.deltaTime * 10f * _carConfig.SteeringSpeed);
            }
            else if (_steeringAxis > 0f)
            {
                _steeringAxis = _steeringAxis - (Time.deltaTime * 10f * _carConfig.SteeringSpeed);
            }
            if (Mathf.Abs(_frontLeftWheelCollider.steerAngle) < 1f)
            {
                _steeringAxis = 0f;
            }
            var steeringAngle = _steeringAxis * _carConfig.MaxSteeringAngle;
            _frontLeftWheelCollider.steerAngle = Mathf.Lerp(_frontLeftWheelCollider.steerAngle, steeringAngle, _carConfig.SteeringSpeed);
            _frontRightWheelCollider.steerAngle = Mathf.Lerp(_frontRightWheelCollider.steerAngle, steeringAngle, _carConfig.SteeringSpeed);
        }

        // This method matches both the position and rotation of the WheelColliders with the WheelMeshes.
        void AnimateWheelMeshes()
        {
            try
            {
                Quaternion FLWRotation;
                Vector3 FLWPosition;
                _frontLeftWheelCollider.GetWorldPose(out FLWPosition, out FLWRotation);
                _frontLeftWheelMesh.transform.position = FLWPosition;
                _frontLeftWheelMesh.transform.rotation = FLWRotation;

                Quaternion FRWRotation;
                Vector3 FRWPosition;
                _frontRightWheelCollider.GetWorldPose(out FRWPosition, out FRWRotation);
                _frontRightWheelMesh.transform.position = FRWPosition;
                _frontRightWheelMesh.transform.rotation = FRWRotation;

                Quaternion RLWRotation;
                Vector3 RLWPosition;
                _backLeftWheelCollider.GetWorldPose(out RLWPosition, out RLWRotation);
                _backLeftWheelMesh.transform.position = RLWPosition;
                _backLeftWheelMesh.transform.rotation = RLWRotation;

                Quaternion RRWRotation;
                Vector3 RRWPosition;
                _backRightWheelCollider.GetWorldPose(out RRWPosition, out RRWRotation);
                _backRightWheelMesh.transform.position = RRWPosition;
                _backRightWheelMesh.transform.rotation = RRWRotation;
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex);
            }
        }

        //
        //ENGINE AND BRAKING METHODS
        //

        // This method apply positive torque to the wheels in order to go forward.
        public void GoForward()
        {
            //If the forces aplied to the rigidbody in the 'x' asis are greater than
            //3f, it means that the car is losing traction, then the car will start emitting particle systems.
            if (Mathf.Abs(localVelocityX) > 2.5f)
            {
                _isDrifting = true;
                DriftCarPS();
            }
            else
            {
                _isDrifting = false;
                DriftCarPS();
            }
            // The following part sets the throttle power to 1 smoothly.
            throttleAxis = throttleAxis + (Time.deltaTime * 3f);
            if (throttleAxis > 1f)
            {
                throttleAxis = 1f;
            }
            //If the car is going backwards, then apply brakes in order to avoid strange
            //behaviours. If the local velocity in the 'z' axis is less than -1f, then it
            //is safe to apply positive torque to go forward.
            if (localVelocityZ < -1f)
            {
                Brakes();
            }
            else
            {
                if (Mathf.RoundToInt(_carSpeed) < _carConfig.MaxSpeed)
                {
                    //Apply positive torque in all wheels to go forward if maxSpeed has not been reached.
                    _frontLeftWheelCollider.brakeTorque = 0;
                    _frontLeftWheelCollider.motorTorque = (_carConfig.AccelerationMultiplier * 50f) * throttleAxis;
                    _frontRightWheelCollider.brakeTorque = 0;
                    _frontRightWheelCollider.motorTorque = (_carConfig.AccelerationMultiplier * 50f) * throttleAxis;
                    _backLeftWheelCollider.brakeTorque = 0;
                    _backLeftWheelCollider.motorTorque = (_carConfig.AccelerationMultiplier * 50f) * throttleAxis;
                    _backRightWheelCollider.brakeTorque = 0;
                    _backRightWheelCollider.motorTorque = (_carConfig.AccelerationMultiplier * 50f) * throttleAxis;
                }
                else
                {
                    // If the maxSpeed has been reached, then stop applying torque to the wheels.
                    // IMPORTANT: The maxSpeed variable should be considered as an approximation; the speed of the car
                    // could be a bit higher than expected.
                    _frontLeftWheelCollider.motorTorque = 0;
                    _frontRightWheelCollider.motorTorque = 0;
                    _backLeftWheelCollider.motorTorque = 0;
                    _backRightWheelCollider.motorTorque = 0;
                }
            }
        }

        // This method apply negative torque to the wheels in order to go backwards.
        public void GoReverse()
        {
            //If the forces aplied to the rigidbody in the 'x' asis are greater than
            //3f, it means that the car is losing traction, then the car will start emitting particle systems.
            if (Mathf.Abs(localVelocityX) > 2.5f)
            {
                _isDrifting = true;
                DriftCarPS();
            }
            else
            {
                _isDrifting = false;
                DriftCarPS();
            }
            // The following part sets the throttle power to -1 smoothly.
            throttleAxis = throttleAxis - (Time.deltaTime * 3f);
            if (throttleAxis < -1f)
            {
                throttleAxis = -1f;
            }
            //If the car is still going forward, then apply brakes in order to avoid strange
            //behaviours. If the local velocity in the 'z' axis is greater than 1f, then it
            //is safe to apply negative torque to go reverse.
            if (localVelocityZ > 1f)
            {
                Brakes();
            }
            else
            {
                if (Mathf.Abs(Mathf.RoundToInt(_carSpeed)) < _carConfig.MaxReverseSpeed)
                {
                    //Apply negative torque in all wheels to go in reverse if maxReverseSpeed has not been reached.
                    _frontLeftWheelCollider.brakeTorque = 0;
                    _frontLeftWheelCollider.motorTorque = (_carConfig.AccelerationMultiplier * 50f) * throttleAxis;
                    _frontRightWheelCollider.brakeTorque = 0;
                    _frontRightWheelCollider.motorTorque = (_carConfig.AccelerationMultiplier * 50f) * throttleAxis;
                    _backLeftWheelCollider.brakeTorque = 0;
                    _backLeftWheelCollider.motorTorque = (_carConfig.AccelerationMultiplier * 50f) * throttleAxis;
                    _backRightWheelCollider.brakeTorque = 0;
                    _backRightWheelCollider.motorTorque = (_carConfig.AccelerationMultiplier * 50f) * throttleAxis;
                }
                else
                {
                    //If the maxReverseSpeed has been reached, then stop applying torque to the wheels.
                    // IMPORTANT: The maxReverseSpeed variable should be considered as an approximation; the speed of the car
                    // could be a bit higher than expected.
                    _frontLeftWheelCollider.motorTorque = 0;
                    _frontRightWheelCollider.motorTorque = 0;
                    _backLeftWheelCollider.motorTorque = 0;
                    _backRightWheelCollider.motorTorque = 0;
                }
            }
        }

        //The following function set the motor torque to 0 (in case the user is not pressing either W or S).
        public void ThrottleOff()
        {
            _frontLeftWheelCollider.motorTorque = 0;
            _frontRightWheelCollider.motorTorque = 0;
            _backLeftWheelCollider.motorTorque = 0;
            _backRightWheelCollider.motorTorque = 0;
        }

        // The following method decelerates the speed of the car according to the decelerationMultiplier variable, where
        // 1 is the slowest and 10 is the fastest deceleration. This method is called by the function InvokeRepeating,
        // usually every 0.1f when the user is not pressing W (throttle), S (reverse) or Space bar (handbrake).
        public void DecelerateCar()
        {
            if (Mathf.Abs(localVelocityX) > 2.5f)
            {
                _isDrifting = true;
                DriftCarPS();
            }
            else
            {
                _isDrifting = false;
                DriftCarPS();
            }
            // The following part resets the throttle power to 0 smoothly.
            if (throttleAxis != 0f)
            {
                if (throttleAxis > 0f)
                {
                    throttleAxis = throttleAxis - (Time.deltaTime * 10f);
                }
                else if (throttleAxis < 0f)
                {
                    throttleAxis = throttleAxis + (Time.deltaTime * 10f);
                }
                if (Mathf.Abs(throttleAxis) < 0.15f)
                {
                    throttleAxis = 0f;
                }
            }
            _carRigidbody.velocity = _carRigidbody.velocity * (1f / (1f + (0.025f * _carConfig.DecelerationMultiplier)));
            // Since we want to decelerate the car, we are going to remove the torque from the wheels of the car.
            _frontLeftWheelCollider.motorTorque = 0;
            _frontRightWheelCollider.motorTorque = 0;
            _backLeftWheelCollider.motorTorque = 0;
            _backRightWheelCollider.motorTorque = 0;
            // If the magnitude of the car's velocity is less than 0.25f (very slow velocity), then stop the car completely and
            // also cancel the invoke of this method.
            if (_carRigidbody.velocity.magnitude < 0.25f)
            {
                _carRigidbody.velocity = Vector3.zero;
                CancelInvoke("DecelerateCar");
            }
        }

        // This function applies brake torque to the wheels according to the brake force given by the user.
        public void Brakes()
        {
            _frontLeftWheelCollider.brakeTorque = _carConfig.BrakeForce;
            _frontRightWheelCollider.brakeTorque = _carConfig.BrakeForce;
            _backLeftWheelCollider.brakeTorque = _carConfig.BrakeForce;
            _backRightWheelCollider.brakeTorque = _carConfig.BrakeForce;
        }

        // This function is used to make the car lose traction. By using this, the car will start drifting. The amount of traction lost
        // will depend on the handbrakeDriftMultiplier variable. If this value is small, then the car will not drift too much, but if
        // it is high, then you could make the car to feel like going on ice.
        public void Handbrake()
        {
            CancelInvoke("RecoverTraction");
            // We are going to start losing traction smoothly, there is were our 'driftingAxis' variable takes
            // place. This variable will start from 0 and will reach a top value of 1, which means that the maximum
            // drifting value has been reached. It will increase smoothly by using the variable Time.deltaTime.
            driftingAxis = driftingAxis + (Time.deltaTime);
            float secureStartingPoint = driftingAxis * _fronLeftWheelExtremumSlip * _carConfig.HandbrakeDriftMultiplier;

            if (secureStartingPoint < _fronLeftWheelExtremumSlip)
            {
                driftingAxis = _fronLeftWheelExtremumSlip / (_fronLeftWheelExtremumSlip * _carConfig.HandbrakeDriftMultiplier);
            }
            if (driftingAxis > 1f)
            {
                driftingAxis = 1f;
            }
            //If the forces aplied to the rigidbody in the 'x' asis are greater than
            //3f, it means that the car lost its traction, then the car will start emitting particle systems.
            if (Mathf.Abs(localVelocityX) > 2.5f)
            {
                _isDrifting = true;
            }
            else
            {
                _isDrifting = false;
            }
            //If the 'driftingAxis' value is not 1f, it means that the wheels have not reach their maximum drifting
            //value, so, we are going to continue increasing the sideways friction of the wheels until driftingAxis
            // = 1f.
            if (driftingAxis < 1f)
            {
                _fronLeftWheelFriction.extremumSlip = _fronLeftWheelExtremumSlip * _carConfig.HandbrakeDriftMultiplier * driftingAxis;
                _frontLeftWheelCollider.sidewaysFriction = _fronLeftWheelFriction;

                _frontRightWheelFriction.extremumSlip = _frontRightWheelExtremumSlip * _carConfig.HandbrakeDriftMultiplier * driftingAxis;
                _frontRightWheelCollider.sidewaysFriction = _frontRightWheelFriction;

                _backLeftWheelFriction.extremumSlip = _backLeftWheelExtremumSlip * _carConfig.HandbrakeDriftMultiplier * driftingAxis;
                _backLeftWheelCollider.sidewaysFriction = _backLeftWheelFriction;

                _backRightWheelFriction.extremumSlip = _backRightWheelExtremumSlip * _carConfig.HandbrakeDriftMultiplier * driftingAxis;
                _backRightWheelCollider.sidewaysFriction = _backRightWheelFriction;
            }

            // Whenever the player uses the handbrake, it means that the wheels are locked, so we set 'isTractionLocked = true'
            // and, as a consequense, the car starts to emit trails to simulate the wheel skids.
            _isTractionLocked = true;
            DriftCarPS();

        }

        // This function is used to emit both the particle systems of the tires' smoke and the trail renderers of the tire skids
        // depending on the value of the bool variables 'isDrifting' and 'isTractionLocked'.
        public void DriftCarPS()
        {
            try
            {
                if (_isDrifting)
                {
                    _backLeftWheelSmokeParticle.Play();
                    _backRightWheelSmokeParticle.Play();
                }
                else if (!_isDrifting)
                {
                    _backLeftWheelSmokeParticle.Stop();
                    _backRightWheelSmokeParticle.Stop();
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex);
            }

            try
            {
                if ((_isTractionLocked || Mathf.Abs(localVelocityX) > 5f) && Mathf.Abs(_carSpeed) > 12f)
                {
                    _backLeftWheelTrail.emitting = true;
                    _backRightWheelTrail.emitting = true;
                }
                else
                {
                    _backLeftWheelTrail.emitting = false;
                    _backRightWheelTrail.emitting = false;
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex);
            }
        }

        // This function is used to recover the traction of the car when the user has stopped using the car's handbrake.
        public void RecoverTraction()
        {
            _isTractionLocked = false;
            driftingAxis = driftingAxis - (Time.deltaTime / 1.5f);
            if (driftingAxis < 0f)
            {
                driftingAxis = 0f;
            }

            //If the 'driftingAxis' value is not 0f, it means that the wheels have not recovered their traction.
            //We are going to continue decreasing the sideways friction of the wheels until we reach the initial
            // car's grip.
            if (_fronLeftWheelFriction.extremumSlip > _fronLeftWheelExtremumSlip)
            {
                _fronLeftWheelFriction.extremumSlip = _fronLeftWheelExtremumSlip * _carConfig.HandbrakeDriftMultiplier * driftingAxis;
                _frontLeftWheelCollider.sidewaysFriction = _fronLeftWheelFriction;

                _frontRightWheelFriction.extremumSlip = _frontRightWheelExtremumSlip * _carConfig.HandbrakeDriftMultiplier * driftingAxis;
                _frontRightWheelCollider.sidewaysFriction = _frontRightWheelFriction;

                _backLeftWheelFriction.extremumSlip = _backLeftWheelExtremumSlip * _carConfig.HandbrakeDriftMultiplier * driftingAxis;
                _backLeftWheelCollider.sidewaysFriction = _backLeftWheelFriction;

                _backRightWheelFriction.extremumSlip = _backRightWheelExtremumSlip * _carConfig.HandbrakeDriftMultiplier * driftingAxis;
                _backRightWheelCollider.sidewaysFriction = _backRightWheelFriction;

                Invoke("RecoverTraction", Time.deltaTime);

            }
            else if (_fronLeftWheelFriction.extremumSlip < _fronLeftWheelExtremumSlip)
            {
                _fronLeftWheelFriction.extremumSlip = _fronLeftWheelExtremumSlip;
                _frontLeftWheelCollider.sidewaysFriction = _fronLeftWheelFriction;

                _frontRightWheelFriction.extremumSlip = _frontRightWheelExtremumSlip;
                _frontRightWheelCollider.sidewaysFriction = _frontRightWheelFriction;

                _backLeftWheelFriction.extremumSlip = _backLeftWheelExtremumSlip;
                _backLeftWheelCollider.sidewaysFriction = _backLeftWheelFriction;

                _backRightWheelFriction.extremumSlip = _backRightWheelExtremumSlip;
                _backRightWheelCollider.sidewaysFriction = _backRightWheelFriction;

                driftingAxis = 0f;
            }
        }
    }
}