using NuCore.Utilities; 

namespace SymX
{
    /// <summary>
    /// MassView 2.1
    /// 
    /// Dumps required information for Microsoft Symbol Server links to a file.
    /// </summary>
    public static class MassView
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

        public static bool Run()
        {
            string inFolder = CommandLine.CsvInFolder;
            string outFile = CommandLine.OutFile;

            if (!Directory.Exists(inFolder))
            {
                NCLogging.Log($"The folder {inFolder} does not exist!", ConsoleColor.Red);
                return false; 
            }
            else
            {
                if (CommandLine.Verbosity >= Verbosity.Verbose) NCLogging.Log($"MassView: Checking folder {inFolder}, dumping to {outFile}");
                StreamWriter bw = null;

                bw = new StreamWriter(new FileStream(outFile, FileMode.Create));

                // Write the CSV elements
                bw.WriteLine("FileName,TimeDateStamp,ISO8601,Hex,SizeOfImage,URL");

                string[] dirFiles = Directory.GetFiles(inFolder);

                foreach (string fileName in dirFiles)
                {
                    // list of file extensions that are usually PEs 
                    // .WinMD is Windows Metadata file, used for API metadata/exposing in Win8+ WinRT
                    // .cpls are Control Panel Applets, they are always PEs
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
                            // read e_lfanew
                            br.BaseStream.Seek(e_lfanewOffset, SeekOrigin.Begin);

                            uint e_lfanew = br.ReadUInt32();

                            // check for the PE header 

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
                                    DateTime date = new DateTime(1970, 1, 1, 1, 1, 1).AddSeconds(timeDateStamp);
                                    string dateIso = date.ToString("yyyy-MM-dd HH:mm:ss");
                                    string dateHex = timeDateStamp.ToString("x");

                                    br.BaseStream.Seek(e_lfanew + SizeOfImageOffset, SeekOrigin.Begin); // we don't need to distinguish between PE32 (x86) and PE32+ (x86-64) here, as the offsets just happen to line up where we need it

                                    uint sizeOfImage = br.ReadUInt32();
                                    string sizeOfImageHex = sizeOfImage.ToString("x");

                                    if (CommandLine.Verbosity >= Verbosity.Verbose) Console.WriteLine($"{fileName}: {dateIso} (hex: {dateHex}, unix: {timeDateStamp}), ImageSize: {sizeOfImageHex}");

                                    // truncate the path so that we generate valid URLs
                                    string[] fileNameFolders = fileName.Split('\\');

                                    // i don't think there's any possible situation where there could NOT be slashes in this path
                                    string fileNameOnly = fileNameFolders[fileNameFolders.Length - 1];

                                    if (outFile != null) bw.WriteLine($"{fileName},{timeDateStamp},{dateIso},{dateHex},{sizeOfImageHex},https://msdl.microsoft.com/download/symbols/{fileNameOnly}/{dateHex}{sizeOfImageHex}/{fileNameOnly}");
                                }
                            }
                        }
                    }
                }

                bw.Close();

                NCLogging.Log($"Successfully wrote CSV file to: {outFile}!", ConsoleColor.Green);

                return true; 
            }
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
                    if (CommandLine.Verbosity >= Verbosity.Normal)
                    {
                        NCLogging.Log($"Warning: Rejected line {curLine} as it does not have URL section", ConsoleColor.Yellow);
                        numRejectedLines++; 
                    }
                }
                else
                {
                    string csvLineUrl = csvLineSections[URL_COLUMN_NUMBER - 1];

                    if (CommandLine.Verbosity >= Verbosity.Verbose) NCLogging.Log($"Found URL: {csvLineUrl}");

                    urls.Add(csvLineUrl);
                }
            }

            // take away 1 as we don't count the first line
            if (numRejectedLines > 0 && CommandLine.Verbosity >= Verbosity.Normal) NCLogging.Log($"Warning: {numRejectedLines + 1} of {csvLines.Length - 1} lines (excluding the first line) did not have a valid URL column and were skipped!", ConsoleColor.Yellow);
            return urls;
        }
    }
}