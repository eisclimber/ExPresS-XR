# ExPresS XR

<img align="left" width="80" height="80" hspace="20" src="https://github.com/eisclimber/ExPresS-XR/assets/49446532/1935b2c9-000b-4440-8bd6-53c087d49b34">
ExPresS XR (<b>Ex</b>perimentation and <b>Pres</b>entation for <b>S</b>cience with Open<b>XR</b>) is a toolkit for VR and XR in Unity.
Based on the OpenXR Standard, its aim is to help automate early stages of development by providing configurable base implementations of components that are expected to be useful for scientific XR projects.  

## Getting Started & Documentation

If you are new, check out the [Getting Started](https://github.com/eisclimber/ExPresS-XR/wiki/Getting-Started)-Page in the wiki and [video tutorials](https://www.youtube.com/playlist?list=PLaAvR_HPw8vhvauv-PpZuULIV3pETSwn_).

The wiki and full documentation can be found [here](https://github.com/eisclimber/ExPresS-XR/wiki)

## Features

- A fully configurable XR Rig.
  - Supports `Controller` and `Head Gaze`-Input.
  - Different movement options: Teleportation, Continuous Move, Climbing, Grab Move, Teleport-Stations, ....
  - Customizable interaction options: Near-Far, Ray, Grab, Poke, ....
  - Head Collision with collision detection, pushback and visual feedback.
  - Virtual animated hands allowing you to grab, poke and push objects.
  - A basic implementation of Inverse Kinematics (IK). For more elaborate IK use add-ons like FinalIK.
- A huge expansion of Unity's interaction toolkit.
  - An all-in-one, expandable and easy-to-use system for creating custom interactables like buttons. levers, joysticks, multi-dimensional sliders, and much more.
  - Sockets that highlight their size and can be setup to accept certain objects.
  - A socket that will move objects back to the socket's position when no interaction is performed.
  - Scaling of grabbed objects.
  - UI keyboards usable with XR.
  - Support for hand/wrist menus.
  - Audio and haptic feedback for collisions and interactions.
  - Helpers for localization.
- An easy-to-use system for gathering and exporting data from anywhere in the VR.
  - Gather any CSV data within seconds!
  - Supports different separators and multiple columns.
  - Structured similar to Unity's event system.
  - Data can be saved locally or sent via HTTP.
  - Can be controlled through the editor or via code.
- Various VR minigames for testing your skils.
  - Archery (aka `BaAM`): An fully fledged out archery system along with targets and a minigame logic.
  - Boxing: A simple boxing mechanic.
  - Coin Scale: Find the counterfeit coin by comparing coins based on their weight.
  - Coin Throw: Throw coins (or objects) into moving targets.
  - Excavation: Discover objects by unveiling them from beneath the dirt. 
  - Puzzle: Socket based puzzles for creating jigsaw puzzles or similar mechanics.
  - Target Area: A basic system for trigger-based repeated interactions, like breaking stone.
  - Tile Game: A tile based game that lets you place tiles to gain points from creating larger and larger areas (similar to Dorfromantik or Carcassonne).
- Configurable exhibition displays to present objects along with further information.
- A fully customizable quizzing system.
  - Users answer a question by pressing a physical button in the VR.
  - Can be tailored and edited to one's likings using a setup dialog.
  - Allows configuring a multitude of parameters, such as: Multiple or single choice, number of answers, question order, etc.
  - Supports Questions, Answers and Feedback in the form of Text, GameObjects, Images and Videos.
  - The Feedback can be shown in different ways or omitted.
  - Everything can be exported via the data gathering system.
- A HUD system, allowing full screen fades and other permanent UI elements.
- Automatic Creation of "neutral"-looking rooms with specified dimensions for quick experiment setups.
- A VR-ready Main Menu Components and Scene.
- All the little helpers you'll need for making your perfect VR game.
- Ready to play example scenes showing the features and applications of `ExPresS XR`.
- In-editor setup dialogs and tutorials for a quick start with your project.
- Detailed documentation of the project components and common workflows.
- [YouTube Tutorials](https://www.youtube.com/watch?v=-k2wBBZ9a1w&list=PLaAvR_HPw8vhvauv-PpZuULIV3pETSwn_)


## Installation

The project is largely contained within in the `ExPresS XR` directory of the `Asset` Folder and aims to have as few external dependencies as possible.  
Because of this you can simply copy and the directory into your existing project, copying the Input Mappings from the `Settings` directory and adjusting `Tags` and `Interaction Layer`.

If you are starting with a new project the process is even simpler: Just press `Use this template` while you are logged in with your `GitHub` account to create a fresh `ExPresS XR` project.  

It is planned to separate the project into several smaller UnityPackages, that then will be accessible in the Asset Store.

## Made with ExPresS XR

![MadeWithExPresSXR](https://github.com/eisclimber/ExPresS-XR/assets/49446532/9a2dab28-50a6-4f29-a882-cb13002a6634)

- **Hetepheres Tomb** join famous egyptologist Peter Manuelian of the Harvard University at the Giza Necropolis in Egypt. At the foot of the Great Pyramid lies the mysterious tomb of queen Hetepheres, the mother of king Khufu. Help the excavators discover the queens secrets that lie almost 100 feet underground to restore the queens treasures.  
It is available for free on the [Meta Quest Store](https://www.meta.com/en-gb/experiences/hetepheres-tomb/28627787526867149/) and on [Steam](https://store.steampowered.com/app/3553060/Hetepheres_Tomb__Secrets_of_the_Lost_Queen).
- The internationally appraised exhibition **Tempelsteuer und Taubenhändler (Doves and Temple Taxes)** is a VR recreation of Herod's Temple with an emphasis on how currency was used and exchanged in temples in ancient times.  
It is available on [Sidequest](https://sidequestvr.com/app/33452/temple-tax-and-doves) and [Itch.io](https://eisclimber.itch.io/temple-tax-and-doves).
- **Hornmoldhaus VR** is an interactive and engaging exhibition about Japanese culture and medicine in ancient Germany. It features 3D scans of invaluable artifacts, a partial remodel of the city museum in Bietigheim-Bissingen and fun minigames.  
Released on [Sidequest](https://sidequestvr.com/app/21084/hornmoldhaus-vr) and [Itch.io](https://eisclimber.itch.io/hornmoldhaus-vr)
- A numismatic/roman-inspired exhibition with awesome minigames
- [Workshops on using VR for Cultural Heritage Presentation using ExPresS XR for the CIVIS Days 2023 and FORGE 2023](https://github.com/eisclimber/VRMuseumTemplate)
- [A recreation of an Egyptian burial chamber](https://github.com/eisclimber/VR-Burial-Chamber)
- [A sample serving as an VR-Exhibition of the church in Mühlen a. N.](https://github.com/eisclimber/express-xr-exhibition-kirche-muehlen)
- And of course the ready-to-play Scenes/Samples from the project itself:)

If you create your own project using ExPresS XR, feel free to contact me, so I can add you to the list (also don't forget the [survey](https://github.com/eisclimber/ExPresS-XR/blob/main/ExPresS%20XR%20Survey.pdf)).

## Known Issues

ExPress XR tries to evolve together with Unity's XR Interaction Toolkit. If you find any bugs, check out the [Issues](https://github.com/eisclimber/ExPresS-XR/issues)-Page and report them. If you want to provide fixes for the issues or expand ExPresS XR's functionality, feel free to open a Pull Request.

## Help Us Out

ExPresS XR needs your feedback to improve. For that, you will find a [survey](https://github.com/eisclimber/ExPresS-XR/blob/main/ExPresS%20XR%20Survey.pdf) in the repository of your project.

If you like the project, you can buy me a coffee (Programmer + Coffee = Code :D): https://ko-fi.com/eisclimber  


## Credits

Created and Maintained by Luca "eisclimber" Dreiling

Contributors:
- Lena Matulla: Archery System and Minigame (BAaM)
- Kevin Körner: Coin Scale and Coin Throw Minigames, and Head of Advertisement


Special Thanks to Kevin Körner for the great mentoring during and after the creation of the masters thesis this project is subject of. 
Also thanks to Stefan Krmnicek for his continuous engagement in the project itself and other projects using ExPresS XR.

Supported by the VWStiftung.


## Contact and Support 

Twitter/X: [@eisclimber](https://twitter.com/eisclimber)  
E-Mail: [luca.dreiling@gmx.de](mailto:luca.dreiling@gmx.de)

## Version

ExPresS XR is currently developed and tested with Unity 6000.3.9f1 and uses XR Interaction Toolkit 3.3.1.

## License

MIT License (Attribution is appreciated)
