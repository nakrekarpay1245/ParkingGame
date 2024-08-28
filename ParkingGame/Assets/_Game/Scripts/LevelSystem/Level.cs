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
        [SerializeField] private GameObject _carControllerPrefab;

        [Header("Parking Area Setup")]
        [Tooltip("Prefab for the Parking Area to be instantiated.")]
        [SerializeField] private GameObject _parkingAreaPrefab;

        [Header("Level Frame Setup")]
        [Tooltip("Reference to the LevelFrame component.")]
        [SerializeField] private LevelFrame _levelFrame;

        [Header("Points Setup")]
        [Tooltip("Transform for the Parking Area point.")]
        [SerializeField] private Transform _parkingAreaPoint;

        [Tooltip("Transform for the Player point.")]
        [SerializeField] private Transform _playerPoint;

        private void Start()
        {
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
            InstantiateCarController();

            // Initialize the LevelFrame
            _levelFrame.Init();

            // Generate the ParkingArea
            GenerateParkingArea();
        }

        /// <summary>
        /// Instantiates the CarController at the PlayerPoint position.
        /// </summary>
        private void InstantiateCarController()
        {
            if (_playerPoint != null)
            {
                Instantiate(_carControllerPrefab, _playerPoint.position, _playerPoint.rotation);
                //Debug.Log("CarController instantiated at PlayerPoint.");
            }
            else
            {
                Debug.LogError("PlayerPoint is not assigned.");
            }
        }

        /// <summary>
        /// Generates the Parking Area at the ParkingAreaPoint position.
        /// </summary>
        private void GenerateParkingArea()
        {
            if (_parkingAreaPoint != null)
            {
                Instantiate(_parkingAreaPrefab, _parkingAreaPoint.position, _parkingAreaPoint.rotation);
                //Debug.Log("ParkingArea generated at ParkingAreaPoint.");
            }
            else
            {
                Debug.LogError("ParkingAreaPoint is not assigned.");
            }
        }
    }
}