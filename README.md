# Parking Perfect! 🚗🎮

**Parking Perfect!** is an arcade-style car parking game built with **Unity Engine** for **WebGL** and **Android** platforms. The goal of the game is to park the car in the designated parking area without hitting any traffic cones. The game features intuitive controls, smooth animations, and a dynamic economy system. The project follows **SOLID** and **OOP** principles with a strong focus on performance and clean code.

## 🎥 Gameplay Video

[![Parking Perfect! Gameplay](https://img.youtube.com/vi/WfJcOrbDuKw/0.jpg)](https://www.youtube.com/watch?v=WfJcOrbDuKw)

Click the image to watch the gameplay video.

## 🎮 Gameplay Overview

In **Parking Perfect!**, players control a car using either keyboard or on-screen controls:

- **Web Controls**: `W`, `A`, `S`, `D` for movement and `Space` for the handbrake.
- **Mobile Controls**: On-screen buttons for forward, reverse, left, right, and handbrake.

## 🖼️ Screenshots

| Menu Scene        | Gameplay Scene     |
|-------------------|--------------------|
| ![Menu](Assets/Screenshots/menu_scene.png) | ![Gameplay](Assets/Screenshots/game_scene.png) |

### Objective:
- Park the car perfectly without hitting any traffic cones.
- Earn up to 3 stars based on your performance.
- Stars and parking success are rewarded with in-game currency.
- Damage to the car affects both the stars earned and the amount of money rewarded.
- Health reaches zero? The level fails, but you still earn a small consolation reward.

## 🌟 Key Features

- **Arcade Car Controls**: Realistic and responsive controls that are easy to pick up but challenging to master.
- **Smooth Camera Follow**: The camera dynamically follows the player's car, ensuring smooth and isometric transitions.
- **Dynamic Level Management**: Each level is loaded dynamically with obstacles and parking areas.
- **Damage & Health System**: Real-time health updates based on collisions with obstacles like traffic cones.
- **Car Selection & Economy**: Unlock new cars and manage your in-game currency through our gallery system.
- **2 Playable Cars & 10 Test Levels**: Start with two different cars, each with unique handling, and test your skills across ten challenging levels.

## 🛠️ Technologies Used

- **Unity Engine**: Game development and scene management.
- **C#**: Core programming language, following **SOLID** and **OOP** principles.
- **DOTween**: Smooth animations and transitions for UI elements and gameplay events.
- **Scriptable Objects**: Flexible and reusable game data management (e.g., car configurations, levels, etc.).
- **Singleton Pattern**: Efficient management of game managers and critical systems.
- **JSON Data Saving**: Game progress and player data are saved in JSON format via PlayerPrefs for persistence.

## 🌐 WebGL Demo

[Play the demo online](https://erkanyaprak.itch.io/parking-perfect) - Try **Parking Perfect!** directly in your browser.

## 📚 Architecture & Design

**Parking Perfect!** is designed using modular and scalable components, adhering to **SOLID** principles. The following are some of the core architectural patterns and code management approaches used:

- **Management-Based Architecture**: Managers are responsible for specific areas of functionality like level loading, UI updates, economy management, and player input.
- **Scriptable Object Pattern**: This pattern provides flexibility in managing game data such as car configurations, player progress, and game settings.
- **Interfaces and Abstract Classes**: The damage system is implemented through interfaces (`IDamageable`, `IDamager`) and abstract base classes (`AbstractDamageableBase`, `AbstractDamagerBase`) to ensure flexibility and reusability.

### Key Components:

- **CarController.cs**: Handles player input and car physics. Input is processed through the `InputHandler.cs` and `PlayerInputSO` scriptable object.
- **CarDamageHandler.cs**: Manages player health, damage, and death events, using `AbstractDamageableBase`.
- **CarParkingChecker.cs**: Verifies if the player successfully parks the car.
- **Obstacle.cs**: Implements collision logic that damages the player when they hit obstacles (e.g., traffic cones).
- **CustomButton.cs**: A custom button implementation using DOTween for smooth scale animations.
- **SaveManager.cs**: Manages game saving and loading, using JSON formatting to store data in PlayerPrefs.

## 📊 Game Flow

- **Menu Scene**: The player can start the game, change cars, access the settings (sound/music settings), or exit the game.
- **Game Scene**: Upon starting the game, the `LevelManager.cs` loads the selected level and manages the car, parking area, and obstacles.
- **Game Data**: The game's progress, including the list of unlocked levels, earned money, and purchased cars, is managed by the `GameData` scriptable object. All changes are saved via the `SaveManager.cs` system.

## 📦 Asset Credits

- **Car Controller**: [PROMETEO: Car Controller](https://assetstore.unity.com/packages/tools/physics/prometeo-car-controller-209444)
- **Car Model**: [ARCADE: FREE Racing Car from Mena](https://assetstore.unity.com/packages/3d/vehicles/land/arcade-free-racing-car-161085?srsltid=AfmBOooox9FEisp0149uLYhA5dXPn1doXpfyu6_bshLimV5c4zNrcMiI)
- **Traffic Cone Prop**: [Polypizza](https://poly.pizza/m/OlveRhO9Ha)

## Contact Information
- **Developer:** Erkan Yaprak
- **GitHub Profile:** [nakrekarpay1245](https://github.com/nakrekarpay1245)
- **Personal Website:** [erkanyaprak.w3spaces.com](https://erkanyaprak.w3spaces.com)
- **Old Clone Projects:** [All Games](https://erkanyaprak.w3spaces.com/allgames.html)
- **Upcoming Project Promotion Page:** *Hard Deliver* (Upcoming on Steam)
- **Email:** [rknyprk79@gmail.com](mailto:rknyprk79@gmail.com)
- **LinkedIn:** [Erkan Yaprak](https://www.linkedin.com/in/erkan-yaprak)

## 💻 Installation

To get the game up and running locally:

1. Clone the repository:
   ```bash
   git clone https://github.com/nakrekarpay1245/ParkingPerfect.git
2. Open the project in Unity (Version 2020 or later recommended).
3. Switch the build target to either WebGL or Android.
4. Play in the editor or build and deploy the game to your platform of choice.

Made with 💙 by Erkan Yaprak
