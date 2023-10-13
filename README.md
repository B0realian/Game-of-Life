# Game-of-Life
 Game of Life Assignment 2023

Our assignment was to finish a basic version of Conway's Game of Life, with extra points for any flair we wished to add. This version features quite a lot of cells and so
doesn't run very quickly. New cells are bright green but turn blue within 20 generations. They undergo another colour change to red starting at 100 generations and at the
grand old age of 1000 they start to turn yellow. When cells die, they fade to black.

This makes it easier to track what cells survive for longer periods of time but an additional effect of the fading dead cells is that it gives a sense of momentum to clusters
where cells are created and die.

I also added a pause-screen with the info written above plus what controls are available in game. The game starts paused. This was partly to avoid having to create a UI in
Unity which I thought would be overkill for what it is.
