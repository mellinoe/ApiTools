# ApiTools

Repository containing several libraries and tools used in our CoreFx build process, especially related to facade creation and manipulation,
and contract generation and verification.

# Components

* GenFacades : A command-line tool used for creating full and partial facades.
* Microsoft.Fx.CommandLine : A simple command-line parsing library.
* Microsoft.CCI.Extensions : Some more specialized extensions useful for more specific assembly analysis and manipulation, built on top of CCI.

This repository builds assemblies/executables for both .NET Core and the full .NET Framework. Certain features are currently non-portable,
such as PDB generation and rewriting, and therefore can't be used from the .NET Core version of the tool. Eventually we aim to eliminate any
such inconsistencies and move fully over to the .NET Core version.

NOTE: The desktop version doesn't use ILMerge on the executable/its dependencies, so it isn't exactly identical to the one produced by our internal build.