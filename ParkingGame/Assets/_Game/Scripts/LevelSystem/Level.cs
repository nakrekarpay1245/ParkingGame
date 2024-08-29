using _Game.Car;
using _Game.Management;
using _Game.Scripts;
using UnityEngine;

namespace _Game.LevelSystem
{
    /// <summary>
    /// The Level class is responsible for initializing the CarController, LevelFrame, 
    /// and ParkingArea based on specific child object positions.
    /// </summary>
    public class Level : MonoBehaviour
    {
        [Header("Car Setup")]
        [Tooltip("Prefab for the CarController to be instantiated.")]
        [SerializeField] private CarController _carControllerPrefab;

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

        private CarController _carController;
        private ParkingArea _parkingArea;
        private LevelManager _levelManager;

        private void Awake()
        {
            _levelManager = ServiceLocator.Get<LevelManager>();
        }

        private void Start()
        {
            _levelManager.OnLevelComplete += Dispose;
            _levelManager.OnLevelFail += Dispose;

            // Ensure required prefabs and references are set
            if (_carControllerPrefab == null || _parkingAreaPrefab == null)
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
            if (_playerPoint != null)
            {
                CarController carController =
                    Instantiate(_carControllerPrefab, _playerPoint.position, _playerPoint.rotation);

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
                ParkingArea parkingArea = Instantiate(_parkingAreaPrefab, _parkingAreaPoint.position, _parkingAreaPoint.rotation);
                //Debug.Log("ParkingArea generated at ParkingAreaPoint.");
                return parkingArea;
            }
            Debug.LogError("ParkingAreaPoint is not assigned.");
            return null;
        }

        private void Dispose()
        {
            _levelFrame.Dispose();
            _parkingArea.Dispose();
            _carController.Dispose();
        }
    }
}