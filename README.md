# microscope

A CodeLens extension for Visual Studio that lets you inspect the intermediate language instructions (IL) of a method.

WIP: nothing works yet.

## TODO

### V0.1 Release

* improve looking up types and members in an `AssemblyDefinition` (+ tests)
* measure runtime of major steps
* cache `AssemblyDefintion`s
* count `box` and unconstrained `callvirt` instructions
* deploy to marketplace

### V1.0 Release

* update when typing/saving/building
* XML doc summary of op code as tooltip
* double-clicking an instruction navigates to MSDN page

### V1.X Release

* VB support
* F# support?
* is it possible to move in-memory compiling and IL retrieval out of the VS process?
    * is it actually running in-proc?
    * can we even access the `Compilation` out-of-proc?
* custom UI

## Credits

* [Mono.Ceceil](https://github.com/jbevain/cecil)
* [VSCodeILViewer](https://github.com/JosephWoodward/VSCodeILViewer)
