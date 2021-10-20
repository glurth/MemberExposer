This tool allows you to easily configure your scene's UI, to link with and display, members of a class.
It starts by defining several Pre-Fab UI controls, that have a ExposedMemberToUIAdapter component on them, and storing them in the project as pre-fab assets.

Then, the user can add a MemberExposer comonent to an otherwise empty gameobject in the scene.  Once added, the user may use this component to select a class and it's various members. With a button click, they can create a UI control in the scene for it (using of of the prefabs defined in the first step), and link the member's data with this component.

The program's state machine (or whatever), can active these control's and link them with a particular object (of the exposed class), by calling a single function in the MemberExposer, passing in the object to be displayed.

