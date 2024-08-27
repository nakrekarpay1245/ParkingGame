using _Game.Scripts._helpers.Audios;
using _Game.Scripts._helpers.Particles;
using _Game.Scripts._helpers.TimeManagement;
using _Game.Scripts.Car;
using _Game.Scripts.UI;
using UnityEngine;

namespace _Game.Scripts._helpers
{
    /// <summary>
    /// Acts as a central hub for accessing various managers in the game.
    /// Inherits from MonoSingleton to ensure a single instance across the game.
    /// </summary>
    public class GlobalBinder : MonoSingleton<GlobalBinder>
    {
        [Header("Managers")]
        [Tooltip("Handles audio functionalities like playing sounds and managing music.")]
        [SerializeField] private AudioManager _audioManager;

        [Tooltip("Manages particle effects used throughout the game.")]
        [SerializeField] private ParticleManager _particleManager;

        [Tooltip("Handles time management including countdowns, timers, and related functions.")]
        [SerializeField] private TimeManager _timeManager;

        [Header("Game Systems")]
        [Tooltip("Manages vehicle functionalities including controls and physics.")]
        [SerializeField] private CarController _carController;

        [Tooltip("Handles damage and health functionalities for the vehicle.")]
        [SerializeField] private CarDamageHandler _carDamageHandler;

        [Tooltip("Handles UI functionalities, such as displaying vehicle speed.")]
        [SerializeField] private UIManager _uiManager;

        /// <summary>
        /// Provides public access to the AudioManager instance.
        /// </summary>
        public AudioManager AudioManager => _audioManager;

        /// <summary>
        /// Provides public access to the ParticleManager instance.
        /// </summary>
        public ParticleManager ParticleManager => _particleManager;

        /// <summary>
        /// Provides public access to the TimeManager instance.
        /// </summary>
        public TimeManager TimeManager => _timeManager;

        /// <summary>
        /// Provides public access to the CarController instance.
        /// </summary>
        public CarController CarController => _carController;

        /// <summary>
        /// Provides public access to the CarDamageHandler instance.
        /// </summary>
        public CarDamageHandler CarDamageHandler => _carDamageHandler;

        /// <summary>
        /// Provides public access to the UIManager instance.
        /// </summary>
        public UIManager UIManager => _uiManager;
    }
}