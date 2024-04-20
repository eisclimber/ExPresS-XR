# ExPresS XR

<img align="left" width="80" height="80" src="https://github.com/eisclimber/ExPresS-XR/assets/49446532/1935b2c9-000b-4440-8bd6-53c087d49b34">
 ExPresS XR (<b>Ex</b>perimentation and <b>Pres</b>entation for <b>S</b>cience with Open<b>XR</b>) is a toolkit for VR and XR in Unity.
Based on the OpenXR Standard, its aim is to help automate early stages of development by providing configurable base implementations of components that are expected to be useful for scientific XR projects.  

## Getting Started & Documentation

If you are new, check out the [Getting Started](https://github.com/eisclimber/ExPresS-XR/wiki/Getting-Started)-Page in the wiki and [video tutorials](https://www.youtube.com/playlist?list=PLaAvR_HPw8vhvauv-PpZuULIV3pETSwn_).

The full documentation can be found [here](https://github.com/eisclimber/ExPresS-XR/wiki)


## Structure

Following the aim of the OpenXR standard itself, ExPresS XR allows development for a multitude of devices.
That is why the project does not only support VR headsets with controllers but also a controller-free mode that can be used with smartphones (+ a VR mount, like the Google Cardboard)

The project is divided into three categories: General, Experimentation and Presentation.

- General: Implementations of configurable XR Rigs, Movement, XR-based UI and Interaction, as well as in-editor tutorials
- Experimentation: Providing an easy solution to collect and export data, automatic generation of a "clean" test environment, as well as a fully customizable quizzing system
- Presentation: Options for displaying objects in VR in interesting ways that allow building virtual exhibitions with ease

Apart from the code itself, the wiki features useful workflow tutorials that aim to help inexperienced developers (e.g. building end systems or scanning real-world objects to be imported into the project)

## Features

- Ready to play example scenes showing the features and applications of `ExPresS XR`.
- In-editor setup dialogs and tutorials for a quick start with your project.
- Detailed documentation of the project components and workflow.
- A fully configurable XR Rig.
  - Three input modes: Controller, Head Gaze, and Eye Gaze (untested).
  - Different movement options: Teleportation, Continuous Move, Climbing, Grab Move, Teleport-Stations, ....
  - Customizable interaction options: Ray, Grab/Direct, Poke, UI, ....
  - Head Collision with collision detection, pushback and visual feedback.
  - Virtual animated hands allowing grabbing and pushing of objects.
  - Display the controllers used in the game.
  - A system for hand animations when grabbing.
  - A basic implementation of Inverse Kinematics (IK). For more elaborate IK use add-ons like FinalIK.
- A great expansion of Unity's interaction toolkit.
  - Scaling of grabbed objects.
  - Sockets that highlight their size and can be setup to accept certain objects.
  - A socket that will move objects back to the socket's position when no interaction is performed.
  - Physical Buttons with toggle mode.
  - Custom teleportation areas and sockets.
  - UI keyboards usable with XR.
  - Sound- and rumble-emission upon collision.
  - Grab and interaction triggers.
  - Various minigames and much more!
  - Helpers for localization.
- A HUD system, allowing full screen fades and other permanent UI elements.
- Configurable displays to present objects and further information in VR.
  - The objects can be picked up and inspected.
- Automatic Creation of "neutral"-looking rooms with specified dimensions for quick experiment setups.
- An easy-to-use system for gathering and exporting data from anywhere in the VR.
  - Gather any CSV data within seconds!
  - Supports different separators and multiple columns.
  - Structured similar to Unity's event system.
  - Data can be saved locally or sent via HTTP.
  - Can be controlled through the editor or via code.
- A fully customizable quizzing system.
  - Users answer a question by pressing a physical button in the VR.
  - Can be tailored and edited to one's likings using a setup dialog.
  - Allows configuring a multitude of parameters, such as: Multiple or single choice, number of answers, question order, etc.
  - Supports Questions, Answers and Feedback in the form of Text, GameObjects, Images and Videos.
  - The Feedback can be shown in different ways or omitted.
  - Everything can be exported via the data gathering system.
- A VR-ready Main Menu Components and Scene
- All the little helpers you'll need for making your perfect VR game.
- A huge wiki and [YouTube Tutorials](https://www.youtube.com/watch?v=-k2wBBZ9a1w&list=PLaAvR_HPw8vhvauv-PpZuULIV3pETSwn_)


## Made with ExPresS XR

![MadeWithExPresSXR](https://github.com/eisclimber/ExPresS-XR/assets/49446532/9a2dab28-50a6-4f29-a882-cb13002a6634)

- The internationally appraised exhibition "Tempelsteuer und Taubenhändler (Doves and Temple Taxes)" is a VR recreation of Herod's Temple with an emphasis on how currency was used and exchanged in temples in ancient times.  
The exhibition is currently touring various cities in Europe and will be coming to Itch and Sidequest very soon.
- Hornmoldhaus VR is an interactive and engaging exhibition about Japanese culture and medicine in ancient Germany. It features §D scans of invaluable artifacts, a partial remodel of the city museum in Bietigheim-Bissingen and fun minigames.  
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

Created by Luca "eisclimber" Dreiling

Contributors:
- Kevin Körner: Coin Scale and Coin Throw Minigames


Special Thanks to Kevin Körner for the great mentoring during and after the creation of the masters thesis this project is subject of. 
Also thanks to Stefan Krmnicek for his continuous engagement in the project itself and other projects using ExPresS XR.

Supported by the VWStiftung.


## Contact and Support 

Twitter: [@eisclimber](https://twitter.com/eisclimber)
E-Mail: [luca.dreiling@gmx.de](mailto:luca.dreiling@gmx.de)

## Version

ExPresS XR is currently developed and tested with Unity 2022.3.24f1.

## License

MIT License (Attribution is appreciated)
