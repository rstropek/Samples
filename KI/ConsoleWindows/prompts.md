I am building a framework for a simple window manager for the console. The entire logic should be in the ConWin.Lib project. ConWin.App is a demo app. The xUnit tests should be in the ConWin.Tests project.

Let's start by building an abstract base WindowBase class representing a window. A window is a rectangular area on the screen. It has the following properties:

* Position
* Size
* Background color (default is black)
* Foreground color (default is white)
* Z-index (default is 0)

The window must have a virtual Draw method that draws the window on the screen.

Also add helper records for Position and Size. Colors should be represented by Console Colors from .NET.

Derive a Window class from WindowBase. The Window class should have the following properties:

* Title
* Border style (single, double, none; none is default)

Create a WindowManager class that manages the windows. Drawing of top-level windows must be done in the order of the z-index. However, if a window is a child of another window, it should be drawn on top of the parent window. A window must not be drawn if it is not visible (i.e. another window is fully in front of it). We do not support partial drawing of windows.

For now, do not generate unit tests. However, I would like to have a demo app in ConWin.App that shows how to use the framework.

---

I am building a framework for a simple window manager for the console. The entire logic should be in the ConWin.Lib project. ConWin.App is a demo app. The xUnit tests should be in the ConWin.Tests project.

I need keyboard handling. The WindowManager should offer an async method that handles key presses until the user presses Ctrl+q. The Window Manager must keep track of a single window that currently has the focus. The WindowBase class must have a virtual method that handles key presses. The WindowManager must call this method when the user presses a key and the window has the focus.

Additionally, WindowBase must have helper methods similar to Console.Write and Console.SetCursorPosition. The position must be relative to the window's position + border. The foreground and background colors must match the window's properties. The helper methods must ensure that a caller cannot write outside the window's bounds (i.e. check the length of the string to write and the position + length of the string to write).


