using _Game.Scripts._helpers;
using UnityEngine;

namespace _Game.Scripts
{
    public class ParkingArea : MonoBehaviour
    {
        [SerializeField]
        private string _parkingAreaParticleKey = "parking_area";
        [SerializeField]
        private string _parkingAreaWithoutSuccessParticleKey = "parking_area_no_success";
        [SerializeField]
        private string _parkingAreaSuccessParkingParticleKey = "parking_area_success";

        private void Start()
        {
            GlobalBinder.singleton.CarParkingChecker.OnEnterParkingAreaWithoutSuccess += HandleEnterParkingAreaWithoutSuccess;
            GlobalBinder.singleton.CarParkingChecker.OnParkingSuccessful += HandleParkingSuccessful;
        }

        private void HandleEnterParkingAreaWithoutSuccess()
        {
            Debug.Log("Car entered the parking area but parking is not successful yet.");
            // Other logic
            GlobalBinder.singleton.ParticleManager.PlayParticleAtPoint(_parkingAreaWithoutSuccessParticleKey,
                transform.position, Quaternion.identity, transform);
        }

        private void HandleParkingSuccessful()
        {
            Debug.Log("Parking has been successfully completed!");
            // Other logic
            GlobalBinder.singleton.ParticleManager.PlayParticleAtPoint(_parkingAreaSuccessParkingParticleKey,
                transform.position, Quaternion.identity, transform);
        }
    }
}