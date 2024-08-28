using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Game.Car
{
    /// <summary>
    /// This class handles the logic for checking if the car is properly parked within a defined 
    /// parking area. It ensures that no obstacles are present and the car's velocity is below a 
    /// threshold for successful parking.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class CarParkingChecker : MonoBehaviour
    {
        [Header("Parking Controller Parameters")]
        [Tooltip("List of obstacles detected within the parking area.")]
        [SerializeField]
        private List<GameObject> _parkingObstacleList = new List<GameObject>();

        [Tooltip("The Rigidbody component of the car.")]
        [SerializeField]
        private Rigidbody _carRigidbody;

        [Tooltip("Minimum velocity at which the car can be considered as parked.")]
        [Range(0f, 1f)]
        [SerializeField]
        private float _minimumVelocityForParking = 0.01f;

        private bool _isParkingSuccessful = false;
        private bool _isInParkingArea = false;

        // Actions for external event handling
        public Action OnEnterParkingAreaWithoutSuccess;
        public Action OnParkingSuccessful;
        public Action OnExitParkingArea;

        /// <summary>
        /// Initializes the required components.
        /// </summary>
        private void Awake()
        {
            if (_carRigidbody == null)
            {
                _carRigidbody = GetComponent<Rigidbody>();
            }

            ServiceLocator.Register(this);
        }

        /// <summary>
        /// Handles the logic for successful parking.
        /// </summary>
        private void SuccessParking()
        {
            if (!_isParkingSuccessful)
            {
                _isParkingSuccessful = true;
                Debug.Log("Parking Successful!");

                // Trigger the parking success action
                OnParkingSuccessful?.Invoke();

                // Example of further game logic
                //GlobalBinder.singleton.LevelManager.CompleteGame();
            }
        }

        /// <summary>
        /// Called when a collider enters the trigger area. Detects obstacles within the parking frame.
        /// </summary>
        /// <param name="other">The collider that entered the trigger.</param>
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("ParkingFrame"))
            {
                AddObstacle(other.gameObject);
            }

            if (other.CompareTag("ParkingArea") && !_isParkingSuccessful)
            {
                _isInParkingArea = true;
                Debug.Log("Entered parking area but parking not successful yet.");

                // Trigger the action for entering the parking area without success
                OnEnterParkingAreaWithoutSuccess?.Invoke();
            }
        }

        /// <summary>
        /// Called while a collider stays in the trigger area. Checks for successful parking conditions.
        /// </summary>
        /// <param name="other">The collider staying in the trigger.</param>
        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("ParkingArea") && CanPark())
            {
                SuccessParking();
            }
        }

        /// <summary>
        /// Called when a collider exits the trigger area. Removes obstacles when they leave the parking frame.
        /// </summary>
        /// <param name="other">The collider that exited the trigger.</param>
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("ParkingFrame"))
            {
                RemoveObstacle(other.gameObject);
            }

            if (other.CompareTag("ParkingArea"))
            {
                _isInParkingArea = false;
                Debug.Log("Exited parking area.");

                // Trigger the action for exiting the parking area
                OnExitParkingArea?.Invoke();
            }
        }

        /// <summary>
        /// Adds an obstacle to the parking obstacle list if it's not already there.
        /// </summary>
        /// <param name="obstacle">The obstacle to add.</param>
        private void AddObstacle(GameObject obstacle)
        {
            if (!_parkingObstacleList.Contains(obstacle))
            {
                _parkingObstacleList.Add(obstacle);
            }
        }

        /// <summary>
        /// Removes an obstacle from the parking obstacle list if it's present.
        /// </summary>
        /// <param name="obstacle">The obstacle to remove.</param>
        private void RemoveObstacle(GameObject obstacle)
        {
            if (_parkingObstacleList.Contains(obstacle))
            {
                _parkingObstacleList.Remove(obstacle);
            }
        }

        /// <summary>
        /// Determines whether the car can park successfully by checking for obstacles and velocity.
        /// </summary>
        /// <returns>True if the car can park successfully; otherwise, false.</returns>
        private bool CanPark()
        {
            return _parkingObstacleList.Count == 0 &&
                _carRigidbody.velocity.magnitude <= _minimumVelocityForParking &&
                _isInParkingArea;
        }
    }
}