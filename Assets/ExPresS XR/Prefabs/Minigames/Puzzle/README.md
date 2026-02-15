# Puzzle

A Game Framework for a puzzle game using sockets and interactables.

# Preparation

- Create a "board" model/GameObject. It should have a transform (empty GameObject) for the position of each puzzle piece. If you have a model with all puzzle pieces placed at the correct location, you can simply remove all components.
- Create puzzle pieces. Ideally these should have their origin at their geometrical center and be placed at (0, 0, 0). This makes snapping easier and ensures correct Rigidbody behavior.  

A good blender workflow would be creating the board model so that the pieces are placed correctly in the board.  
Then adjust the origin of the pieces to their geometry (In Object Mode: Right click with all pieces selected > Set Origin > To Geometry). Next for each piece add an Empty (In Object Mode: Shift + A > Empty > Plain Axes) and snap it to it by selecting the empty first and the piece second, pressing "Shift + S" and "Selection to Active".  
As a precaution, duplicate the pieces without moving ("Shift + D", then "Escape") and disable them, to have a backup. Next select the non-disabled pieces and snap them to the 3D cursor (placed at the world origin) via "Shift + S" and "Selection to 3D Cursor".  
Finally export the board and all the Empties as one model, as well as all the pieces individually.

## Setup

- Add the SocketsPuzzleLogic to the base/board GameObject of your puzzle. Adjust the board size to the dimensions of your board. If it is not in a grid layout, just use the number of your pieces in one dimension and 1 in the other.
- To all positions where a puzzle piece should be placed, add an ObjectSubmitSocketInteractor as well as a collision shape. For snapping valid pieces into place, enable "Hover Socket Snapping" and disable "Show Interactable Hover Meshes".
- Add all sockets to the list "BoardSockets" of the SocketsPuzzleLogic.
- Create an empty GameObject and add an ExPresSXRGrabInteractable to it, as well as a Rigidbody. It is recommended saving it as a prefab.
- For each puzzle piece, add piece model as as a child of the interactable prefab. Add a Collision and save it as prefab variant.
- Add all puzzle piece **Prefabs** to the list "Piece Prefabs" of the SocketsPuzzleLogic.
- Add an empty GameObject and add couple of PutBackSocketInteractors as its children. There can be less than number of pieces of the puzzle. Add a PuzzleSpawner to the emtpy gameObject and add all PutBackSockets to the "RespawnSockets". Set the "Puzzle" to your SocketsPuzzleLogic. Depending if you want it, enable "spawnOnStart".
- To start you now only have to spawn new pieces, by calling "RespawnSockets.RerollSocketsTiles" if not done automatically on start.

P.S. "_defaultPiece", "_scoreDisplayPrefab", "_pointsDisplayUpOffset", "_pointsDisplayScale" and "_completionDelay" of the SocketsPuzzleLogic are not important in this case.


#### TODO