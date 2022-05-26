SymX Alpha Preview
A Microsoft Symbol Server bulk download tool
Version 3.0.0 Alpha 4

26 May 2022

This is an experimental and buggy preview of the future merger of msdlurlgen and MassView (the MassView functionality is not in yet) that adds many new features, is far faster, and far more robust.

Please note that this is alpha quality software, is missing features, and is not guaranteed to be stable. Please suggest features and test for bugs! Requires an x64 machine (ARM64 will also be supported and can be compiled on request).

RELEASE NOTES:


Alpha 3 (May 23, 2022):
* MassView is now integrated! Use -generatecsv in combination with -csvinfolder and -csvoutfile
* Fixed MassView not working with read only files
* Added URL field to MassView
* Refactoring
* Fixed bug with files being added to the download queue hundreds of thousands of times
* Kind of allowed multiple file downloads (a number is prepended so it isn't overwritten)
* Fixed typos in the help file
* Mitigated issue where Verbose mode + -l option randomly crashed (and spewed 0x00 to the log file), something to do with thread safety
* Verbose mode is even more verbose

Alpha 2 (May 22, 2022)
* Initial release

--Changes from msdlurlgen 2.6.0--
* Fully rewritten in C# and .NET 6.0
* Y2K38 compliance
* Multithreading! (over an order of magnitude faster)
* Logging!
* Different levels of verbosity!
* Ability to specify a range of image sizes!
* MassView features!
* Downloading multiple files (kind of)

Features coming in later alpha versions:

* CSV input (read a csv, download all files) 
* Changing the output directory
* Hex string imput for start and end time (i'm working on it pivot :D)

I found a bug!
- DM me (Starfrost#9088) with what happened, OS, architecture, and your CL args.