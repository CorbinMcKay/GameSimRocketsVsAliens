Project for my Game Simulation class, using C# and XNA framework.

https://github.com/CorbinMcKay/GameSimRocketsVsAliens/assets/133295104/5a4c37f5-9810-4a94-b95b-93d104b539b9

This class was my introduction to game development, using a programming language I had never touched. 
Everyones first game is not good to say the least, and this is no exception. But its my game, and I loved every part of making
it, learning fundamental concepts, all while learning C# on the fly coming from python as my first language. I learned about sprites,
screen and world coordinates, direction, speed, bouncing off world edge, collisions, movement logic, animations, and so on. 

For my simulation project, I chose to make a zombies game. Instead of humans and zombies, did rocketships and ufos (mainly because I could draw these easier and wanted to do a space theme).
--When a ufo collides with a rocketship, the rocketship turns into an alien.
--Asteriods and blackholes are placed along the game. Collision with a black hole destroys the sprite. If a spite collides with an asteroid, they deflect away (Dear past Corbin... what was this logic). 
--Motion has time based logic implemented, and bouncing logic for screen edge and asteriods. 
--Rockets have logic implemented with a panic distance to run from ufos, collision avoidance with neighbors, and flock with nearby rockets.
--Aliens have a chase distance, where movement logic is changed to attempt to follow nearby rockets.
--Powerup bonuses (gold stars) are loaded into the game. 

Powerup bonus: 
	-Four powerups are loaded into the game, keeping fixed positions
	-If a Rocket or Alien collide with the bonus, they enter their bonus state. The bonus itself is returned dead and removed from the game. 
	-Created textures for Rockets and Aliens in the bonus state. 
	-A Rocket in the bonus state is immune to normal aliens, recieves a slight upgrade in speed, and tracks down the average position of aliens.
	-A Rocket in the bonus state kills an alien in the normal state. 
	-An Alien in the bonus state recieves a big upgrade in speed, preventing normal rockets from outrunning them
	-If a Rocket and Alien collide while both being in their bonus states, both creatures return to their normal state and deflect 
	   in opposite directions.
	-If zero bonuses remain, a single bonus is spawned randomly in the game. 
	-Rockets and Aliens will travel straight at the bonus if within a specified range.

Sound Effects / Game song:
	-Mutation sound effect is played whenever an infection occurs
	-Bonus sound effect is played when a creature obtains a bonus
	-An explosion sound effect is played when a creature collides with a black hole, or when a Rocket in the bonus state collides with a normal zombie
	-A game song is played on a loop while the game is active. 
	-Once the Rockets or Aliens win, the song is paused and a victory sound effect is played
	-Functions were created to control volume of each soundeffect/song

Animation: 
	-Created animation spritesheet
	-Whenever a creature dies, an explosion animation is played. 

Keyboard Inputs: 
	-Holf F to fastforward simulation (3x Speed)
	-Press D to pause simulation
	-Press S to resume simulation

Victory:
	-Once there are zero remaining Rockets or Aliens, the victory text is written to the screen
	-Victory text displays the winner, along with the total time survived in seconds
	-Once there is a victor, the simulation is stopped and all creatures no longer move
