# microscope

A CodeLens extension for Visual Studio that lets you inspect the intermediate language instructions (IL) of a method.

WIP: not published on the VS extension marketplace yet.

## TODO

### v0.1 Release

* improve looking up types and members in an `AssemblyDefinition` (+ tests)
* measure runtime of major steps
* cache `AssemblyDefintion`s
* deploy to marketplace

### v1.0 Release

* create icon
    * can I use GitHub's :microscope: emoji?
    * emojis are probably too small for marketplace (90x90)
* update when typing/saving/building
* XML doc summary of op code as tooltip
* double-clicking an instruction navigates to MSDN page

### v1.X Release

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
