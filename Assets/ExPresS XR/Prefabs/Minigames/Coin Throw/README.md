# Coin Throw (Delta Game 2.0)

Over 2000 years ago people loved games at least as much as we did.
Lacking technical achievements like computers, smartphones or VR headsets, children played the Delta-Game.
Ironically this *roman* game was named after the *greek* uppercase letter Δ ("Delta"). The romans drew a triangle on the ground and split it into 10 sections, in which they threw nuts or stones. Landing a hit inside a section awarded the player between 1 point for the closest and up to 10 points for the farthest section.

As a homage to the game we decided that it is time for a version 2.0!
Your goal is simple: Try to hit the moving bowls on the ground with the denarius to your right. Further bowls will get you more points.

To start stand between the pillars and press the start button.

P.S: Feel free to practice throwing the coin first.

## Setup

- Create an interactable with a CoinReset-Compoent. This will reset the coins position to the initial position once hitting the target.
- Create a CoinResetArea to the floor (and all other areas you might need it) to reset the coins when missing the targets.
- Create targets with a Collision and ThrowTarget-Component.
- For moving targets, add a PingPongLinearTween. It will move back and forward from the start position with the provided delta, speed and repetitions.
- To start, enable detectHits for the targets, start the tweens and allow interaction with the interactable (optional).