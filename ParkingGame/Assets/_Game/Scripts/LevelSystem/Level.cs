using _Game.Car;
using _Game.Data;
using _Game.Management;
using _Game.Scripts;
using DG.Tweening;
using UnityEngine;

namespace _Game.LevelSystem
{
    /// <summary>
    /// The Level class is responsible for initializing the CarController, LevelFrame, 
    /// and ParkingArea based on specific child object positions. It also manages the 
    /// cleanup process with a delay before disposing of the objects.
    /// </summary>
    public class Level : MonoBehaviour
    {
        [Header("Game Data")]
        [Tooltip("Reference to the game data ScriptableObject.")]
        [SerializeField] private GameData _gameData;

        [Header("Parking Area Setup")]
        [Tooltip("Prefab for the Parking Area to be instantiated.")]
        [SerializeField] private ParkingArea _parkingAreaPrefab;

        [Header("Level Frame Setup")]
        [Tooltip("Reference to the LevelFrame component.")]
        [SerializeField] private LevelFrame _levelFrame;

        [Header("Points Setup")]
        [Tooltip("Transform for the Parking Area point.")]
        [SerializeField] private Transform _parkingAreaPoint;

        [Tooltip("Transform for the Player point.")]
        [SerializeField] private Transform _playerPoint;

        [Header("Dispose Settings")]
        [Tooltip("Time delay before disposing of level objects.")]
        [SerializeField, Range(0.1f, 5f)] private float _disposeDelay = 1f;

        private CarController _carController;
        private ParkingArea _parkingArea;
        private LevelManager _levelManager;

        private void Awake()
        {
            _levelManager = ServiceLocator.Get<LevelManager>();
        }

        private void Start()
        {
            _levelManager.OnLevelComplete += DelayedDispose;
            _levelManager.OnLevelFail += DelayedDispose;

            // Ensure required prefabs and references are set
            if (_parkingAreaPrefab == null)
            {
                Debug.LogError("One or more references are missing in the Level component.");
                return;
            }

            // Find child objects by name
            _playerPoint = transform.Find("PlayerPoint");
            _parkingAreaPoint = transform.Find("ParkingAreaPoint");
            _levelFrame = GetComponentInChildren<LevelFrame>();

            // Ensure the points are found
            if (_playerPoint == null || _parkingAreaPoint == null)
            {
                Debug.LogError("PlayerPoint or ParkingAreaPoint not found in the Level object.");
                return;
            }

            // Instantiate the CarController
            _carController = InstantiateCarController();

            // Initialize the LevelFrame
            _levelFrame.Init();

            // Generate the ParkingArea
            _parkingArea = GenerateParkingArea();
        }

        /// <summary>
        /// Instantiates the CarController at the PlayerPoint position.
        /// </summary>
        private CarController InstantiateCarController()
        {
            CarController carControllerPrefab = _gameData.SelectedCarPrefab;

            if (_playerPoint != null)
            {
                CarController carController = Instantiate(
                    carControllerPrefab,
                    _playerPoint.position,
                    _playerPoint.rotation
                );

                //Debug.Log("CarController instantiated at PlayerPoint.");
                return carController;
            }
            Debug.LogError("PlayerPoint is not assigned.");
            return null;
        }

        /// <summary>
        /// Generates the Parking Area at the ParkingAreaPoint position.
        /// </summary>
        private ParkingArea GenerateParkingArea()
        {
            if (_parkingAreaPoint != null)
            {
                ParkingArea parkingArea = Instantiate(
                    _parkingAreaPrefab,
                    _parkingAreaPoint.position,
                    _parkingAreaPoint.rotation
                );

                //Debug.Log("ParkingArea generated at ParkingAreaPoint.");
                return parkingArea;
            }
            Debug.LogError("ParkingAreaPoint is not assigned.");
            return null;
        }

        /// <summary>
        /// Adds a delay before calling the Dispose function.
        /// </summary>
        private void DelayedDispose()
        {
            // Use DOTween to delay the call to Dispose
            DOVirtual.DelayedCall(_disposeDelay, Dispose);
        }

        /// <summary>
        /// Disposes of the level objects (CarController, LevelFrame, ParkingArea).
        /// </summary>
        private void Dispose()
        {
            _levelFrame?.Dispose();
            _parkingArea?.Dispose();
            _carController?.Dispose();
        }
    }
}