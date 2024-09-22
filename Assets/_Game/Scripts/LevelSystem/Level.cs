using _Game.Car;
using _Game.Data;
using _Game.Management;
using _Game.Scripts;
using DG.Tweening;
using System.Collections;
using UnityEngine;

namespace _Game.LevelSystem
{
    /// <summary>
    /// The Level class handles the initialization of essential level components 
    /// such as CarController, LevelFrame, and ParkingArea. It also manages 
    /// cleanup and disposal with a delayed mechanism, using DOTween.
    /// </summary>
    public class Level : MonoBehaviour
    {
        [Header("Game Data")]
        [Tooltip("Reference to the game data ScriptableObject.")]
        [SerializeField] private GameData _gameData;

        [Header("Parking Area Setup")]
        [Tooltip("Prefab for the Parking Area to be instantiated.")]
        [SerializeField] private string _parkingAreaPrefabPath = "ParkingArea";


        [Header("Level Frame Setup")]
        [Tooltip("Reference to the LevelFrame component.")]
        [SerializeField] private LevelFrame _levelFrame;

        [Header("Points Setup")]
        [Tooltip("Transform for the Parking Area spawn position.")]
        [SerializeField] private Transform _parkingAreaPoint;

        [Tooltip("Transform for the Player spawn position.")]
        [SerializeField] private Transform _playerPoint;

        [Header("Dispose Settings")]
        [Tooltip("Time delay (in seconds) before disposing of the level objects.")]
        [SerializeField, Range(0.1f, 5f)] private float _disposeDelay = 1f;

        private CarController _carController;
        private ParkingArea _parkingArea;
        private LevelManager _levelManager;

        private void Awake()
        {
            StartCoroutine(InitializeDependencies());
        }

        /// <summary>
        /// Coroutine that waits for dependencies to be registered before proceeding.
        /// </summary>
        private IEnumerator InitializeDependencies()
        {
            while (_levelManager == null)
            {
                _levelManager = ServiceLocator.Get<LevelManager>();
                yield return null;
            }
        }

        /// <summary>
        /// Subscribes to level completion and failure events to handle cleanup.
        /// </summary>
        private void SubscribeToEvents()
        {
            if (!_levelManager)
            {
                _levelManager = ServiceLocator.Get<LevelManager>();
            }
            _levelManager.OnLevelComplete += HandleLevelEnd;
            _levelManager.OnLevelFail += HandleLevelEnd;
        }

        /// <summary>
        /// Ensures that all required references and transforms are correctly set up.
        /// </summary>
        private void ValidateInitialReferences()
        {
            if (_parkingAreaPrefabPath == null)
            {
                Debug.LogError("ParkingAreaPrefab is missing.");
                return;
            }

            _playerPoint = transform.Find("PlayerPoint");
            _parkingAreaPoint = transform.Find("ParkingAreaPoint");

            if (_playerPoint == null || _parkingAreaPoint == null)
            {
                Debug.LogError("PlayerPoint or ParkingAreaPoint not found in the Level object.");
            }

            if (_levelFrame == null)
            {
                _levelFrame = GetComponentInChildren<LevelFrame>();
                if (_levelFrame == null)
                {
                    Debug.LogError("LevelFrame component is missing.");
                }
            }
        }

        /// <summary>
        /// Initializes the core components of the level: CarController, ParkingArea, and LevelFrame.
        /// </summary>
        private void InitializeLevelComponents()
        {
            _carController = InstantiateCarController();
            _parkingArea = InstantiateParkingArea();
        }

        /// <summary>
        /// Instantiates the CarController at the PlayerPoint position using Resources.Load to load the prefab.
        /// The prefab is loaded based on the string key stored in the SelectedCarConfig.
        /// </summary>
        /// <returns>Returns the instantiated CarController object.</returns>
        private CarController InstantiateCarController()
        {
            if (_playerPoint == null)
            {
                Debug.LogError("Failed to instantiate CarController: PlayerPoint is missing.");
                return null;
            }

            // Check if the selected car has a valid config and prefab key
            string carPrefabKey = _gameData.SelectedCarPrefabKey;
            if (!string.IsNullOrEmpty(carPrefabKey))
            {
                // Load the car prefab using Resources.Load with the key/path
                CarController carPrefab = Resources.Load<CarController>(carPrefabKey);

                if (carPrefab != null)
                {
                    // Instantiate the car prefab at the player point
                    return Instantiate(carPrefab, _playerPoint.position, _playerPoint.rotation);
                }
                else
                {
                    Debug.LogError($"Failed to load CarController prefab: No prefab found at path '{carPrefabKey}'.");
                }
            }
            else
            {
                Debug.LogError("Failed to instantiate CarController: SelectedCarConfig does not contain a valid prefab key.");
            }

            return null;
        }

        /// <summary>
        /// Instantiates the ParkingArea at the ParkingAreaPoint position by loading the prefab 
        /// via Resources.Load. This ensures flexibility in loading assets at runtime while maintaining 
        /// good performance.
        /// </summary>
        /// <returns>Returns the instantiated ParkingArea object.</returns>
        private ParkingArea InstantiateParkingArea()
        {
            if (_parkingAreaPoint == null)
            {
                Debug.LogError("Failed to instantiate ParkingArea: ParkingAreaPoint is missing.");
                return null;
            }

            // Check if the parking area prefab exists in Resources with the expected path
            ParkingArea parkingAreaPrefab = Resources.Load<ParkingArea>(_parkingAreaPrefabPath);

            if (parkingAreaPrefab != null)
            {
                // Instantiate the parking area at the specified spawn point
                return Instantiate(parkingAreaPrefab, _parkingAreaPoint.position, _parkingAreaPoint.rotation);
            }
            else
            {
                Debug.LogError($"Failed to load ParkingArea prefab: No prefab found at path '{_parkingAreaPrefabPath}'.");
                return null;
            }
        }

        /// <summary>
        /// Handles the cleanup of level objects after a delay when the level is completed or failed.
        /// </summary>
        private void HandleLevelEnd()
        {
            if (_disposeDelay > 0)
            {
                DOVirtual.DelayedCall(_disposeDelay, Dispose);
            }
            else
            {
                Dispose();
            }
        }

        /// <summary>
        /// Cleans up the level objects by calling their respective dispose methods.
        /// </summary>
        private void Dispose()
        {
            _carController?.Dispose();
            _parkingArea?.Dispose();
            _levelFrame?.Dispose();
        }

        public void Init()
        {
            ValidateInitialReferences();
            InitializeLevelComponents();

            _carController?.Init();
            _parkingArea?.Init();
            _levelFrame?.Init();

            SubscribeToEvents();
        }

        private void OnDestroy()
        {
            if (_levelManager != null)
            {
                _levelManager.OnLevelComplete -= HandleLevelEnd;
                _levelManager.OnLevelFail -= HandleLevelEnd;
            }
        }
    }
}