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

A MSDL-compatible SymStore bulk download tool
Version 4.0 Alpha 4

September 6, 2022

SymX is a bulk download tool for binaries stored on symbol servers created using SymStore.exe, including the Microsoft Symbol Server, It has been used to find files from unleaked builds of Windows as well as dumping all the binaries from a particular Windows version.
It is a merger of msdlurlgen and MassView that adds many new features to both, is far faster (100+ URLs per second at max threads, vs. <5), and is far more robust.

It is not intended as a replacement for symchk and cannot at present be used to download symbols, only binaries.
THIS IS AN ALPHA VERSION OF SYMX AND DOES NOT HAVE THE SAME GUARANTEED STABILITY AS A FINAL VERSION!

RELEASE NOTES HAVE BEEN MOVED TO RELEASENOTES.TXT AS OF 4.0.0 ALPHA4! 
Command-line syntax has changed with this version! Any batch files, shell scripts, or similar designed to work with SymX 3.x, or 4.0 alpha 3 or earlier, will not work!
Q&A for this application is in qanda.txt.

CONFIDENTIALITY NOTICE:
Please note that SymX is not a public tool. Due to the way it is designed, it could realistically be used for committing distributed denial of service attacks. No decision has yet been made on if it will be made public. Until then, please do not distribute this tool EXCEPT to people you __trust__.

Minimum Requirements:
x86-64 or ARM64 architecture
.NET 6.0 runtime
x86-64: Windows 7 SP1 or later
ARM64: Windows 10, Insider build 21277 (Cobalt) or later
An internet or intranet connection in order to download files. 
Windows Console Host 

Recommended Requirements:
x86-64 or ARM64 architeture
.NET 6.0 runtime
x86-64: Windows 10, version 1607 and later or Windows 11
Windows Terminal (faster text drawing)

Warnings:
On Windows 10, version 1507 (10240) and earlier operating systems, using normal velocity will cause SEVERE rendering issues due to a lack of support for virtual terminal sequences. 
Please use a newer operating system (1511 minimum, 1607 recommended) or a different verbosity level in order to get correct rendering on normal verbosity.

Normal and verbose mode will be a lot slower in conhost than Windows Terminal due to Terminal's faster (and non-blocking?) text rendering. Conhost blocks on text render and slows down all threads.

Excel converts any "xe000" string to scientific notation, which causes issues in CSV file generation. Either turn off scientific notation or use a different tool (such as Notepad or klogg) to work around this. 
