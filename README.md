# Retro Hackathon - Training an AI to play Super Mario Bros
For this hackathon we tried to recreate the first level and as many game aspects of Super Mario Bros as we could completely from scratch given the time we had. This [website](https://supermarioemulator.com/supermario.php) was used as a reference during our level and game design along with sound effects from [here](https://themushroomkingdom.net/media/smb/wav) to try make our game as old school as possible! We built and trained an Agent (using the ML-Agents library) to play the level with reinforcment learning using the Proximal Policy Optimization (PPO) algorithm. More information on how the Agent works and was trained, as well as general information about the game we developed is provided below.

## How to run the game
To run the game, go to the 'Unity Builds/Windows/' folder in the repo and run the 'Retro Hackathon.exe' file. You will then be presented with a main menu giving you the option to either play the game yourself or watch the AI we trained play the game. Since we trained the AI at different stages of the game's development, we have included two options for watching the AI play, one with enemies (goombas) and one without them. Given that the former had less time to train since it was trained at a later stage in the game's development, and also has a more complex environment since it includes moving enemies to avoid, it performs slightly worse or at least less consistently than the version without enemies. **Please note that this build is only available for windows.**

<p align="center">
  <img src="https://github.com/valerija-h/Retro-Hackathon/blob/main/Images/Main%20Menu.jpg" width="50%" height="50%">
</p>

## Implementing the game and AI
The game and Agent were developed using Unity version 2019.4.3f1. All scripts, scenes, animations, prefabs, and other components were made from scratch, however, the sprites and audio used in the game were taken from online (links will be provided to our main sources used below). The scripts used to create the Agent and train it were also built from scratch. 

### How does the Agent work?
The agent takes observations from it's environment in the form of Raycasts and certain variables (for example, one observation would be whether he is Big Mario or not from grabbing a mushroom). We then encourage certain actions or decisions the Agent makes during training by giving it rewards (positive reinforcement). For instance, if the Mario AI collects a mushroom then we reward it positively. Bigger rewards are given for end objectives such as reaching the end of level flag. An environment with multiple instances of the game is used for training so as to acquire more experiences in the same training window and thus hopefully learn quicker.

<p align="center">
  <img src="https://github.com/valerija-h/Retro-Hackathon/blob/main/Images/Multiple%20Environments.png" width="400px" height="300px">
  <img src="https://github.com/valerija-h/Retro-Hackathon/blob/main/Images/Raycasts.png" width="400px" height="300px">
</p>

The below gifs shows how the Agent (trained on an environment with Goombas) played at the start of training (left) versus how it played at the end of the training (right).

<p align="center">
  <img src="https://github.com/valerija-h/Retro-Hackathon/blob/main/Images/BeforeTraining.gif" width="400px" height="300px">
  <img src="https://github.com/valerija-h/Retro-Hackathon/blob/main/Images/AfterTraining.gif" width="400px" height="300px">
</p>

The below graphs show the mean reward per episode and mean episode length in steps on the y axes versus the number of steps trained for on the x axes when training the AI for the environment without goombas. As can be seen, the mean reward per episode increased significantly over time showing that the performance of the agent truly improved over the course of the training.

<p align="center">
  <img src="https://github.com/valerija-h/Retro-Hackathon/blob/main/Images/Tensorboard graphs.jpg" width="50%" height="50%">
</p>

### What are the limitations of your game?
Due to time restraints, we weren't able to implement all aspects of the game that we wanted. We did not have time to implement underground levels, turtle enemies, invincibility, "Fire" Mario and multiple levels. We were however able to implement full functionalities of mystery blocks, brick blocks, goombas and basic game mechanics (such as dying, reaching the end, basic animations, keeping track of score, coins and time and so on..), although there might still be some occasional bugs which could be fixed given a bit more time.

### What future improvements would you implement given more time?
Given more time on this project, we would train our AI for longer and test a wider variety of hyperparameters in order to achieve the best performance possible. Moreover, we would like to implement the full features of the original game which we didn't have time to implement during this hackathon as well as the rest of the levels. Finally, we would attempt to fix any occasional bugs and work to smoothen the general gameplay experience.

