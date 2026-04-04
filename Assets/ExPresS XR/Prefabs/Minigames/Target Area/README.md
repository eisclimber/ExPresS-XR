# Target Area

This game implements a generic 'quest'-pattern of touching areas with another area (multiple times).
This make context changes rather easy. One could easily have to light torches, perform a magic spell by moving a wand in some pattern, pet a pet, ...

## Sword Cleaning

Preserve an ancient sword by oiling it and padding it with a powdered cloth.

1. Grab the sword!
2. Use the oiled cloth (black stick) to wipe down both sides of the sword.
3. Pad the sword with the powdered cloth (green stick). You must put the little blue ball in and out of the highlighted areas.
4. Enjoy your restored sword!

P.S.: After restoring the sword one could make it so that the sword can slice through things. Not adding this as it requires external addons. If you are interested have a look at tutorial or ask me: https://www.youtube.com/watch?v=GQzW6ZJFQ94

### Setup

- Create new GameObjects consisting of the areas visuals, a Collision and the "TargetArea" script.
- Set the "Actions to Complete" value to how often the area must be touched (i.e. entered and exited) to complete. Negative values are interpreted as infinite actions.
- It is recommended adding a Color Switcher and connecting the TargetAreas "Completion Event" with the "ActivateAlternativeMaterial()" function to provide visual feedback.
- Place the target areas at the desired locations of your target. This can be a static object or in the case of the sword an interactable.
- Create an interactable to be your TargetAreaTrigger by creating a GrabInteractable and adding a new GameObject with a Trigger-Collision to it. Add the TargetAreaTrigger-Script to the Trigger-GameObject.
- Move and resize that Trigger-GameObject to the position in your interactable, with which you want to trigger the target areas (i.e. the "Hotspot").
- Add references to the TargetAreas that should be triggered by the TargetAreaTrigger, by dragging the into the "Targets" property of the triggerer. If you need to add many targets, lock the Inspector tab (lock-icon on the top right of the Inspector panel), select all areas from the inspector and drag & drop them into the header of the "Targets" property (where it says "Targets" and displays the size).
- To start, either enable the "Auto Start" property of the triggerer or call "SetupTargets()".


## Breakable Stone

Use the pickaxe to destroy object gradually either by changing the model, the texture or both.

### Setup

- Create a TargetArea as described above, but also add the "BreakableModel" script to it.
- Configure the "MaxDamage" of the model (i.e. how often it must be hit) and set the "ActionsToComplete" of the TargetArea to -1 (for simplicity).
- Connect the "OnActionPerformed" event of the TargetArea to the "IncreaseDamage" function of the "BreakableModel".
- Either (or do both):
    - Provide a List of DamageModels for each state damage value, including the "no damage" state. In case you'e checked "Deactiavte On Max Damage" you can omit the last model, so your list need to be either "MaxDamage" or "MaxDamage + 1" long. Also make sure to not use the same model instance twice, duplicate the model instead. If you want to change multiple objects, group them under a GameObject and provide it as reference.
    - Provide a renderer that utilizes a shader to reflect the damage on a material basis. You can create a new Material using the "Breakable Stone" Shader with your own damage texture texture or create a new shader completely from scratch. The damage will be passed as float between 0.0f and 1.0f to the material via the variable set as "Shader Variable Name". The value for the float will be linear but can be set to different values per damage by providing the as list via "Shader Variable Values". This includes the first and last stage, so you will need "MaxDamage + 1" entries.
- The triggerer can be set up in the same way as above. You can use the "AverageVelocity" to require hitting with a specific speed and/or direction, but the actual logic for the implementation is up to you.