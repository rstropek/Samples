[Details](https://docs.microsoft.com/en-us/visualstudio/releases/2019/release-notes)

# New Start Dialogs

* Recent projects
* Clone from start dialog
* *New Project* dialog
  * So many project types -> needed refactoring
  * Tag logic (try *web linux*)
  * Recent types
  * Launch installer if something is missing
* Project configuration wizard
  * Project types depend on selected framework version

# Various Improvements

* More vertical room
* Subtle visual changes
* Better handling of display configurations in multi-monitor scenarios
  * Search for *Visual Experience* settings
* Seach for *Preview Features* -> configure preview features centrally
* Preview/trial/paid extensions
* Performance improvements
* Git support for stashing changes
* [*Pull Requests for Visual Studio*](https://marketplace.visualstudio.com/items?itemName=VSIDEVersionControlMSFT.pr4vs) extension (not covered here)
* Managing Azure DevOps work items within Visual Studio (not covered here)
* Search for *maximum allowed typing latency*
* Default C# language version depends on framework version
* Hover over closing brace -> information about context of block
* *Kubernetes*/*Helm* support
  * *Add* on `DemoApi`, add *Container Orchestrator Support*
  * Mention *Azure Dev Spaces*
* *Background Tasks* icon in lower left corner
  * *Rebuild all* in *StarshipTraveler* solution

# Interesting New Shortcuts

* Visual Clipboard Ring (*Ctrl + Shift + V*)
* *Build Selection* (*Ctrl + B*)

# Solution Filters

* Open *StarshipTravelers* sample
* Unload all projects except *Model*
* Save as solution filter file and show its content
* *Do not load projects* settings in *Open Project* dialog

# *Search Visual Studio*

* Search for:
  * *Font*
  * *web app*
  * *profile*
* Fuzzy search: *nisights*
* New project from search (search for *core web app*)

# Refactorings

* New refactorings
* [Reference](https://docs.microsoft.com/en-us/visualstudio/ide/refactoring-in-visual-studio?view=vs-2019)
  * Change method signature for `BuildFullName`
  * Parameter wrapping for `BuildFullName` and `return`-statement in `IsValidPerson`
  * Turn anonymous type into tuple for `CalculateAverageAge`
  * Turn anonymous type into class for `IsValidPerson`
    * Turn into class `Person`
    * Wrap and align
    * Move to to separate file
    * Create interface `IPerson`
    * Pull up two properties into interface
    * Move interface to separate file
  * Expression body vs. block body for `CalculateAverageAge`
  * Refactor `result` in `IsValidPerson`:
    * Move declaration near reference
    * Wrap and align
    * Implicit typing
    * Inline variable
  * Unused variables diagnostics
    * Use discard for `dummy` in `Main`
  * Code generation have been updated: Generate deconstructor for `Appelation`

# Live Share

* *Not* screen sharing
* New: Installed by default
* Start live sharing session with VSCode
* Speak about *Read Only* mode
* New features:
  * Demo: Start debugging session (show *Live Share* options)
  * Demo: Add comment in VS, reply in VSCode
  * Access to source control diffs (for e.g. code reviews)
  * Education mode for up to 30 users
* [More...](https://devblogs.microsoft.com/visualstudio/visual-studio-live-share-for-real-time-code-reviews-and-interactive-education/)

# Code Lense

* Now part of Community Edition
* Demo *Reference* data in Code Lenses
* Show *Reference* for `Base.Name` -> categorized by read/write

# IntelliCode

* Add DB access code in *IntelliCode.cs*
* Mention that it can be trained from own code ([details](https://docs.microsoft.com/en-us/visualstudio/intellicode/intellicode-visual-studio))

# Document Health and Code Cleanup

* Add `int i` to *UglyBrokenCode.cs* and demonstrate Document Health indicator
* Show *Code Cleanup* settings
* Search *var* in VS search -> *Code Style* settings
* Speak about influence of [*.editorconfig*](https://docs.microsoft.com/en-us/visualstudio/ide/create-portable-custom-editor-options) file
* Run *Code Cleanup* on *UglyBrokenCode.cs*

# Debugging

* Set breakpoint at the end of `DoSomethingWithTickets`
* Run program
* In *Locals*, search for *Cybertron*
  * Point out depth filter
* Create a watch expression `fullTicketInfo[0].Passenger`, add *,* to see *data format options*
* Debug when value changes
  * Start debugger
  * Set breakpoint `somethingImportantHappened` when value changes on a *specific* item
  * Demonstrate data breakpoint, show use of object identifier in call stack

# Performance

* Search for *Product Updates*
  * Updates now downloaded in background
  * Control installation mode

