using _Game._helpers.Particles;
using _Game.Car;
using _Game.Management;
using DG.Tweening;
using UnityEngine;

namespace _Game.Scripts
{
    /// <summary>
    /// Manages particle effects for the parking area, responding to parking events like 
    /// entering and exiting the parking area, with and without success. Ensures only one 
    /// particle system is active at any given time and uses DOTween for smooth transitions.
    /// </summary>
    public class ParkingArea : MonoBehaviour
    {
        [Header("Particle Keys")]
        [Tooltip("Key for the default parking area particle.")]
        [SerializeField] private string _parkingAreaParticleKey = "parking_area";

        [Tooltip("Key for the particle played when parking is not successful.")]
        [SerializeField] private string _parkingAreaWithoutSuccessParticleKey = "parking_area_not_success";

        [Tooltip("Key for the particle played when parking is successful.")]
        [SerializeField] private string _parkingAreaSuccessParkingParticleKey = "parking_area_success";

        [Header("Dispose and Init Settings")]
        [Tooltip("Time duration for scaling down before deactivating the object.")]
        [SerializeField, Range(0.1f, 2f)] private float _disposeTime = 0.5f;

        [Tooltip("Time duration for scaling up during initialization.")]
        [SerializeField, Range(0.1f, 2f)] private float _initTime = 0.5f;

        private ParticleSystem _parkingAreaParticle;
        private ParticleSystem _parkingAreaNoSuccessParticle;
        private ParticleSystem _parkingAreaSuccessParticle;

        private LevelManager _levelManager;
        private ParticleManager _particleManager;
        private CarParkingChecker _parkingChecker;

        private void Awake()
        {
            Initialize();
        }

        /// <summary>
        /// Initializes and sets up the necessary components.
        /// </summary>
        private void Initialize()
        {
            _levelManager = ServiceLocator.Get<LevelManager>();
            _particleManager = ServiceLocator.Get<ParticleManager>();
            _parkingChecker = ServiceLocator.Get<CarParkingChecker>();

            if (_particleManager == null || _parkingChecker == null || _levelManager == null)
            {
                Debug.LogError("One or more required managers are missing from the ServiceLocator.");
                return;
            }

            CacheInitialParticles();
            SubscribeToParkingEvents();
        }

        /// <summary>
        /// Caches the default parking area particle for immediate playback on start.
        /// </summary>
        private void CacheInitialParticles()
        {
            _parkingAreaParticle = _particleManager.PlayParticleAtPoint(
                _parkingAreaParticleKey, transform.position, Quaternion.identity, transform
            );
        }

        /// <summary>
        /// Subscribes to parking-related events from the CarParkingChecker.
        /// </summary>
        private void SubscribeToParkingEvents()
        {
            _parkingChecker.OnExitParkingArea += HandleExitParkingArea;
            _parkingChecker.OnEnterParkingAreaWithoutSuccess += HandleEnterParkingAreaWithoutSuccess;
            _parkingChecker.OnParkingSuccess += HandleParkingSuccessful;
        }

        /// <summary>
        /// Handles the event when the car exits the parking area without success.
        /// Plays the default parking area particle.
        /// </summary>
        private void HandleExitParkingArea()
        {
            PlayParticle(ref _parkingAreaParticle, _parkingAreaParticleKey);
        }

        /// <summary>
        /// Handles the event when the car enters the parking area without success.
        /// Plays the particle indicating unsuccessful parking.
        /// </summary>
        private void HandleEnterParkingAreaWithoutSuccess()
        {
            PlayParticle(ref _parkingAreaNoSuccessParticle, _parkingAreaWithoutSuccessParticleKey);
        }

        /// <summary>
        /// Handles the event when the car successfully parks.
        /// Plays the success particle and triggers level completion.
        /// </summary>
        private void HandleParkingSuccessful()
        {
            PlayParticle(ref _parkingAreaSuccessParticle, _parkingAreaSuccessParkingParticleKey);
            _levelManager.CompleteLevel();
        }

        /// <summary>
        /// Stops all active particles and plays the specified particle.
        /// This ensures only one particle is active at a time.
        /// </summary>
        /// <param name="particle">Reference to the ParticleSystem to be played.</param>
        /// <param name="particleKey">The key used to retrieve and play the particle.</param>
        private void PlayParticle(ref ParticleSystem particle, string particleKey)
        {
            StopAllParticles();

            if (particle == null)
            {
                particle = _particleManager.PlayParticleAtPoint(
                    particleKey, transform.position, Quaternion.identity, transform
                );
            }

            particle.Play();
        }

        /// <summary>
        /// Stops all currently active particle systems to avoid conflicts.
        /// </summary>
        private void StopAllParticles()
        {
            _parkingAreaParticle?.Stop();
            _parkingAreaNoSuccessParticle?.Stop();
            _parkingAreaSuccessParticle?.Stop();
        }

        /// <summary>
        /// Scales down the object and deactivates it after the specified dispose time.
        /// </summary>
        public void Dispose()
        {
            transform.DOScale(Vector3.zero, _disposeTime).OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
        }

        /// <summary>
        /// Initializes the object by scaling it up over the specified init time.
        /// </summary>
        public void Init()
        {
            transform.DOScale(Vector3.one, _initTime);
        }

        /// <summary>
        /// Visualizes the parking area using Gizmos in the Scene view.
        /// </summary>
        private void OnDrawGizmos()
        {
            Transform parkingAreaTransform = transform.GetChild(0);
            if (parkingAreaTransform != null)
            {
                Gizmos.color = new Color(1f, 1f, 0f, 0.25f);
                Gizmos.DrawCube(parkingAreaTransform.position, parkingAreaTransform.lossyScale);

                Gizmos.color = new Color(1f, 0.5f, 0f, 0.5f);
                Gizmos.DrawWireCube(parkingAreaTransform.position, parkingAreaTransform.lossyScale);
            }
        }

        /// <summary>
        /// Unsubscribes from all events to avoid memory leaks when the object is destroyed.
        /// </summary>
        private void OnDestroy()
        {
            if (_parkingChecker != null)
            {
                _parkingChecker.OnExitParkingArea -= HandleExitParkingArea;
                _parkingChecker.OnEnterParkingAreaWithoutSuccess -= HandleEnterParkingAreaWithoutSuccess;
                _parkingChecker.OnParkingSuccess -= HandleParkingSuccessful;
            }
        }
    }
}