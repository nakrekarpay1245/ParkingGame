using _Game.Scripts._helpers.Audios;
using _Game.Scripts._helpers.Particles;
using _Game.Scripts._helpers.TimeManagement;
using _Game.Scripts.Car;
using _Game.Scripts.UI;
using UnityEngine;

namespace _Game.Scripts._helpers
{
    /// <summary>
    /// GlobalBinder centralizes access to core game systems and registers them with the ServiceLocator.
    /// </summary>
    public class GlobalBinder : MonoBehaviour
    {
        //[Header("Managers")]
        //[Tooltip("Handles audio functionalities like playing sounds and managing music.")]
        //[SerializeField] private AudioManager _audioManager;

        //[Tooltip("Manages particle effects used throughout the game.")]
        //[SerializeField] private ParticleManager _particleManager;

        //[Tooltip("Handles time management including countdowns, timers, and related functions.")]
        //[SerializeField] private TimeManager _timeManager;

        //[Header("Game Systems")]
        //[Tooltip("Manages vehicle functionalities including controls and physics.")]
        //[SerializeField] private CarController _carController;

        //[Tooltip("Handles damage and health functionalities for the vehicle.")]
        //[SerializeField] private CarDamageHandler _carDamageHandler;

        //[Tooltip("Handles UI functionalities, such as displaying vehicle speed.")]
        //[SerializeField] private UIManager _uiManager;

        //private void Awake()
        //{
        //    RegisterServices();
        //}

        ///// <summary>
        ///// Registers all the services in the ServiceLocator.
        ///// </summary>
        //private void RegisterServices()
        //{
        //    ServiceLocator.Register(_audioManager);
        //    ServiceLocator.Register(_particleManager);
        //    ServiceLocator.Register(_timeManager);
        //    ServiceLocator.Register(_carController);
        //    ServiceLocator.Register(_carDamageHandler);
        //    ServiceLocator.Register(_uiManager);
        //}
    }
}