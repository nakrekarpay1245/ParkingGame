using _Game.Car;
using _Game.LevelSystem;
using _Game.Save;
using System.Collections.Generic;
using UnityEngine;

namespace _Game.Data
{
    [CreateAssetMenu(fileName = "GameData", menuName = "Data/GameData")]
    public class GameData : ScriptableObject
    {
        [Header("Game Configuration")]
        [Tooltip("List of levels in the game.")]
        [SerializeField]
        private List<Level> _levelList;
        private int _currentLevelIndex;

        public int CurrentLevelIndex
        {
            get
            {
                _currentLevelIndex = SaveManager.LoadLevelIndex();
                return _currentLevelIndex % _levelList.Count;
            }
            set
            {
                _currentLevelIndex = value;
                SaveManager.SaveLevelIndex(_currentLevelIndex);
            }
        }

        public Level CurrentLevel => _levelList[CurrentLevelIndex];

        [Header("Economy Configuration")]
        [Tooltip("The player's current amount of coins.")]
        [SerializeField]
        private int _coins;

        public int Coins
        {
            get
            {
                _coins = SaveManager.LoadCoins();
                return _coins;
            }
            set
            {
                _coins = value;
                SaveManager.SaveCoins(_coins);
            }
        }

        [Header("Car System")]
        [Tooltip("List of all available cars in the game.")]
        [SerializeField]
        private List<Car> _carList;
        public List<Car> CarList { get => _carList; set => _carList = value; }

        [Tooltip("Index of the currently selected car.")]
        [SerializeField]
        private int _selectedCarIndex;

        public int SelectedCarIndex
        {
            get
            {
                _selectedCarIndex = SaveManager.LoadSelectedCarIndex();
                return _selectedCarIndex;
            }
            set
            {
                _selectedCarIndex = value;
                SaveManager.SaveSelectedCarIndex(_selectedCarIndex);
            }
        }

        public CarController SelectedCarPrefab => _carList[SelectedCarIndex].CarPrefab;
        public GameObject SelectedCarModelPrefab => _carList[SelectedCarIndex].CarModelPrefab;
        public CarConfigSO SelectedCarConfig => _carList[SelectedCarIndex].CarConfig;

        public bool BuyCar(int carIndex)
        {
            if (carIndex < 0 || carIndex >= _carList.Count)
                return false;

            Car carToBuy = _carList[carIndex];

            if (!carToBuy.IsPurchased)
            {
                carToBuy.IsPurchased = true;
                SaveManager.SaveCarPurchase(carIndex, carToBuy.IsPurchased);
                return true;
            }

            return false;
        }

        public void SelectCar(int carIndex)
        {
            if (carIndex >= 0 && carIndex < _carList.Count && _carList[carIndex].IsPurchased)
            {
                SelectedCarIndex = carIndex;
            }
        }
    }

    [System.Serializable]
    public class Car
    {
        [Header("Car Information")]
        [Tooltip("Name of the car.")]
        [SerializeField]
        private string _name;

        [Tooltip("The price of the car in coins.")]
        [SerializeField]
        private int _price;

        [Tooltip("Indicates whether the car has been purchased.")]
        [SerializeField]
        private bool _isPurchased;

        [Header("Car Prefabs")]
        [Tooltip("The main prefab representing the car.")]
        [SerializeField]
        private CarController _carPrefab;

        [Tooltip("The model prefab used for displaying the car in menus or previews.")]
        [SerializeField]
        private GameObject _carModelPrefab;

        public string Name => _name;
        public int Price => _price;
        public bool IsPurchased
        {
            get => _isPurchased;
            set => _isPurchased = value;
        }

        public CarController CarPrefab => _carPrefab;
        public CarConfigSO CarConfig => CarPrefab.CarConfig;
        public GameObject CarModelPrefab => _carModelPrefab;
    }
}