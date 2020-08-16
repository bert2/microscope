# microscope

A CodeLens extension for Visual Studio that lets you inspect the intermediate language instructions (IL) of a method.

WIP: not published on the VS extension marketplace yet.

## TODO

### v0.1 Release

* improve looking up types and members in an `AssemblyDefinition` (+ tests)
    * nested classes
    * overloads
    * generic methods/classes
    * ...
    * do we really have to implement this on our own?
* make logging configurable or disable it in release builds
* strong-name `Microscope.Shared` correctly (it's currently signed by me which seems stupid)
* deploy to marketplace

### v1.0 Release

* create icon
    * can I use GitHub's :microscope: emoji?
    * emojis are probably too small for marketplace (90x90)
* update when typing/saving/building
* XML doc summary of op code as tooltip
* double-clicking an instruction navigates to MSDN page

### v1.X Release

* cache `AssemblyDefintion`s?
    * results of basic performance measurement:
        * first CodeLens takes ~1 sec, because in-memory compiling takes long
        * everything seems to be cached aftewards and instructions can be retrieved in 15 - 40 ms
        * compiling still takes most of the time (10 - 30 ms)
    * would shave off most of the runtime cost (except for first CodeLens)
    * but how to invalidate the cash on code changes?
    * is it worth the memory cost in big solutions?
* edge cases:
    * what happens when opening a project instead of a solution?
    * what happens when opening a file without opening the project?
* settings page to configure update frequency
* VB support
* F# support?
* is it possible to move in-memory compiling and IL retrieval out of the VS process?
    * is it actually running in-proc?
    * can we even access the `Compilation` out-of-proc?
* custom UI?
* support more code elements? (properties, classes, ...)

## Credits

* [VSCodeILViewer](https://github.com/JosephWoodward/VSCodeILViewer)
* [Mono.Ceceil](https://github.com/jbevain/cecil)
