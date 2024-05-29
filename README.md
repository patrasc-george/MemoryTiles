# MemoryTiles

The MemoryTiles application is a desktop game built using Windows Presentation Foundation (WPF) that offers a memory tile matching experience. The application supports multiple user profiles, allowing each user to save their game progress and track their performance.

Upon starting the application, users are greeted with the Main Window, where they can select their profile from a list, create new profiles, delete existing ones, or navigate through their profiles. The profiles include details such as the user’s name, avatar, games played, and progress. The application ensures profiles are saved and loaded correctly from text files, maintaining the user's game state across sessions.

When a user chooses to start a new game, the New Game Window allows them to set the dimensions of the game board, ensuring the chosen dimensions are suitable for creating pairs of tiles. The game itself is managed within the Game Window, which displays the game board, manages the timer, and handles user interactions for flipping tiles to find matching pairs. The game can be paused and saved, allowing users to resume from where they left off.

In addition to the core game functionality, the application includes a Credits Window to display application credits, a Statistics Window to show user statistics, and a Play Window that provides options to start a new game, open an existing saved game, view statistics, or see credits. Each of these windows is designed to be user-friendly, providing a seamless experience.
