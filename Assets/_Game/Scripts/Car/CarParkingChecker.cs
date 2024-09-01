using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; // For animations with DOTween

namespace _Game.Car
{
    /// <summary>
    /// Handles logic for detecting if the car is correctly parked within a designated parking area.
    /// Ensures the car's velocity is below a threshold and there are no obstacles in the parking area.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class CarParkingChecker : MonoBehaviour
    {
        [Header("Parking Configuration")]
        [Tooltip("Minimum velocity for the car to be considered parked.")]
        [Range(0f, 1f)]
        [SerializeField]
        private float _minimumVelocityForParking = 0.01f;

        [Tooltip("The Rigidbody component of the car.")]
        [SerializeField]
        private Rigidbody _carRigidbody;

        [Header("Obstacle Detection")]
        [Tooltip("List of obstacles detected in the parking area.")]
        [SerializeField, HideInInspector]
        private HashSet<GameObject> _parkingObstacles = new HashSet<GameObject>();

        private bool _isParkingSuccessful = false;
        private bool _isInParkingArea = false;

        // Events for external handling
        public Action OnParkingSuccess;
        public Action OnEnterParkingAreaWithoutSuccess;
        public Action OnExitParkingArea;

        /// <summary>
        /// Initializes the required components and services.
        /// </summary>
        private void Awake()
        {
            _carRigidbody = GetComponent<Rigidbody>();

            // Register the service (optional)
            ServiceLocator.Register(this);
        }

        /// <summary>
        /// Handles parking success logic.
        /// </summary>
        private void SuccessParking()
        {
            if (_isParkingSuccessful) return;

            _isParkingSuccessful = true;
            Debug.Log("Parking Successful!");

            // Trigger success event
            OnParkingSuccess?.Invoke();

            // Optional: Apply animation when parking is successful (e.g., car smoothly settles)
            transform.DOLocalMoveY(0.2f, 1f).SetEase(Ease.OutBounce);
        }

        /// <summary>
        /// Detects when an object enters the parking trigger area and handles obstacle and area entry logic.
        /// </summary>
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("ParkingFrame"))
            {
                AddObstacle(other.gameObject);
            }
            else if (other.CompareTag("ParkingArea") && !_isParkingSuccessful)
            {
                _isInParkingArea = true;
                OnEnterParkingAreaWithoutSuccess?.Invoke();
            }
        }

        /// <summary>
        /// Continuously checks for parking conditions when an object remains in the parking area trigger.
        /// </summary>
        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("ParkingArea") && CanPark())
            {
                SuccessParking();
            }
        }

        /// <summary>
        /// Detects when an object exits the parking trigger area and handles obstacle and area exit logic.
        /// </summary>
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("ParkingFrame"))
            {
                RemoveObstacle(other.gameObject);
            }
            else if (other.CompareTag("ParkingArea"))
            {
                _isInParkingArea = false;
                OnExitParkingArea?.Invoke();
            }
        }

        /// <summary>
        /// Adds an obstacle to the detected obstacle list if it hasn't already been added.
        /// </summary>
        private void AddObstacle(GameObject obstacle)
        {
            _parkingObstacles.Add(obstacle);
        }

        /// <summary>
        /// Removes an obstacle from the detected obstacle list if it's present.
        /// </summary>
        private void RemoveObstacle(GameObject obstacle)
        {
            _parkingObstacles.Remove(obstacle);
        }

        /// <summary>
        /// Determines whether the car can be considered parked based on the absence of obstacles, 
        /// low velocity, and being within the parking area.
        /// </summary>
        /// <returns>True if the car can be parked; otherwise, false.</returns>
        private bool CanPark()
        {
            return _parkingObstacles.Count == 0 &&
                   _carRigidbody.velocity.magnitude <= _minimumVelocityForParking &&
                   _isInParkingArea;
        }
    }
}