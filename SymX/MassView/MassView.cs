using NuCore.Utilities; 

namespace SymX
{
    /// <summary>
    /// MassView 2.0
    /// 
    /// Dumps required information for symsrv links to a file.
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
                bw.WriteLine("FileName,TimeDateStamp,ISO8601,Hex,SizeOfImage,URL"); // write csv elements

                string[] dirFiles = Directory.GetFiles(inFolder);

                foreach (string fileName in dirFiles)
                {
                    // list of file extensions (all of these are PEs)
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

                                // skip MZ/NE files in 32bit. skip other files.
                                if (peMagic[0] == PEMagicData[0]
                                    && peMagic[1] == PEMagicData[1])
                                {
                                    br.BaseStream.Seek(e_lfanew + TimeDateStampOffset, SeekOrigin.Begin); // timestamp is at 0x08

                                    ulong timeDateStamp = br.ReadUInt64();
                                    DateTime date = new DateTime(1970, 1, 1, 1, 1, 1).AddSeconds(timeDateStamp);
                                    string dateIso = date.ToString("yyyy-MM-dd HH:mm:ss");
                                    string dateHex = timeDateStamp.ToString("x");

                                    br.BaseStream.Seek(e_lfanew + SizeOfImageOffset, SeekOrigin.Begin); // we don't need to distinguish between PE32 (x86) and PE32+ (x86-64) here, as it just happens to line up where we need it

                                    uint sizeOfImage = br.ReadUInt32();
                                    string sizeOfImageHex = sizeOfImage.ToString("x");

                                    if (CommandLine.Verbosity >= Verbosity.Verbose) Console.WriteLine($"{fileName}: {dateIso} (hex: {dateHex}, unix: {timeDateStamp}), imagesize: {sizeOfImageHex}");

                                    if (outFile != null) bw.WriteLine($"{fileName},{timeDateStamp},{dateIso},{dateHex},{sizeOfImageHex},https://msdl.microsoft.com/download/symbols/{fileName}/{dateHex}{sizeOfImageHex}/{fileName}");
                                }
                            }
                        }
                    }
                }

                bw.Close();

                NCLogging.Log($"Written CSV file to: {outFile}!", ConsoleColor.Green);

                return true; 
            }
        }
    }
}