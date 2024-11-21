

namespace SymX
{
    /// <summary>
    /// CSVFile
    /// 
    /// Dumps required information for symbol server links to a CSV file.
    /// </summary>
    public static class CSVFile
    {
        /// <summary>
        /// PE magic bytes
        /// </summary>
        private static byte[] PEMagicData = { 0x50, 0x45 };

        /// <summary>
        /// Offset of e_lfanew
        /// </summary>
        private static byte e_lfanewOffset = 0x3C;

        /// <summary>
        /// Offset of the TimeDateStamp value, relative to the value found in <see cref="e_lfanewOffset"/>.
        /// </summary>
        private static byte TimeDateStampOffset = 0x08;

        /// <summary>
        /// Offset of the SizeOfImage value, relative to the value found in <see cref="e_lfanewOffset"/>.
        /// </summary>
        private static byte SizeOfImageOffset = 0x50;

        /// <summary>
        /// One-indexed column number of the URL column. CHANGE THIS IF LINE 55 CHANGES. 
        /// Temporary solution until a better rewrite?
        /// </summary>
        private static uint URL_COLUMN_NUMBER = 6;

        /// <summary>
        /// Runs a standard MassView scam
        /// </summary>
        /// <returns></returns>
        public static bool Run()
        {
            string inFolder = Configuration.CsvInFolder;
            string outFile = Configuration.OutFile;

            if (!Directory.Exists(inFolder))
            {
                Logger.Log($"The folder {inFolder} does not exist!", ConsoleColor.Red);
                return false;
            }
            else
            {
                if (Configuration.Verbosity >= Verbosity.Normal) Logger.Log($"CSV Export mode: Checking folder {inFolder}, dumping to {outFile}");

                StreamWriter bw = new StreamWriter(new FileStream(outFile, FileMode.Create));

                // Write the CSV elements
                bw.WriteLine("FileName,TimeDateStamp,ISO8601,Hex,SizeOfImage,Url");

                // set up the list of files to get files frmo
                string[] dirFiles = null;

                if (!Configuration.Recurse)
                { 
                    dirFiles = Directory.GetFiles(inFolder);
                }
                else
                {
                    // generate a list of files recursively
                    dirFiles = BuildListOfUrlsRecursively().ToArray();
                }

                foreach (string fileName in dirFiles)
                {
                    // list of file extensions that are usually PEs 
                    // .WinMD is Windows Metadata file, used for API metadata/exposing in Win8+ WinRT
                    // .cpls are Control Panel Applets, they are always PEs
                    // .scrs are screensavers, which are also PE DLLs
                    // .rlls are resource libraries
                    if (fileName.Contains(".exe")
                        || fileName.Contains(".dll")
                        || fileName.Contains(".sys")
                        || fileName.Contains(".ocx")
                        || fileName.Contains(".rll")
                        || fileName.Contains(".cpl")
                        || fileName.Contains(".scr")
                        || fileName.Contains(".winmd"))
                    {
                        using (BinaryReader br = new BinaryReader(File.OpenRead(fileName)))
                        {
                            // make sure the file is long enough to be a PE
                            // fix bug
                            if (br.BaseStream.Length > SizeOfImageOffset)
                            {
                                // read e_lfanew
                                br.BaseStream.Seek(e_lfanewOffset, SeekOrigin.Begin);

                                uint e_lfanew = br.ReadUInt32();

                                // check for the PE header 
                                // not exactly foolproof, but should work 99.99% of the time. and in worse case we will get invalid values and skip the file anyway
                                if (e_lfanew < br.BaseStream.Length - 4)
                                {
                                    br.BaseStream.Seek(e_lfanew, SeekOrigin.Begin);

                                    byte[] peMagic = br.ReadBytes(2);

                                    // Skip files that aren't Portable Executables
                                    if (peMagic[0] == PEMagicData[0]
                                        && peMagic[1] == PEMagicData[1])
                                    {
                                        br.BaseStream.Seek(e_lfanew + TimeDateStampOffset, SeekOrigin.Begin); // timestamp is at 0x08

                                        // convert the date to hex formats
                                        ulong timeDateStamp = br.ReadUInt64();

                                        if (timeDateStamp >= 253402218018 // 1/1/1970 to 12/31/9999 
                                            || timeDateStamp < 0) timeDateStamp = 0;

                                        DateTime date = new DateTime(1970, 1, 1, 1, 1, 1).AddSeconds(timeDateStamp);
                                        string dateIso = date.ToString("yyyy-MM-dd HH:mm:ss");
                                        string dateHex = timeDateStamp.ToString("x");

                                        br.BaseStream.Seek(e_lfanew + SizeOfImageOffset, SeekOrigin.Begin); // we don't need to distinguish between PE32 (x86) and PE32+ (x86-64) here, as the offsets just happen to line up where we need it

                                        uint sizeOfImage = br.ReadUInt32();
                                        string sizeOfImageHex = sizeOfImage.ToString("x");

                                        if (Configuration.Verbosity >= Verbosity.Verbose) Console.WriteLine($"{fileName}: {dateIso} (hex: {dateHex}, unix: {timeDateStamp}), ImageSize: {sizeOfImageHex}");

                                        // truncate the path so that we generate valid URLs
                                        string[] fileNameFolders = fileName.Split('\\');

                                        // i don't think there's any possible situation where there could NOT be slashes in this path
                                        string fileNameOnly = fileNameFolders[fileNameFolders.Length - 1];

                                        // 0x5C = \
                                        // This escapes the string so that Excel does not interpret any e000 as scientific notation
                                        string finalString = $"{fileName},{timeDateStamp},{dateIso},{dateHex},{sizeOfImageHex},{Configuration.SymbolServerUrl}/{fileNameOnly}/{dateHex}{sizeOfImageHex}/{fileNameOnly}";

                                        if (outFile != null) bw.WriteLine(finalString);
                                    }
                                }
                            }

                        }
                    }
                }

                bw.Close();

                Logger.Log($"Successfully wrote CSV file to: {outFile}!", ConsoleColor.Green);

                return true;
            }
        }

        private static List<string> BuildListOfUrlsRecursively(string directory = null, List<string> urls = null)
        {
            if (urls == null) urls = new List<string>();
            if (directory == null) directory = Configuration.CsvInFolder;

            foreach (string fileName in Directory.GetFiles(directory))
            {
                urls.Add(fileName);
            }

            foreach (string dirName in Directory.GetDirectories(directory))
            {
                BuildListOfUrlsRecursively(dirName, urls);
            }

            return urls; 
        }

        public static List<string> ParseUrls(string csvFile)
        {
            List<string> urls = new List<string>();

            string[] csvLines = File.ReadAllLines(csvFile);

            int numRejectedLines = 0;

            // skip the first line (CSV header) by starting at 1
            for (int curLine = 1; curLine < csvLines.Length; curLine++)
            {
                string csvLine = csvLines[curLine];

                string[] csvLineSections = csvLine.Split(',');

                if (csvLineSections.Length < URL_COLUMN_NUMBER)
                {
                    if (Configuration.Verbosity >= Verbosity.Normal)
                    {
                        Logger.Log($"Warning: Rejected CSV line {curLine} as it does not have URL section", ConsoleColor.Yellow);
                        numRejectedLines++;
                    }
                }
                else
                {
                    string csvLineUrl = csvLineSections[URL_COLUMN_NUMBER - 1];

                    if (Configuration.Verbosity >= Verbosity.Verbose) Logger.Log($"Found URL: {csvLineUrl}");

                    urls.Add(csvLineUrl);
                }
            }

            // take away 1 as we don't count the first line
            if (numRejectedLines > 0 && Configuration.Verbosity >= Verbosity.Normal) Logger.Log($"Warning: {numRejectedLines + 1} of {csvLines.Length - 1} lines (excluding the first line) did not have a valid URL column and were skipped!", ConsoleColor.Yellow);
            return urls;
        }
    }
}