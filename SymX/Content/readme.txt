SymX Alpha Preview
A Microsoft Symbol Server bulk download tool
Version 3.0.0 Alpha 5

28 May 2022

This is an experimental and buggy preview of the future merger of msdlurlgen and MassView that adds many new features, is far faster, and is far more robust.

Please note that this is alpha quality software, is missing planned features, and is not guaranteed to be stable. Please suggest features and test for bugs! Requires an x64 machine (ARM64 will also be supported and can be compiled on request).

RELEASE NOTES:

Alpha 5 (May 28, 2022 21:30): 
* SymX will now try to resume downloads a number of times before failing. -maxretries will control this value - the default is 5.
* Fixed URLs per second always being 0 bug
* Added -symbolserverurl: Allows you to use a custom symbol server url for download tools. The default is https://msdl.microsoft.com/download/symbols
* Added -useragentvendor and -useragentversion. These do not work with the default Microsoft symbol server and are only used for custom symbol servers.
* Fixed command line parsing bug where -start, -end, -imagesize, and -filename needed to be passed when supplying -infile
* Added -outfolder to change the file download output folder. Default is /download in the root directory. A slash is automatically appended.
* Fixed some minor bugs with text
* Added new icon to SymX.exe

Alpha 4 (May 28, 2022 13:15):
* Added -infile - will attempt to download all files from a CSV generated with -generatecsv (this only works with Alpha 4-generated CSVs due to the lack of a properly functional URL column)
* Fixed invalid MassView URL generation bug
* Download report now reports URLs per second
* Fixed multiple file downloading always having using the file name of the first file found
* Added a temporary file generated with the list of successful links. Useful if it goes over scrollback or you crash. -dontgeneratetempfile / -dtemp suppresses the generation of this log file.
* Added vanity logo in \Content
* Added timeout handling. 
* Implemented -hextime / -h option, which will interpret the -start and -end parameters as a hex-format 
* Refactoring and code cleanup
* Now outputs how many URLs failed to download or timed out
* Now outputs URLs per second rate
* Successful URLs are now outputted in green 

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