# Boxing

Implements a reactive boxing mechanic based on Target Areas.

## Setup

- Add a TargetAreaTriggerer(, a Rigidbody and a Collision configured as trigger) to both of the hands of the rig.
- Create GameObjects with BoxingTargetAreas-Components and collision to use as targets.
- Optionally create a TargetAreaRandomizer that will activate one of the target TargetAreas configured in random timed intervals.
- If you want animate the target areas, create an animation for the areas and and link the animator in the areas.
- Other prefabs like damage numbers can be spawned on hit, by providing them as DamageDisplayPrefab for the BoxingTargetAreas. 