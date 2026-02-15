# Tile Game

The Tile Game lets you place square tiles in a grid, collecting points by creating large areas of the same tile, similar to Dorfromantik or Carcassonne.


## Explanation

The game uses a board of square tiles, supporting directional snapping in 90 degree rotations around the y-axis of the board sockets.
Placed tiles taken from sockets are respawned with either a purely random selection of areas or from a number of predefined variants.
These areas are represented by an array of entries, consisting of a unique id (=array index), a name, a color for displaying their scores and a material assigning to a tile.
The scores are simply calculated using a flood fill in each direction that continues only if tiles are connected through adjacent specs of the same area type.

## Setup

### Tile Interactable

- Model a tile or use the existing model from the examples. Each area should have its own material! Tip: Use descriptive names as you need to map them later.
- Create a new GameObject with a "ExPresSXRGrabInteractable", "Rigidbody" and "TileVisuals" script. 
- Add the tile Model as child of the interactable and a Collision.
- In the TileVisuals script set "Renderer" to the one of the tile model.
- Set up the "Material Idxs" with the corresponding indices of the materials of your model. This is sadly required as the order of the materials can not be easily derived.
- Add a ScoreNumbers prefab (located under 'Minigames>Common' or create your own) as "Points Display Prefab" and adjust the timing and sizing for displaying the scores. For debugging use the three dots in the header of the "Tile Visuals" script an choose "Display Test Scores" while the game is running.
- Safe it as a prefab. Create prefab variants from this, if you want to control which tiles can be spawned. 

### Board Sockets

- Create a new "Board Socket" GameObject with a "TileSubmitSocket" script and a collision. Add visuals and a highlighter to your likings.
- Make sure to set "NumSteps" of the "TileSubmitSocket" to 4 (= 90 degree snaps).
- It is recommended to set "RequireFrontSideUp" to prevent unwanted rotations when placing tiles front side down.
- Safe it as a prefab.

### Tile Respawn Sockets

- Create a new "Tile Respawn Socket" GameObject with a "TileRespawnSocket" script and a collision.
- If you created prefab variants of your tile, add them to the "Prefab Variants" array.
- It is recommended to not set "Put Back Prefab" to your tiles prefab yet as this will result in the same initial tile. Set it when being instanced for your TileGame.
- Safe it as a prefab.

### Tile Game

Setup the overall game and helper utility.

- Create a new "Tile Game" GameObject with the "TileGame" script. Optionally but highly recommended, add the "TileGameBoardSetupUtility" and "TileGameDebugUtility" scripts.
- Create a new "Board Sockets" GameObject as child of the tile game, and set the reference in the "TileGame" script to the new GameObject.
- Create a set of areas, using new entries in the "Areas" array of the "TileGame". They should have a name, color for the scores and a material.
    - Make sure the Areas are set in the TileGame, RespawnSocket(-Prefabs) and Tile(-Prefabs). Use right-click for Copy&Pasting the "Areas" value.
- Using the "TileGameBoardSetupUtility" providing the "TileGame" and "BoardSocket" you can create and set up the board sockets with the specified sizing and elevation, by selecting "Setup Board" from the list appearing when opening the three dots in the header of the component.
    - If you decide to do it manually, add the board sockets, configure their "Board Pos" position and connect the "OnTileSubmitted" with "TileGame.AddTileFromBoardSubmission".
- Create a set of TileRespawnSockets and set the "Put Back Prefab" to the prefab of your tiles. This will spawn a new random tile. If you have configured tile variants, you can skip this step.
- Add references to these sockets "Tile Respawn Sockets" in your TileGame.
- Add a new Canvas GameObject with a Text GameObject, that has a "TotalScoreText" script attached to it. Connect the reference of the "Text", drop in the ScoreNumbers prefab and adjust the sizing.

## Tile Game Debug Utility

This is a small helper component for testing tile submission via the editor:
- Add Tile from Board Submission: Adds a with the configured Area IDs at the position, displaying the scores in the provided instance of TileVisuals.
- Add Tile Manually: Adds a tile without displaying the scores in a TileVisuals.
- Evaluate Board: Performs evaluates the board starting from "Insert Pos".