# microscope

A CodeLens extension for Visual Studio that lets you inspect the intermediate language instructions (IL) of a method.

WIP: nothing works yet.

TODO:

* smarter print of instruction operand (+ test)
* improve looking up types and members in an `AssemblyDefinition` (+ tests)
* measure runtime of major steps
* cache `AssemblyDefintion`s
* count `box` and unconstrained `callvirt` instructions
* deploy to marketplace
* update when typing/saving/building
* XML doc summary of op code as tooltip
* double-clicking an instruction navigates to MSDN page
* VB support
* F# support?
* custom UI
