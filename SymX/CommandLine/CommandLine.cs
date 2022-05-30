using NuCore.Utilities;
using System.Globalization;

namespace SymX
{
    /// <summary>
    /// CommandLine
    /// 
    /// Defines the command-line options that SymX can use,
    /// and methods to parse them.
    /// </summary>
    public static class CommandLine
    {
        /// <summary>
        /// The start (in 64-bit unix time format) time to scan for files in.
        /// </summary>
        public static ulong Start { get; set; }

        /// <summary>
        /// The end (in 64-bit unix time format) time to scan for files to.
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

        /// <summary>
        /// The maximum download retries permissible before a file fails.
        /// </summary>
        public static uint MaxRetries { get; set; }

        /// <summary>
        /// Output folder for downloaded files. Default is /download
        /// </summary>
        public static string OutFolder { get; set; }

        /// <summary>
        /// If not <c>null</c>, the user-agent vendor string will be overridden with this value.
        /// </summary>
        public static string UserAgentVendor { get; set; }

        /// <summary>
        /// If not <c>null</c>, the user-agent version string will be overridden with this value.
        /// </summary>
        public static string UserAgentVersion { get; set; }

        /// <summary>
        /// Optional overriding of the symbol server url
        /// </summary>
        public static string SymbolServerUrl { get; set; }

        #region Defaults
        /// <summary>
        /// Private: Default user agent vendor string to use while sending requests.
        /// </summary>

        private static string DEFAULT_UA_VENDOR = "Microsoft-Symbol-Server";

        /// <summary>
        /// Private: Default user agent version string to use while sending requests.
        /// </summary>

        private static string DEFAULT_UA_VERSION = "10.1710.0.0";

        /// <summary>
        /// Private: The default symbol server URL
        /// </summary>
        private static string DEFAULT_SYMSRV_URL = "https://msdl.microsoft.com/download/symbols";

        /// <summary>
        /// Private: The default output folder.
        /// </summary>
        private static string DEFAULT_OUTPUT_FOLDER = "download";
        #endregion

        /// <summary>
        /// Constructor for <see cref="CommandLine"/> that sets up the default values.
        /// </summary>
        static CommandLine()
        {
            // Set up default values
            NumThreads = 12;
            MaxRetries = 8;

            Verbosity = Verbosity.Normal;

            UserAgentVendor = DEFAULT_UA_VENDOR;
            UserAgentVersion = DEFAULT_UA_VERSION;

            OutFolder = DEFAULT_OUTPUT_FOLDER;

            SymbolServerUrl = DEFAULT_SYMSRV_URL;
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
                            case "-maxretries":
                            case "-retries":
                            case "-max":
                            case "-m":
                                MaxRetries = Convert.ToUInt32(nextArg);
                                continue;
                            case "-outfolder":
                            case "-of":
                                OutFolder = nextArg;
                                continue;
                            case "-useragentvendor":
                            case "-uavendor":
                                UserAgentVendor = nextArg;
                                continue;
                            case "-useragentversion":
                            case "-uaversion":
                                UserAgentVersion = nextArg;
                                continue;
                            case "-symbolserverurl":
                            case "-symsrvurl":
                            case "-symsrv":
                                SymbolServerUrl = nextArg;
                                continue;
                        }
                    }
                }

                if (OutFolder != null
                && !Directory.Exists(OutFolder)) Directory.CreateDirectory(OutFolder);

                if (InFile != null)
                {
                    if (!File.Exists(InFile))
                    {
                        Console.WriteLine($"-infile: The file {InFile} does not exist!");
                        return false;
                    }

                    return true;
                }

                if (!GenerateCsv) // non-massview mode
                {
                    // Check for valid start, end, and filename
                    if (Start <= 0
                        || End <= 0)
                    {
                        Console.WriteLine("-start or end: Required option not present!");
                        return false;
                    }

                    if (FileName == null)
                    {
                        Console.WriteLine("-filename: Required option not present");
                        return false;
                    }

                    // Check for invalid image size.
                    if (ImageSize == null)
                    {
                        if (ImageSizeMin == 0
                            || ImageSizeMax == 0)
                        {
                            Console.WriteLine("Either -imagesize or both -imagesizemin and -imagesizemax must be present!");
                            return false;
                        }
                    }

                    // Check for invalid or DDOSing thread count options. 
                    if (NumThreads < 1
                        || NumThreads > 30)
                    {
                        Console.WriteLine("-numthreads: must be between 1-30 - no DDOSing the servers!");
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

                    // Only allow official DbgX user agent with official symsrv
                    if (SymbolServerUrl == DEFAULT_SYMSRV_URL)
                    {
                        UserAgentVendor = DEFAULT_UA_VENDOR;
                        UserAgentVersion = DEFAULT_UA_VERSION;
                    }

                    // default filename
                    if (OutFile == null) OutFile = FileName;
                }
                else // massview mode
                {
                    if (CsvInFolder == null
                    || OutFile == null)
                    {
                        Console.WriteLine("-csvinfolder and -outfile: both must be provided if -generatecsv is provided!");
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                NCLogging.Log($"An error occurred while parsing command-line arguments: {ex.Message}", ConsoleColor.Red);

                if (Verbosity >= Verbosity.Verbose) NCLogging.Log($"\n\nStacktrace: {ex.StackTrace}", ConsoleColor.Red);

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
                Console.WriteLine($"{SymXVersion.SYMX_APPLICATION_NAME} {SymXVersion.SYMX_VERSION_EXTENDED_STRING}");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("A Microsoft Symbol Server bulk download tool");
                Console.WriteLine("© 2022 starfrost");
            }
        }
    }
}
