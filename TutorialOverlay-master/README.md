TutorialOverlay
==========

A simple way to add "tutorial" overlays to existing WPF applications.

How it works
------------------
At runtime, TutorialOverlay steps through a predefined set of "Steps", each of which define an element to highlight and a message to display explaining the step.  Elements are targeted by name, and are located by recursively searching the VisualTree, starting at the currently active Window element.

When TutorialOverlay detects that the user has appropriately interacted with the target element, TutorialOverlay automatically moves on to the next step in the tutorial.  The tutorial can be exited at any time by click the "Close tutorial" button in the top right corner of the overlay.