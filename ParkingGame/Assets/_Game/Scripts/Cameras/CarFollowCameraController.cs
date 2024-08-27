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
        [Header("Target Settings")]
        [Tooltip("The transform of the car that the camera will follow.")]
        [SerializeField] private Transform carTransform;

        [Header("Camera Settings")]
        [Tooltip("Speed at which the camera follows the car.")]
        [Range(1f, 10f)]
        [SerializeField] private float followSpeed = 2f;

        [Tooltip("Speed at which the camera looks at the car.")]
        [Range(1f, 10f)]
        [SerializeField] private float lookSpeed = 5f;

        private Vector3 _initialCameraOffset;
        private Vector3 _initialCameraPosition;

        private void Awake()
        {
            Initialize();
        }

        /// <summary>
        /// Initializes camera position relative to the car.
        /// </summary>
        private void Initialize()
        {
            _initialCameraPosition = transform.position;
            if (carTransform != null)
            {
                _initialCameraOffset = _initialCameraPosition - carTransform.position;
            }
            else
            {
                Debug.LogError("Car Transform is not assigned.");
            }
        }

        private void FixedUpdate()
        {
            if (carTransform != null)
            {
                FollowCar();
                LookAtCar();
            }
        }

        /// <summary>
        /// Smoothly follows the car based on the initial camera offset.
        /// Uses Lerp for smooth transitions.
        /// </summary>
        private void FollowCar()
        {
            Vector3 targetPosition = carTransform.position + _initialCameraOffset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        }

        /// <summary>
        /// Smoothly rotates the camera to look at the car.
        /// Uses Lerp for smooth rotation.
        /// </summary>
        private void LookAtCar()
        {
            Vector3 directionToLook = carTransform.position - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(directionToLook, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, lookSpeed * Time.deltaTime);
        }

        /// <summary>
        /// Optionally uses DOTween for smoother camera transitions.
        /// This function could be called when you want a more cinematic camera movement.
        /// </summary>
        public void SmoothFollowWithDOTween()
        {
            Vector3 targetPosition = carTransform.position + _initialCameraOffset;
            transform.DOMove(targetPosition, 1f / followSpeed).SetEase(Ease.InOutQuad);
        }

        public void SmoothLookAtWithDOTween()
        {
            Vector3 directionToLook = carTransform.position - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(directionToLook, Vector3.up);
            transform.DORotateQuaternion(targetRotation, 1f / lookSpeed).SetEase(Ease.InOutQuad);
        }
    }
}