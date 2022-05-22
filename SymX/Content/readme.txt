SymX Alpha Preview
A Microsoft Symbol Server bulk download tool
Version 3.0.0 Alpha 2

22 May 2022

This is an experimental and buggy preview of the future merger of msdlurlgen and MassView (the MassView functionality is not in yet) that adds many new features, is far faster, and far more robust.

Please note that this is alpha quality software, is missing features, and is not guaranteed to be stable. Please suggest features and test for bugs! Requires an x64 machine (ARM64 will also be supported and can be compiled on request).

--Changes from msdlurlgen 2.6.0--
* Fully rewritten in C# and .NET 6.0
* Y2K38 compliance
* Multithreading! (over an order of magnitude faster)
* Logging!
* Different levels of verbosity!
* Ability to specify a range of image sizes!

Features coming in later alpha versions:
* MassView features!
* Downloading multiple files
* CSV input (read a csv, download all files) 
* Dump to URL and exit!