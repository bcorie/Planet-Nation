
# Project Planet Nation
[Markdown Cheatsheet](https://github.com/adam-p/markdown-here/wiki/Markdown-Here-Cheatsheet)

### Student Info
-   Name: Corie Beale
-   Section: 01

## Simulation Design
A planet system where smaller objects revolve around larger objects. Objects include planets, spaceships, and asteroids. All smaller objects are randomly generated, while planets have a default layout on startup. The game space is not infinite. There will be a cloud of dust as a rounded border to block further camera movement. The player may look around the system and will not be restricted to the screen space.

### Controls
-   Add/remove a planet.
    -   Control: LMB
    -   Place a planet who's center is the mouse's position on mouse click.
    -   Menu will be available to change properties of a planet before placing it.
-   Camera movement
    -   Control: Move mouse to edge of screen
    -   Moves the camera past whatever screen size given to add more space to the world and more interactivity between agents.

## Asteroid
Asteroids are everywhere! They are common in the planet system and are affected by planets' gravities.

### Passive Gravitation
**Objective:** Gravitate toward planets.

#### Steering Behaviors
- Gravitates toward planets
   - Seeks nearest planet
   
#### State Transistions
- Agent is in this state by default.

### Abilities
- Destroys itself if it collides with a planet.

## Planets
Acts as a point of gravitation.

## Drill Ship
These ships mine resources from planets and takes them back home.

### Depart
**Objective:** Land on the planet nearby.

#### Steering Behaviors
- Gravitate towards the target planet
- Obstacles - Asteroids
- Seperation - Other Drill Ships

#### State Transitions
- In this state by default.

### Drill
**Objective:** Do not move. Very important process underway. Drills up material to fill the Drill Ship's tank on the planet's surface.

#### Steering Behaviors
- Stick to the surface of the target planet.

#### State Transitions
- Drill Ship contacts the target planet.

### Return
**Objective:** Escape the asteroid-filled planet system.

#### Steering Behaviors
- Escape the planet system.
- Obstacles - Asteroids
- Seperation - Other asteroids

#### State Transitions
- Drill Ship's tank is full.

## Sources
-   Art: Corie Beale

## Make it Your Own
- Art assets created by me (Corie Beale)
- UI and menus
- Total Agents: 2/2
	- Asteroid, Drill Ship
- Total States: 4
	- Asteroid: Passive Gravitation
	- Mine Ship: Depart, Drill, Return
