# Excavation

Try your hand at archaeology by carefully excavating precious artifacts.
Use your brush to reveal what lies beneath a layer of dust and dirt.

## Explanation

Be warned it is about to get technical! We are using a few cool tricks to implement this feature:
- **UV Collisions**: If you are familiar with 3D modelling and texturing, you'll have come across UV maps. They are used to map a 2D texture to faces/vertices of a model using a 2D vector where each axis is between 0.0f and 1.0f.  
A neat feature is that Unity provides these UV coordinates when calculating Ray- or Spherecast collisions. That means if we UV unwrap our excavation plane so match the full area between (0,0) and (1,1), we can use a Spherecast to detect where our brush is hitting it.  
- **Shader Drawing**: We are using a shader material to programmatically draw a so called "SplatMap" texture. It represents the ares the brush has touch and thus should be creased.  
This is achieved by accessing the current splat map in the GPU. Using a special draw material it is rendered on a new texture, adding another dot with the size, location and color channel of the brush hitting the plane. The result is then automatically set as "SplatMap" texture of the "Excavation" shader material where it defines the height and blend factor of the excavated surface. See `ExcavationArea.ExcavateAt()` for reference.
- **Mipmaps**: We use Mipmaps for determining which areas have been excavated and to what extend. Mipmaps are variants of a smaller textures generated automatically with sizes of power of two (1x1, 2x2, 4x4, 8x8, ...). This downsampling results in each pixel of a mipmap containing the average of the corresponding area of the original image.  
This makes checking the overall color of a texture as simple as checking the single pixel in the 1x1 mipmap, rather than having to iterate over the full image. So if we want to check if 80% has been excavated, we can check if the overall red color of the SplatMap is over 0.8 red (assuming we're using the red channel).  
The same principle applies for checking the completion of individual sections of the area. We're simply using a larger mipmap (matching our "Granularity").
- **CPU and GPU Synchronization**: The manipulation of the textures is performed on the GPI as they are used as textures for materials, which are also processed on the GPU. The problem is that the game logic and thus the evaluation of completion is running as a script on the CPU. While both GPU and CPU might be having a "reference" to the same texture, the changes made to the GPU are not "written back" to the actual texture. Without this the CPU will see no changes at all.  
In order to write the changes back, both GPU and CPU need to sync up as they are running at vastly different speeds. As you might have guessed, synching up means waiting, plummeting the framerate of the application if done in each Update()-function.  
As there is no way around exchanging the textures in this way, the solution is still rather simple: Just do if while something is happening and do it slowly.


## Setup

### Excavation Game

This describes the general setup of the main game logic.

- Create a model for your excavation area and drop it into your scene.
    - It must contain a plane to be excavated - rectangular or square is recommended. Technically any model should work, both it makes it harder to set up and debug.
    - Subdivide (and triangulate) the plane matching your desired "resolution" for excavating. Otherwise the model can not be deformed and it will stay flat.
    - UV unwrap the plane separately to expand between 0.0f and 1.0f (i.e. cover the whole UV texture area).
- Create a new GameObject for your Excavation Game, adding both the "ExcavationGame" script.
- Drop the reference to the excavation plane into the "Area" property of the ExcavationGame, adjusting the "Grid Gizmo Draw Scale" and "Grid Transform" to align the gizmos with the area.
- Choose a granularity for your grid. Setting it to "0" evaluate the whole area. For evaluating individual subsections choose a granularity to **roughly** cover these sections. Keep the granularity low as it will have a performance impact of your game (and will be a pain to set up).
- Set up **at least one** zone (see below). This defines how the game can be completed.
- Add a "Timer" script to the Excavation Game GameObject:
    - Set "WaitTime" to an acceptable high amount (0.5-1 second is recommended).
    - Set "AutoStart" to "true", "OneShot" to "false". In case you want to not start it automatically, adjust these values.
    - Call the "ExcavationGame.StartCompletionCheck" when the "OnTimeout" event is fired.
- Create a new Material for the excavation plane. Use the "Excavation" Shader or a custom one.
    - Provide a "MainTex" for the untouched surface and "TrailTex" for the excavated surface.
    - Set the Height Multiplier to increase the difference in height between the untouched and excavated surface.
    - The "TrailBlendingClamp" allows you to limit the blending from the "MainTex" to the "TrailTex". A value of 1 will completely show "TrailTex", while 0.6 will result in a blend of 60% "TrailTex" and 40% "MainTex" when being fully excavated.
    - Configure the "SplatBlur" to smooth out the visual and height transition between untouched and excavated surface.
    - The "SplatMap" and "TrailColor" will be populated automatically, so leave it empty.
- Assign the material to the plane to be excavated.


### Excavation Brush

The setup for the brush to excavate with.

- Create a GrabInteractable representing your brush, adding the "ExcavationBrush" script.
- Configure the brush size and draw strength, meaning the size painted on the texture.
- Choose an opacity for painting. This does not reflect the maximum opacity of the brush but rather how fast it will reach full opacity.
- Configure the min and max draw distance and the sphere cast radius of the brush. The later does NOT reflect the size drawn. It is just for calculating collisions.
- The draw channel can be configured to draw in different layers, but this requires a more complex shader. This this technically enables you to have multiple different tools, affecting different types of materials and layers.

### Excavation Zones

Allows specifying regions of the excavation area, that can be evaluated individually and checked for completion. Excavating all zones completes the game.

- Choose a location you want to excavate or place your artifact:
    - For Interactables, create them as ExPresSXRGrabInteractables, set "AllowGrab" to "false" and instantiate them using a PutbackSocket.
- In the ExcavationGame, add another entry to the "Zones" array and give it a "Description".
- Choose a channel and how much percent it should be excavated (i.e. average color in that zone).
- Use the grid of checkboxes to define which tiles belong to the zone. They correspond to the grid drawn in the scene, with the bottom right representing (0,0).
- If you've added an ExPresSXRGrabInteractables use the "OnZoneCompleted" event to set its "AllowGrab" to "true".