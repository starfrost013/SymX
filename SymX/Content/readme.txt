SymX
A Microsoft Symbol Server bulk download tool
Version 3.1.0 Alpha 1

31 May 2022

SymX is a bulk download tool for binaries stored on the Microsoft Symbol Server. It has been used to find files from unleaked builds of Windows as well as dumping all the binaries from a Windows version.
It is a merger of msdlurlgen and MassView that adds many new features to both, is far faster (100+ URLs per second at max threads, vs. <5), and is far more robust.

It is not intended as a replacement for symchk and cannot at present be used to download symbols, only binaries.

This program requires an x64 machine (ARM64 can be compiled on request) and the .NET 6.0 runtime. It also requires an internet or intranet connection in order to download files. 

RELEASE NOTES:
3.0.3 (May 31, 2022):
* Fix -filename not actually being required

3.0.2 (May 30, 2022 20:20):
* Fix percentage (i am extremely dumb)
* More minor refactoring

3.0.1 (May 30, 2022 20:05):
* Fix quiet verbosity not actually downloading any files
* Fix retry count not being reset for each file
* Hopefully fix "9/8" retries bug
* Fix file download count always being 12 behind
* Very minor code refactoring
* Fix minor typo in help 

3.0.0 (May 29, 2022 15:15):
* More grammatical fixes in help, readme, and in some strings

RC 2 (May 29, 2022 13:46):
* Fixed help

RC 1 (May 29, 2022 13:40):
* Fixed bug where the folder specified by -outfolder would not be created in some cases.
* Changed text colours around a bit
* Fixed typos in text
* Increased default retry count from 5 to 8
* Added additional command-line parsing error messages
* Added handling for when SuccessfulURLs.log is already open
* Fixed bug where custom symbol servers were not respected by MassView

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
* Fixed multiple file downloading always using the file name of the first file found
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
* MassView merged in
* Ability to download files from a CSV
* Hex time support!
* Overridable user-agent and support for non-Microsoft symbol servers
* Downloading multiple files!
* Changing the output directory!

I found a bug!
- DM me (Starfrost#9088) with what happened, OS, architecture, and your CL args. Feature suggestions are welcome!