                                                                  *%%%%%#       
                                                               *%%,     #%      
                                                             (%#        /%.     
                                                    @@@@@@,#%/   @@@@@& %%      
     /&@&%,                                          .@@@@@&   *@@@@@. ,%/      
  *@@,    #@@                                          @@@@@@ @@@@@&   %#       
  @@.                             .**      ,*.        #%#@@@@@@@@@.   %#        
   @@@(        %@&      @@*  (@@@,  .@@@@&   (@@    .%#  ,@@@@@@@,  ,%(         
      ,%@@@@    %@%    &@/   (@@      @@,     (@%  #%.  &@@@@@@@@@@(%,          
           @@,   %@(  %@(    (@%      @@.     *@& %#  ,@@@@@  (@@@%%,           
 .&/       @@,    %@/#@(     (@%      @@.     *@@%/  @@@@@&    .%%@@@&          
  *@@@@@@@@@       %@@(      (@%      @@.     /@@  *@@@@@*   .%%.@@@@@@.        
                   %@(                                     *%#                  
                  &@(                                                           
              *%@%,                                                            

A Microsoft Symbol Server bulk download tool
Version 4.0 Alpha 3

xx July 2022

SymX is a bulk download tool for binaries stored on the Microsoft Symbol Server. It has been used to find files from unleaked builds of Windows as well as dumping all the binaries from a Windows version.
It is a merger of msdlurlgen and MassView that adds many new features to both, is far faster (100+ URLs per second at max threads, vs. <5), and is far more robust.

It is not intended as a replacement for symchk and cannot at present be used to download symbols, only binaries.
THIS IS AN ALPHA VERSION OF SYMX AND DOES NOT HAVE THE SAME GUARANTEED STABILITY AS A FINAL VERSION!

Minimum Requirements:
x86-64 or ARM64 architecture
.NET 6.0 runtime
x86-64: Windows 7, Windows 8.1, Windows 10 version 1607 or later, or Windows 11
ARM64: Windows 10, build 21277 or later (SymX-UI.exe ia not supported)
An internet or intranet connection in order to download files. 
Windows Console Host 

Recommended Requirements:
x86-64 or ARM64 architeture
.NET 6.0 runtime
x86-64: Windows 10, version 1607 and later or Windows 11
Windows Terminal (faster text drawing)

Warning: On Windows 10, version 1507 (10240) and earlier operating systems, using the SymX 3.1 UI (normal verbosity) will cause SEVERE rendering issues due to a lack of support for virtual terminal sequences. 
Please use a newer operating system or verbose mode in order to get correct functioning.

Verbose mode and, in future, the SymX 4 UI will be a lot slower in conhost than Windows Terminal due to Terminal's faster (and non-blocking?) text rendering. Conhost blocks on text render and slows down all threads.

Also, excel converts any "xe000" string to scientific notation. Either turn off scientific notation or use a different tool (such as Notepad or klogg) to work around this. 

4.0.0 Alpha 3 (July xx, 2022):
* Added configurability. Settings can be optionally stored in SymX.ini, and will be loaded from it if it exists. Additionally, you can use the -inipath option in order to specify a custom INI to load.
* Fixed a crash when -outfile specified a folder or file that already existed.
* Minor refactoring to remove some redundant code in the CSV generation mode
* Got rid of all remaining independent MassView branding

RELEASE NOTES:
4.0.0 Alpha 2.2 (July 14, 2022):
* Fixed successful downloads sometimes not incrementing

4.0.0 Alpha 2.1 (June 26, 2022):
* Fixed -numdownloads

4.0.0 Alpha 2 (June 26, 2022):
* Downloads are now multithreaded! By default, the number of threads used for downloading will be set to -numthreads (except if it is above 15, where it will be set to 15). However, if you want to override the value, you can use -numdownloads.
* SymX now measures download speed and file size and displays it when verbosity is verbose.
* SymX now obtains the Last-Modified date (only valid for dates after 6/11/2017 due to the Azure move) and displays it on verbose verbosity.
* Old logs are now automatically deleted on startup.
* Reduced log size a bit on normal verbosity by not logging as many things to file.
* Refactoring in NuCore and additional logging functionality
* Added -nologo if you hate version numbers for some reason 
* SymX now deletes partially downloaded files if they fail to download. This fixes the "zero-byte" file issue.
* Fixed -numthreads not being bounded when using -infile

3.1.0 Alpha 1 (June 10, 2022):
* Added a proper user interface with a progress bar
* Added overwrite prevention: The number at the start of the file will be incremented if it exists.
* -recursive option: Allows MassView to recursively search for CSV files
* Text changes
* Fixed help file issues
* Fixed SuccessfulURLs.log (but it may only work after it completes the scan)
* Refactoring

This build does not yet include automatic restart after a certain amount of time or multithreaded downloading, or some other features I have planned for this version of SymX.

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