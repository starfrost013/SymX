using NuCore.Utilities;
using System.Globalization;

namespace SymX
{
    public static class CommandLine
    {
        /// <summary>
        /// The start (in 64-bit unix time) epoch to scan for files in.
        /// </summary>
        public static ulong Start { get; set; }

        /// <summary>
        /// The end (in 64-bit unix time) epoch to scan for files to.
        /// </summary>
        public static ulong End { get; set; }

        /// <summary>
        /// The filename to scan for on the Microsoft Symbol Server.
        /// </summary>
        public static string FileName { get; set; }

        /// <summary>
        /// The filename to save the file to if downloaded.
        /// </summary>
        public static string OutFile { get; set; }

        /// <summary>
        /// If this string is not null, the list of URLs will be dumped to this filename. 
        /// </summary>
        public static string UrlOutFile { get; set; }

        /// <summary>
        /// The image size to search for on the symbol server.
        /// Ignored if both <see cref="ImageSizeMin"/> and <see cref="ImageSizeMax"/> are set.
        /// </summary>
        public static string ImageSize { get; set; }

        /// <summary>
        /// Optional minimum image size to search for on the symbol server. 
        /// If this and <see cref="ImageSizeMax"/> are set, <see cref="ImageSize"/> is ignored.
        /// </summary>
        public static ulong ImageSizeMin { get; set; }

        /// <summary>
        /// Optional maximum image size to search for on the symbol server. 
        /// If this and <see cref="ImageSizeMin"/> are set, <see cref="ImageSize"/> is ignored.
        /// </summary>
        public static ulong ImageSizeMax { get; set; }

        /// <summary>
        /// A MassView-format CSV file containing a list of files, imagesizes, and TimeDateStamps to search for.
        /// </summary>
        public static string InFile { get; set; }

        /// <summary>
        /// If true, a CSV will be generated, then the program will exit.
        /// </summary>
        public static bool GenerateCsv { get; set; }

        /// <summary>
        /// The folder to input CSV files from. Ignored if <see cref="GenerateCsv"/> is not set to true.
        /// </summary>
        public static string CsvInFolder { get; set; }

        /// <summary>
        /// The verbosity level of the application.
        /// See <see cref="Verbosity"/> for extended description.
        /// </summary>
        public static Verbosity Verbosity { get; set; }

        /// <summary>
        /// Do not download files. Simply generate a list of URLs, optionally dump them to a file, and exit.
        /// </summary>
        public static bool DontDownload { get; set; }

        /// <summary>
        /// If true, logging information will be printed to a file. Respects verbosity. 
        /// </summary>
        public static bool LogToFile { get; set; }

        /// <summary>
        /// Limit the number of requests per second so as to not throttle the Microsoft symbol servers.
        /// 30 is the default. Set to -1 for unlimited request speed (dangerous!)
        /// </summary>
        public static int NumThreads { get; set; }

        /// <summary>
        /// Determines that a hex string is being passed directly for the Start and End switches.
        /// </summary>
        public static bool HexTime { get; set; }

        /// <summary>
        /// If this flag is set, the temporary text file containing successful URLs will not be
        /// generated.
        /// </summary>
        public static bool DontGenerateTempFile { get; set; }

        static CommandLine()
        {
            NumThreads = 12;
            Verbosity = Verbosity.Normal;
        }

        /// <summary>
        /// Parses command line arguments.
        /// 
        /// All error handling related to command-line arguments should be done here for the purposes of code quality.
        /// </summary>
        /// <param name="args">The arguments in the form of a string array.</param>
        /// <returns></returns>
        public static bool Parse(string[] args)
        {
            try
            {
                for (int i = 0; i < args.Length; i++)
                {
                    string curArg = args[i];

                    string nextArg = null;
                    if (args.Length - i > 1) nextArg = args[i + 1];

                    curArg = curArg.ToLower();

                    if (curArg.StartsWith("-"))
                    {
                        switch (curArg)
                        {
                            case "-start":
                            case "-s":
                                Start = Convert.ToUInt64(nextArg);
                                continue;
                            case "-end":
                            case "-e":
                                End = Convert.ToUInt64(nextArg);
                                continue;
                            case "-filename":
                            case "-f":
                                FileName = nextArg;
                                continue;
                            case "-imagesize":
                            case "-i":
                                ImageSize = nextArg;
                                continue;
                            case "-infile":
                            case "-in":
                                InFile = nextArg;
                                continue;
                            case "-outfile":
                            case "-out":
                            case "-o":
                                OutFile = nextArg;
                                continue;
                            case "-urloutfile":
                            case "-url":
                                UrlOutFile = nextArg;
                                continue;
                            case "-quiet":
                            case "-q":
                                Verbosity = Verbosity.Quiet;
                                continue;
                            case "-verbose":
                            case "-v":
                                Verbosity = Verbosity.Verbose;
                                continue;
                            case "-generatecsv":
                            case "-g":
                                GenerateCsv = true;
                                continue;
                            case "-numdownloads":
                            case "-threads":
                            case "-num":
                            case "-t":
                                NumThreads = Convert.ToInt32(nextArg);
                                continue;
                            case "-csvinfolder":
                            case "-ci":
                                CsvInFolder = nextArg;
                                continue;
                            case "-logtofile":
                            case "-log":
                            case "-l":
                                LogToFile = true;
                                continue;
                            case "-imagesizemin":
                            case "-imin":
                                // more concise than ulong Convert.ToHexString, as no byte array conversion is necessary
                                ImageSizeMin = ulong.Parse(nextArg, NumberStyles.HexNumber);
                                continue;
                            case "-imagesizemax":
                            case "-imax":
                                ImageSizeMax = ulong.Parse(nextArg, NumberStyles.HexNumber);
                                continue;
                            case "-dontdownload":
                            case "-d":
                                DontDownload = true;
                                continue;
                            case "-hextime":
                            case "-h":
                                HexTime = true;
                                continue;
                            case "-dontgeneratetempfile":
                            case "-dt":
                            case "-dtemp":
                                DontGenerateTempFile = true;
                                continue; 
                        }

                    }
                }

                if (!GenerateCsv) // non-massview mode
                {
                    // Check for valid start, end, and filename
                    if (Start <= 0
                        || End <= 0
                        || FileName == null)
                    {
                        return false;
                    }

                    // Check for invalid image size.
                    if (ImageSize == null)
                    {
                        if (ImageSizeMin == 0
                            || ImageSizeMax == 0)
                        {
                            return false;
                        }
                    }

                    // Check for invalid or DDOSing thread count options. 
                    if (NumThreads < 1
                        || NumThreads > 30)
                    {
                        return false; // don't DDOS the servers
                    }

                    // The user has specified they want hex time format, reconvert to it
                    if (HexTime)
                    {
                        string startString = Start.ToString();
                        string endString = End.ToString();  

                        Start = ulong.Parse(startString, NumberStyles.HexNumber);
                        End = ulong.Parse(endString, NumberStyles.HexNumber);
                    }

                    // check we specified infile
                    if (InFile != null
                        && !File.Exists(InFile))
                    {
                        return false;
                    }

                    // default filename
                    if (OutFile == null) OutFile = FileName;
                }
                else // massview mode
                {
                    if (CsvInFolder == null
                    || OutFile == null)
                    {
                        return false;
                    }
                }


                return true;
            }
            catch (Exception ex)
            {
                NCLogging.Log($"An error occurred parsing command-line arguments: {ex.Message}");

                if (Verbosity >= Verbosity.Verbose) NCLogging.Log($"\n\nStacktrace: {ex.StackTrace}");

                return false; 
            }
        }

        public static void ShowHelp()
        {
            PrintVersion(); 
            Console.WriteLine(Properties.Resources.Help);
        }

        public static void PrintVersion()
        {
            // this always succeeds as we set it to normal in the static constructor of CommandLine()
            if (Verbosity >= Verbosity.Normal)
            {
                // temp until nucore allows you to turn off function name
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{SymXVersion.SYMX_APPLICATION_NAME} {SymXVersion.SYMX_VERSION_EXTENDED_STRING}", ConsoleColor.Green);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("A Microsoft Symbol Server bulk download tool");
                Console.WriteLine("© 2022 starfrost");
            }
        }
    }
}
