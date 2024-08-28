using _Game._helpers;
using _Game._helpers.Particles;
using _Game.Car;
using _Game.Management;
using UnityEngine;

namespace _Game.Scripts
{
    /// <summary>
    /// Manages particle effects and logic for the parking area, reacting to parking-related events.
    /// Uses a cached system for particle playback to enhance performance and ensures that only the required particle system is active at any given time.
    /// </summary>
    public class ParkingArea : MonoBehaviour
    {
        [Header("Particle Keys")]
        [Tooltip("Key for the default parking area particle.")]
        [SerializeField]
        private string _parkingAreaParticleKey = "parking_area";

        [Tooltip("Key for the particle played when parking is not successful.")]
        [SerializeField]
        private string _parkingAreaWithoutSuccessParticleKey = "parking_area_not_success";

        [Tooltip("Key for the particle played when parking is successful.")]
        [SerializeField]
        private string _parkingAreaSuccessParkingParticleKey = "parking_area_success";

        private ParticleSystem _parkingAreaParticle;
        private ParticleSystem _parkingAreaNoSuccessParticle;
        private ParticleSystem _parkingAreaSuccessParticle;

        private LevelManager _levelManager;
        private ParticleManager _particleManager;
        private CarParkingChecker _parkingChecker;

        /// <summary>
        /// Initializes and subscribes to parking-related events.
        /// </summary>
        private void Start()
        {
            _levelManager = ServiceLocator.Get<LevelManager>();

            _particleManager = ServiceLocator.Get<ParticleManager>(); // Access ParticleManager through ServiceLocator
            _parkingChecker = ServiceLocator.Get<CarParkingChecker>(); // Access CarParkingChecker through ServiceLocator

            SubscribeToParkingEvents();

            // Cache and play the initial particle for the parking area
            _parkingAreaParticle = _particleManager.PlayParticleAtPoint(_parkingAreaParticleKey,
                transform.position, Quaternion.identity, transform);
        }

        /// <summary>
        /// Subscribes to events from the CarParkingChecker.
        /// </summary>
        private void SubscribeToParkingEvents()
        {
            _parkingChecker.OnExitParkingArea += HandleExitParkingArea;
            _parkingChecker.OnEnterParkingAreaWithoutSuccess += HandleEnterParkingAreaWithoutSuccess;
            _parkingChecker.OnParkingSuccessful += HandleParkingSuccessful;
        }

        /// <summary>
        /// Handles the event when the car exits the parking area without success.
        /// Stops other particles and plays the default parking area particle.
        /// </summary>
        private void HandleExitParkingArea()
        {
            Debug.Log("Car exited the parking area without successful parking.");
            PlayParticle(ref _parkingAreaParticle, _parkingAreaParticleKey);
        }

        /// <summary>
        /// Handles the event when the car enters the parking area but parking has not yet been successful.
        /// Stops other particles and plays the 'no success' particle.
        /// </summary>
        private void HandleEnterParkingAreaWithoutSuccess()
        {
            Debug.Log("Car entered the parking area but parking is not successful yet.");
            PlayParticle(ref _parkingAreaNoSuccessParticle, _parkingAreaWithoutSuccessParticleKey);
        }

        /// <summary>
        /// Handles the event when parking is successfully completed.
        /// Stops other particles and plays the success particle.
        /// </summary>
        private void HandleParkingSuccessful()
        {
            Debug.Log("Parking has been successfully completed!");
            PlayParticle(ref _parkingAreaSuccessParticle, _parkingAreaSuccessParkingParticleKey);
            _levelManager.LevelComplete();
        }

        /// <summary>
        /// Stops any active particle systems and plays the specified particle.
        /// Ensures only the correct particle is playing at a given time.
        /// </summary>
        /// <param name="particle">Reference to the ParticleSystem to be played.</param>
        /// <param name="particleKey">The key for the particle to be retrieved and played.</param>
        private void PlayParticle(ref ParticleSystem particle, string particleKey)
        {
            StopAllParticles();
            if (particle == null)
            {
                particle = _particleManager.PlayParticleAtPoint(particleKey,
                    transform.position, Quaternion.identity, transform);
            }
            particle?.Play();
        }

        /// <summary>
        /// Stops all currently active particle systems.
        /// </summary>
        private void StopAllParticles()
        {
            _parkingAreaParticle?.Stop();
            _parkingAreaNoSuccessParticle?.Stop();
            _parkingAreaSuccessParticle?.Stop();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(255f, 255f, 0f, 0.25f);
            Gizmos.DrawCube(transform.GetChild(0).transform.position,
                transform.GetChild(0).transform.lossyScale);

            Gizmos.color = new Color(255f, 125f, 0f, 0.5f);
            Gizmos.DrawWireCube(transform.GetChild(0).transform.position,
                transform.GetChild(0).transform.lossyScale);
        }
    }
}