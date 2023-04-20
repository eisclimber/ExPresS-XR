# ExPresS XR

ExPresS XR (**Ex**perimentation and **Pres**entation for **S**cience with Open**XR**) is a toolkit for VR and XR in Unity.
Based on the OpenXR Standard, it aims is to help automate early stages of development by providing configurable base implementations of components that are expected to be useful for scientific XR projects.  

## Now using the brand new Interaction Toolkit 2.3.0

ExPresS XR is now using Unity's XR Interaction Toolkit 2.3.0. This comes with an overhaul to the XR Rig, many new features such as Poke-Interactions and loads of bug fixes.  

## Getting Started & Documentation

If you are new check out the [Getting Started](https://github.com/eisclimber/ExPresS-XR/wiki/Getting-Started)-Page in the wiki and [video tutorials](https://www.youtube.com/playlist?list=PLaAvR_HPw8vhvauv-PpZuULIV3pETSwn_).

The full documentation can be found [here](https://github.com/eisclimber/ExPresS-XR/wiki)


## Structure

Following the aim to the OpenXR standard itself, ExPresS XR aims to allow development for a multitude of devices.
That why the project does not only support VR headsets with controllers but also a controller-free mode that can be used with smartphones (+ a VR mount, like the Google Cardboard)

The project is divided into three categories: General, Experimentation and Presentation.

- General: Implementations of configurable XR Rigs, Movement, XR-based UI and Interaction and a as in-editor tutorials
- Experimentation: Providing an easy solution to collect and export data, automatic generation of a "clean" test environment, as well as a fully customizable quizzing system
- Presentation: Options for displaying objects in VR in interesting ways that allow building virtual exhibitions with ease

Apart from the code itself the wiki features useful workflow tutorials that aim to help inexperienced developers (e.g. building end systems or scanning real world objects to be imported in the project)

## Full Feature List

- Ready to play example scenes showing the features and applications of `ExPresS XR`.
- In-editor setup dialogs and tutorials for a quick start with your project.
- Detailed documentation of the project components and workflow.
- A fully configurable XR Rig.
  - Three input modes: Controller, Head Gaze, Eye Gaze (untested).
  - Different movement options: Teleportation, Continuous Move, Grab Move, ... .
  - Customizable interaction options: Ray, Grab/Direct, Poke, UI, ... .
  - Collision and PlayArea collision detection and visual feedback.
  - Virtual animated hands allowing grabbing and pushing of objects.
  - The Controllers that are used can be displayed in game.
  - A system for Hand Animations when grabbing.
  - A basic implementation of Inverse Kinematics (IK). For more elaborate IK use addons like FinalIK.
- A great expansion of Unity's interaction toolkit.
  - Sockets that are highlight their size and can be setup to accept certain objects.
  - A Socket that will move objects back to to socket's position when no interaction is performed.
  - Physical Buttons with toggle mode.
  - Custom Teleportation Areas and Sockets.
  - UI keyboards usable with XR.
  - Sound-Emission uppon Collision
  - (Deprecated -> Supported by XRGrabInteractable now) Allow grabbing objects on the outside, rather than a single fixed attach point.
- A HUD-system, allowing full screen fades and other permanent ui elements.
- Configurable displays to present objects and further information in VR.
  - The objects can be picked up and inspected.
- Automatic Creation of an "neutral"-looking rooms with specified dimensions for quick experiment setups.
- An easy-to-use system for gathering and exporting data from anywhere in the VR.
  - Is structured similar to Unity's event system.
  - Data can be saved locally or be sent via http.
- A fully customizable quizzing system.
  - Users answer a question by pressing a physical button in the VR.
  - Can be tailored and edited to one's likings using an setup dialog.
  - Allows configuring a multitude of parameters such as: Multiple or single choice, number of answers, question order ... .
  - Supports Questions, Answers and Feedback in the form of Text, GameObjects, Images and Videos.
  - The Feedback can be shown in different ways or be omitted.
  - Everything can be exported via the data gathering system.
- Various little helpers for making life a bit easier.

## Help Us Out

ExPresS XR needs your feedback to improve. For that you will find a [survey](https://github.com/eisclimber/ExPresS-XR/blob/main/ExPresS%20XR%20Survey.pdf) in the repository of your project.

If you like the project you can buy me a coffee: https://ko-fi.com/eisclimber
(We all know: Programmer + Coffe = Code :D)


## Made with ExPresS XR

![ProjectsHeader](https://user-images.githubusercontent.com/49446532/221184626-22493d7d-66d8-422f-ae5b-192f57d661fe.png)

- [A Sample serving as an VR-Exhibition of the Church in Mühlen using ExPresS XR](https://github.com/eisclimber/express-xr-exhibition-kirche-muehlen)

If you create your own project using ExPresS XR feel free to contact me, so I can add you to the list (also don't forget the [survey](https://github.com/eisclimber/ExPresS-XR/blob/main/ExPresS%20XR%20Survey.pdf)).

## Known Issues

ExPress XR tries to evolve together with Unity's XR Interaction Toolkit. If you find any bugs check out the [Issues](https://github.com/eisclimber/ExPresS-XR/issues)-Page and report them. If you want to provide fixes for the Issues or expand ExPresS XR's functionality feel free to open a Pull Request.

## Credits

Created by Luca Dreiling

Special Thanks to Kevin Körner for the great mentoring during and after the creation of the masters thesis this project is subject of.


## Contact and Support 

Twitter: [@eisclimber](https://twitter.com/eisclimber)
E-Mail: [luca.dreiling@student.uni-tuebingen.de](mailto:luca.dreiling@student.uni-tuebingen.de).

## Version

ExPresS XR is currently developed and tested with Unity 2021.3.21f1.

## License

MIT License (Attribution is appreciated)
