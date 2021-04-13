# Unity Parable
This is our project submission for the Introduction to Unity course in 2021.<br>
You can find the project in two relevant branches.

<h3>Parable code</h3>
The first branch "parable-code" contains our adaptation of BoulderDash. In this game you have to collect all 12 gems on the map and avoid enemies to get a secret code.<br> It was planned that you would need this code to unlock the 3d levels of the game - kind of progressing from 2d to 3d.

<br><br>![Boulderminer example gif](https://github.com/michael-ra/unity-parable/blob/main/boulderminer.gif)<br><br>

<h3>Parable chaser</h3>

We sadly overestimated the size of the project and couldn't finish our second part of the project - a 3d styled game where you have to utilize movements to overcome objects and get away from a chaser. 

<br><br>![Movement example gif](https://github.com/michael-ra/unity-parable/blob/main/movement-course.gif)<br><br>

This project (located in the branch "parable-chaser") was created for the current DeepRL in tandem with the Intro to Unity course. The chaser is a DeepRL agent implemented with [Unity's Machine Learning Agents Toolkit](https://github.com/Unity-Technologies/ml-agents).
<br> The branch "parable-movement" has all the movement and a basic level that can demonstrate the current state of the player-movement. And on the "parable-chaser" branch you can already play against a trained agent (scene final)!

<br><br>![Chaser example gif](https://github.com/michael-ra/unity-parable/blob/main/parable-chaser.gif)<br><br>

The animations and movement were more complex than we throught (animation timing, raycasts, checks, edge-cases and more raycasts) so this part of the project is not finished yet but we will strive to complete it towards the deadline of the DeepRL course and onwards. :)
<br>
<br><br>

<h2>2d controls / BoulderDash</h2>
Arrow keys to move

<h2>3d controls / runner</h2>
While BoulderDash should be self-explaining, it might not be the case for the 3d-runner game. <br> Here you currently can: <br>
W to walk <br>
W+Shift to run <br>
Space to jump (only when not running or walking) <br>
Space in front of a climbable wall will grab onto the wall <br>
Space while grabbed onto a wall will pull you ontop the wall <br>
(more to be implemented) <br>
<br>

aaand most important: Theres also a hidden Macarena Dance animation!
