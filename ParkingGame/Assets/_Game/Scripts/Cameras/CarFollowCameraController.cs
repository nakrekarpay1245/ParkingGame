using UnityEngine;
using DG.Tweening; // DOTween for smooth transitions

namespace _Game.Scripts.Cameras
{
    /// <summary>
    /// CarFollowCameraController manages the camera to smoothly follow and look at the target car.
    /// It uses customizable speeds for both the follow and look behaviors.
    /// </summary>
    public class CarFollowCameraController : MonoBehaviour
    {
        [Header("Camera Settings")]
        [Tooltip("Speed at which the camera follows the car.")]
        [Range(1f, 10f)]
        [SerializeField] private float _followSpeed = 2f;

        [Tooltip("Speed at which the camera looks at the car.")]
        [Range(1f, 10f)]
        [SerializeField] private float _lookSpeed = 5f;

        private Transform _carTransform;
        private Vector3 _initialCameraOffset;
        private Vector3 _initialCameraPosition;

        private void Awake()
        {
            // Initialize camera position
            _initialCameraPosition = transform.position;

            ServiceLocator.Register(this);
        }

        private void FixedUpdate()
        {
            if (_carTransform != null)
            {
                FollowCar();
                LookAtCar();
            }
        }

        /// <summary>
        /// Sets the car transform and initializes camera offset.
        /// </summary>
        /// <param name="carTransform">The transform of the car to follow.</param>
        public void SetCarTransform(Transform carTransform)
        {
            _carTransform = carTransform;
            if (_carTransform != null)
            {
                _initialCameraOffset = _initialCameraPosition - _carTransform.position;
                Debug.Log("CarTransform set and initial camera offset calculated.");
            }
            else
            {
                Debug.LogError("Failed to set CarTransform: Transform is null.");
            }
        }

        /// <summary>
        /// Smoothly follows the car based on the initial camera offset.
        /// Uses Lerp for smooth transitions.
        /// </summary>
        private void FollowCar()
        {
            Vector3 targetPosition = _carTransform.position + _initialCameraOffset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, _followSpeed * Time.deltaTime);
        }

        /// <summary>
        /// Smoothly rotates the camera to look at the car.
        /// Uses Lerp for smooth rotation.
        /// </summary>
        private void LookAtCar()
        {
            Vector3 directionToLook = _carTransform.position - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(directionToLook, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, _lookSpeed * Time.deltaTime);
        }

        /// <summary>
        /// Optionally uses DOTween for smoother camera transitions.
        /// This function could be called when you want a more cinematic camera movement.
        /// </summary>
        public void SmoothFollowWithDOTween()
        {
            if (_carTransform != null)
            {
                Vector3 targetPosition = _carTransform.position + _initialCameraOffset;
                transform.DOMove(targetPosition, 1f / _followSpeed).SetEase(Ease.InOutQuad);
            }
        }

        public void SmoothLookAtWithDOTween()
        {
            if (_carTransform != null)
            {
                Vector3 directionToLook = _carTransform.position - transform.position;
                Quaternion targetRotation = Quaternion.LookRotation(directionToLook, Vector3.up);
                transform.DORotateQuaternion(targetRotation, 1f / _lookSpeed).SetEase(Ease.InOutQuad);
            }
        }
    }
}