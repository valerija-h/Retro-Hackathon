# Retro Hackathon - Training an AI to play Super Mario Bros
For this hackathon we tried to recreate the first level and as many game aspects of Super Mario Bros as we could completely from scratch given the time we had. The following link (https://supermarioemulator.com/supermario.php) was used as a reference during our level and game design to try make our game as old school as possible! We built and trained an Agent (using the ML-Agents library) to play the level with reinforcment learning using the PPO algorithm. More information on how the Agent works and was trained, as well as general information about the game we developped is provided below.

## Implementing the game and AI
The game and Agent were developed using Unity version 2019.4.3f1. All scripts, scenes, animations, prefabs, and other components were made from scratch, however, the sprites and audio used in the game were taken from online (links will be provided to our main sources used below). The scripts used to create the Agent and train it were also built from scratch. 

### How does the Agent work?
The agent takes observations from it's environment in the form of Raycasts and certain variables (for example, one observation would be whether he is Big Mario or not from grabbing a mushroom). We then encourage certain actions or decisions the Agent makes during training by giving it rewards (positive reinforcement). For instance, if the Mario AI collects a mushroom then we reward it positively. Bigger rewards are given for end objectives such as reaching the end of level flag.

### What are the limitations of your game?
Due to time restraints, we weren't able to implement all aspects of the game that we wanted. We did not have time to implement underground levels, turtle enemies, invincibility, "Fire" Mario and multiple levels. We were however able to implement ful functionalities of mystery blocks, brick blocks, goombas and basic game mechanics (such as dying, reaching the end, basic animations, keeping track of score, coins and time and so on..)


