using NuCore.Utilities; 

namespace SymX
{
    public static class MassView
    {
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
                StreamWriter bw = null;

                bw = new StreamWriter(new FileStream(outFile, FileMode.Create));
                bw.WriteLine("FileName,TimeDateStamp,ISO8601,Hex,SizeOfImage"); // write csv elements

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
                        using (BinaryReader br = new BinaryReader(new FileStream(fileName, FileMode.Open)))
                        {
                            // read e_lfanew
                            br.BaseStream.Seek(0x3C, SeekOrigin.Begin);

                            uint e_lfanew = br.ReadUInt32();

                            // check for the PE header 

                            if (e_lfanew < br.BaseStream.Length - 4)
                            {
                                br.BaseStream.Seek(e_lfanew, SeekOrigin.Begin);

                                byte[] peMagic = br.ReadBytes(2);

                                // skip MZ/NE files in 32bit. skip others./
                                if (peMagic[0] == 0x50
                                    && peMagic[1] == 0x45)
                                {
                                    br.BaseStream.Seek(e_lfanew + 0x08, SeekOrigin.Begin); // timestamp is at 0x08

                                    long timeDateStamp = br.ReadInt64();
                                    DateTime date = new DateTime(1970, 1, 1, 1, 1, 1).AddSeconds(timeDateStamp);
                                    string dateIso = date.ToString("yyyy-MM-dd HH:mm:ss");
                                    string dateHex = timeDateStamp.ToString("x");

                                    br.BaseStream.Seek(e_lfanew + 0x50, SeekOrigin.Begin); // we don't need to distinguish between PE32 (x86) and PE32+ (x86-64) here, as it just happens to line up where we need it

                                    uint sizeOfImage = br.ReadUInt32();
                                    string sizeOfImageHex = sizeOfImage.ToString("x");

                                    if (CommandLine.Verbosity >= Verbosity.Verbose) Console.WriteLine($"{fileName}: {dateIso} (hex: {dateHex}, unix: {timeDateStamp}), imagesize: {sizeOfImageHex}");

                                    if (outFile != null) bw.WriteLine($"{fileName},{timeDateStamp},{dateIso},{dateHex},{sizeOfImageHex}");
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