# microscope

[![build](https://img.shields.io/appveyor/build/bert2/microscope/main?logo=appveyor)](https://ci.appveyor.com/project/bert2/microscope/branch/main) [![Visual Studio Marketplace Version](https://img.shields.io/visual-studio-marketplace/v/bert.microscope?label=Marketplace&logo=visual-studio&logoColor=%23bb88f3)](https://marketplace.visualstudio.com/items?itemName=bert.microscope) [![Visual Studio Marketplace Installs](https://img.shields.io/visual-studio-marketplace/i/bert.microscope?label=installs&logo=visual-studio&logoColor=%23bb88f3)](https://marketplace.visualstudio.com/items?itemName=bert.microscope) ![last commit](https://img.shields.io/github/last-commit/bert2/microscope/main?logo=github)

A CodeLens extension for Visual Studio that lets you inspect the intermediate language instructions of a method.

![Usage example](img/usage.gif "Usage example")

## Usage

- Install latest stable release from the [Visual Studio Marketplace](https://marketplace.visualstudio.com/items?itemName=bert.microscope).
- Alternatively you can grab the [VSIX package](https://ci.appveyor.com/project/bert2/microscope/branch/main/artifacts) of the latest build from AppVeyor.
- The CodeLens appears on C# methods and displays the number of instructions that will be generated for the method.
- Hover over the CodeLens to see individual counts for the number of `box` and unconstrained `virtcall` instructions in the method.
- Click the CodeLens to get a detailed list of all instructions including their offsets and operands.
- The CodeLens currently won't be updated after the method has been changed. In order to refresh it you will have to close and re-open the C# source file. Auto-update will be implemented in the next release.
- In case the retrieval of instructions fails the CodeLens will display `-` instead of a count. Hover over the CodeLens to see the exception that caused the failure.

## Known issues

- Instructions are not updated automatically on code changes.
- Retrieval of instructions might fail for methods wich have overloads (especially when generic parameters are involved).

## TODO

### Release (1.0.0)

* update IL when typing/saving/building
* XML documentation summary of op code as tooltip
* double-clicking an instruction navigates to MSDN page of op code
* settings page to enable/disable logging

### Future releases

* don't show CodeLens on interface/abstract methods
* cache `AssemblyDefintion`s?
    * results of basic performance measurement:
        * first CodeLens takes ~1 sec, because in-memory compiling takes long
        * everything seems to be cached afterwards and instructions can be retrieved in 15 - 40 ms
        * compiling still takes most of the time (10 - 30 ms)
    * would shave off most of the runtime cost
    * cash needs to be invalidated on code changes
    * is it worth the memory cost in big solutions?
    * reading an `AssemblyDefintion` is not thread-safe
* edge cases:
    * what happens when opening a project instead of a solution?
    * what happens when opening a file without opening the project?
* setting to configure update frequency
* VB support
* F# support?
* is it possible to move in-memory compiling and IL retrieval out of the VS process?
    * can we even access the `Compilation` out-of-proc?
* custom UI?
* support `async` & `IEnumerable` state machines (would benefit from custom UI)
* support more code elements? (properties, classes, ...)

## Changelog

### 0.0.1

- intitial preview release
- enables CodeLens on C# methods showing the number of IL instructions
- clicking the CodeLens opens a details view listing all IL instructions of the method
- refreshing the CodeLens after code changes currently requires closing and re-opening the C# source file

## Credits

### Similar tools

* [VSCodeILViewer](https://github.com/JosephWoodward/VSCodeILViewer) by Joseph Woodward only works with VSCode and isn't updated anymore. Joseph wrote a nice [article](https://josephwoodward.co.uk/2017/01/c-sharp-il-viewer-vs-code-using-roslyn) on its implementation which helped me getting started.
* [Msiler](https://marketplace.visualstudio.com/items?itemName=segrived.msiler2017) by Evgeniy Babaev looks like an excellent tool, but unfortunately it's not available for Visual Studio 2019. I discovered it when I was way into the development of microscope and if I had found it earlier, I might have tried patching Msiler first.

### Dependencies

* [Roslyn](https://github.com/dotnet/roslyn) compiles the current project in memory.
* [Mono.Ceceil](https://github.com/jbevain/cecil) retrieves the IL instructions from the compiled project.
* [nuke](https://github.com/nuke-build/nuke) orchestrates the builds for microscope.
