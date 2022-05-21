
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
        /// If true, the list of URLs will be dumped to this filename. 
        /// </summary>
        public static string UrlOutFile { get; set; }

        /// <summary>
        /// The image size to search for on the symbol server.
        /// </summary>

        public static string ImageSize { get; set; }

        /// <summary>
        /// A MassView-format CSV file containing a list of files, imagesizes, and TimeDateStamps to search for.
        /// </summary>

        public static string InFile { get; set; }

        /// <summary>
        /// If true, a CSV will be generated, then the program will exit.
        /// </summary>
        public static bool CsvGenerate { get; set; }

        /// <summary>
        /// The folder to input CSV files from. Ignored if <see cref="CsvGenerate"/> is not set to true.
        /// </summary>
        public static string CsvInFolder { get; set; }

        /// <summary>
        /// The output file to set the CSV file to. Ignored if <see cref="CsvOutFile"/> is not set to true.
        /// </summary>
        public static string CsvOutFile { get; set; }

        /// <summary>
        /// The verbosity level of the application.
        /// See <see cref="Verbosity"/> for extended description.
        /// </summary>
        public static Verbosity Verbosity { get; set; }

        /// <summary>
        /// Limit the number of requests per second so as to not throttle the Microsoft symbol servers.
        /// 30 is the default. Set to -1 for unlimited request speed (dangerous!)
        /// </summary>
        public static int Throttle { get; set; }

        /// <summary>
        /// Do not download files. Simply generate a list of URLs, optionally dump them to a file, and exit.
        /// </summary>
        public static bool DontDownload { get; set; }

        /// <summary>
        /// The number of files to download per second.
        /// </summary>
        public static int NumOfDownloadsAtOnce { get; set; }

        static CommandLine()
        {
            NumOfDownloadsAtOnce = 12;
            Throttle = 30;
            Verbosity = Verbosity.Normal;
        }

        public static bool Parse(string[] args)
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
                        case "-of":
                            OutFile = nextArg;
                            continue;
                        case "-urloutfile":
                        case "-uof":
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
                        case "-throttle":
                        case "-t":
                            Throttle = Convert.ToInt32(nextArg);
                            continue; 
                        case "-generatecsv":
                        case "-c":
                            CsvGenerate = true;
                            continue;
                        case "-numdownloads":
                        case "-n":
                            NumOfDownloadsAtOnce = Convert.ToInt32(nextArg);
                            continue;
                        case "-csvinfolder":
                        case "-ci":
                            CsvInFolder = nextArg;
                            continue;
                        case "-csvoutfile":
                        case "-co":
                            CsvOutFile = nextArg;
                            continue;
                        case "-dontdownload":
                        case "-d":
                            DontDownload = true;
                            continue; 
                    }
                    
                }
            }

            if (Start <= 0
                || End <= 0
                || FileName == null
                || ImageSize == null)
            {
                return false;
            }

            if (CsvGenerate)
            {
                if (CsvInFolder == null
                    || CsvOutFile == null)
                {
                    return false;
                }
            }

            if (NumOfDownloadsAtOnce < 0
                || NumOfDownloadsAtOnce > 30)
            {;
                return false; // don't DDOS the servers
            }

            // default filename
            if (OutFile == null) OutFile = FileName; 

            return true;
        }

        public static void ShowHelp()
        {
            PrintVersion(); 
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Help placeholder - remove before final release");
            Console.ForegroundColor = ConsoleColor.White; 
        }

        public static void PrintVersion()
        {
            // this always succeeds as we set it to normal in the static constructor of CommandLine()
            if (Verbosity >= Verbosity.Normal)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{SymXVersion.SYMX_APPLICATION_NAME} {SymXVersion.SYMX_VERSION_EXTENDED_STRING}");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("A Microsoft Symbol Server bulk download tool");
                Console.WriteLine("© 2022 starfrost");
            }
        }
    }
}
