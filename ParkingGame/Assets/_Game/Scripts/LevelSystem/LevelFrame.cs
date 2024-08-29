using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using _Game.Obstacles;
using _Game._helpers;
using _Game._helpers.Audios;
using _Game._helpers.Particles;

namespace _Game.LevelSystem
{
    /// <summary>
    /// The LevelFrame class places prefabs at each node and between child nodes at specified intervals.
    /// Prefabs are instantiated with a delay, scaled up from 0 to 1, and fall into place with Rigidbody physics.
    /// Animation and spawning settings are adjustable.
    /// </summary>
    public class LevelFrame : MonoBehaviour
    {
        [Header("Prefab Settings")]
        [Tooltip("The prefab to instantiate between child points and at the points themselves.")]
        [SerializeField] private Obstacle _prefab;

        [Header("Spacing Settings")]
        [Tooltip("The desired interval at which prefabs should be placed. This value will be adjusted to fit the distance.")]
        [Range(0.1f, 10f)]
        [SerializeField] private float _spacing = 2f;

        [Tooltip("Enable looping to connect the last node back to the first.")]
        [SerializeField] private bool _loop = false;

        [Header("Animation Settings")]
        [Tooltip("The duration of the prefab scale-up and movement animation.")]
        [Range(0f, 1f)]
        [SerializeField] private float _animationDuration = 0.5f;

        [Tooltip("The upward offset applied to prefabs before they fall into position.")]
        [Range(0f, 10f)]
        [SerializeField] private float _fallHeight = 5f;

        [Tooltip("The delay between the instantiation of each prefab.")]
        [Range(0f, 2f)]
        [SerializeField] private float _spawnInterval = 0.05f;

        [Tooltip("The easing type for the prefab scaling and movement animations.")]
        [SerializeField] private Ease _initAnimationEase = Ease.OutBack;
        [SerializeField] private Ease _disposeAnimationEase = Ease.InBack;

        [Tooltip("Scaling factor for the instantiated prefabs.")]
        [Range(0.1f, 3f)]
        [SerializeField] private float _scaleFactor = 1f;

        [Tooltip("Rate of change of the spawn delay. A positive value increases delay, a negative value decreases it.")]
        [Range(-0.9f, 0.9f)]
        [SerializeField] private float _delayChangeRate = 0f;

        [Header("Gizmos Settings")]
        [Tooltip("The color of the nodes' gizmos.")]
        [SerializeField] private Color _nodeGizmoColor = Color.blue;

        [Tooltip("The color of the obstacles' gizmos.")]
        [SerializeField] private Color _obstacleGizmoColor = Color.red;

        [Tooltip("The color of the lines between nodes and obstacles.")]
        [SerializeField] private Color _lineGizmoColor = Color.green;

        [Tooltip("The size of the gizmos representing the nodes.")]
        [SerializeField] private float _nodeGizmoSize = 0.2f;

        [Tooltip("The size of the gizmos representing the obstacles.")]
        [SerializeField] private float _obstacleGizmoSize = 0.1f;

        [Header("Effects")]
        [Header("Audio Effects")]
        [Tooltip("The sound play when spawn an obstacle.")]
        [SerializeField] private string _spawnSoundKey = "spawn_obstacle";

        private List<Transform> _nodes = new List<Transform>();
        private List<Vector3> _obstaclePositions = new List<Vector3>();
        private List<Obstacle> _instantiatedObstacles = new List<Obstacle>();

        private AudioManager _audioManager;

        public void Init()
        {
            InitializeNodes();
            CalculateObstaclePositions();

            // Access AudioManager through ServiceLocator
            _audioManager = ServiceLocator.Get<AudioManager>();

            StartCoroutine(PlacePrefabsWithDelay());
        }

        /// <summary>
        /// Collects all child transforms as nodes for the level frame.
        /// </summary>
        private void InitializeNodes()
        {
            _nodes.Clear();
            foreach (Transform child in transform)
            {
                _nodes.Add(child);
            }

            if (_nodes.Count < 2)
            {
                Debug.LogWarning("LevelFrame requires at least two child nodes.");
            }
        }

        /// <summary>
        /// Calculates the positions of obstacles based on node positions and spacing.
        /// </summary>
        private void CalculateObstaclePositions()
        {
            _obstaclePositions.Clear();
            int nodeCount = _nodes.Count;

            for (int i = 0; i < (_loop ? nodeCount : nodeCount - 1); i++)
            {
                Transform startPoint = _nodes[i];
                Transform endPoint = _nodes[(i + 1) % nodeCount];

                float distance = Vector3.Distance(startPoint.position, endPoint.position);
                int prefabCount = Mathf.FloorToInt(distance / _spacing);
                float adjustedSpacing = distance / (prefabCount + 1);
                Vector3 direction = (endPoint.position - startPoint.position).normalized;

                for (int j = 1; j <= prefabCount; j++)
                {
                    Vector3 position = startPoint.position + direction * (j * adjustedSpacing);
                    _obstaclePositions.Add(position);
                }
            }
        }

        /// <summary>
        /// Instantiates prefabs at each node and obstacle positions with animation and delay.
        /// </summary>
        private IEnumerator PlacePrefabsWithDelay()
        {
            List<Transform> nodesCopy = new List<Transform>(_nodes);
            List<Vector3> obstaclePositionsCopy = new List<Vector3>(_obstaclePositions);

            foreach (Transform node in nodesCopy)
            {
                yield return InstantiateAndAnimatePrefab(node.position);
                // Adjust spawn delay with rate change
                _spawnInterval = Mathf.Max(0f, _spawnInterval * (1 + _delayChangeRate));
                yield return new WaitForSeconds(_spawnInterval);
            }

            foreach (Vector3 obstaclePosition in obstaclePositionsCopy)
            {
                yield return InstantiateAndAnimatePrefab(obstaclePosition);
                // Adjust spawn delay with rate change
                _spawnInterval = Mathf.Max(0f, _spawnInterval * (1 + _delayChangeRate));
                yield return new WaitForSeconds(_spawnInterval);
            }
        }

        /// <summary>
        /// Instantiates the prefab at the specified position with animation.
        /// </summary>
        private IEnumerator InstantiateAndAnimatePrefab(Vector3 position)
        {
            // Calculate start position for prefab to animate from
            Vector3 startPosition = position + Vector3.up * _fallHeight;
            Obstacle instance = Instantiate(_prefab, startPosition, Quaternion.identity, transform);

            // Apply scale factor to the prefab
            instance.transform.localScale = Vector3.zero * _scaleFactor;

            // Add instance to the list of instantiated obstacles
            _instantiatedObstacles.Add(instance);

            // Animate scaling and moving using DOTween
            instance.transform
                .DOScale(Vector3.one * _scaleFactor, _animationDuration)
                .SetEase(_initAnimationEase);

            instance.transform
                .DOMove(position, _animationDuration)
                .SetEase(_initAnimationEase);

            _audioManager.PlaySound(_spawnSoundKey);

            // Wait for animation to complete
            yield return new WaitForSeconds(_spawnInterval);

            // Enable physics after animation
            Rigidbody rb = instance.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
            }
        }

        /// <summary>
        /// Moves all instantiated obstacles to Vector3.zero with an animation.
        /// </summary>
        public void Dispose()
        {
            StartCoroutine(MoveObstaclesToZero());
        }

        /// <summary>
        /// Coroutine to move obstacles to Vector3.zero with animation.
        /// </summary>
        private IEnumerator MoveObstaclesToZero()
        {
            Sequence sequence = DOTween.Sequence();

            foreach (Obstacle obstacle in _instantiatedObstacles)
            {
                Vector3 initialPosition = obstacle.transform.position;

                // Create the tween to move and scale the obstacle
                Tween moveAndScaleTween = obstacle.transform
                    .DOScale(Vector3.zero, _animationDuration)
                    .SetEase(_disposeAnimationEase)
                    .OnComplete(() => obstacle.gameObject.SetActive(false));

                // Append the tween to the sequence
                sequence.Append(moveAndScaleTween);

                // Optionally add a delay between obstacle movements
                sequence.AppendInterval(_spawnInterval);

                yield return new WaitForSeconds(_spawnInterval);
            }

            // Play the sequence after all tweens have been appended
            sequence.Play();

            // Clear the list after disposing all obstacles
            _instantiatedObstacles.Clear();
        }

        /// <summary>
        /// Draws gizmos for nodes, obstacles, and connections between them before the game starts.
        /// </summary>
        private void OnDrawGizmos()
        {
            InitializeNodes();
            CalculateObstaclePositions();

            Gizmos.color = _nodeGizmoColor;
            foreach (Transform node in _nodes)
            {
                Gizmos.DrawSphere(node.position, _nodeGizmoSize);
            }

            if (_nodes.Count < 2) return;

            Gizmos.color = _lineGizmoColor;
            for (int i = 0; i < _nodes.Count; i++)
            {
                Transform startPoint = _nodes[i];
                Transform endPoint = _nodes[(i + 1) % _nodes.Count];
                Gizmos.DrawLine(startPoint.position, endPoint.position);
            }

            Gizmos.color = _obstacleGizmoColor;
            foreach (Vector3 obstaclePosition in _obstaclePositions)
            {
                Gizmos.DrawSphere(obstaclePosition, _obstacleGizmoSize);
            }
        }
    }
}