# microscope

A CodeLens extension for Visual Studio that lets you inspect the intermediate language instructions of a method.

WIP: not published on the VS extension marketplace yet.

## TODO

### v0.1 Release

* deploy to marketplace

### v1.0 Release

* update when typing/saving/building
* XML doc summary of op code as tooltip
* double-clicking an instruction navigates to MSDN page
* `CodeLensDataPoint.GetDetails()` might fail, because `data` is `null`... but how?

### v1.X Release

* cache `AssemblyDefintion`s?
    * results of basic performance measurement:
        * first CodeLens takes ~1 sec, because in-memory compiling takes long
        * everything seems to be cached afterwards and instructions can be retrieved in 15 - 40 ms
        * compiling still takes most of the time (10 - 30 ms)
    * would shave off most of the runtime cost (except for first CodeLens)
    * but how to invalidate the cash on code changes?
    * is it worth the memory cost in big solutions?
    * reading an `AssemblyDefintion` is not thread-safe
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
* support `async` & `IEnumerable` state machines (would benefit from custom UI)
* support more code elements? (properties, classes, ...)

## Credits

* [VSCodeILViewer](https://github.com/JosephWoodward/VSCodeILViewer)
* https://marketplace.visualstudio.com/items?itemName=segrived.Msiler
* [Mono.Ceceil](https://github.com/jbevain/cecil)
