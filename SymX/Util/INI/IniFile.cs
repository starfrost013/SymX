namespace SymX
{
    /// <summary>
    /// IniFile
    /// 
    /// NuCore INI parser
    /// Pretty simple - simply uses the first character to determine token type
    /// 
    /// <para>Written February 2022</para>
    /// <para>Updated July 2, 2022 in order to handle comments on the same line as values, handle newlines and rename variables to camelCase</para>
    /// <para>Updated August 2, 2022 to fix a bug with INI comments on the same line as values in non-section lines, as well as to make searches case-insensitive.</para>
    /// <para>Updated August 9, 2022 to add serialisation to file.</para>
    /// <para>Updated January 15, 2023 to add nullable support,</para>
    /// <para>Updated February 11, 2023 to fix case insensitivity consistency between different methods, and to add optional case insensitivity while searching for sections or values.</para>
    /// <para>Updated February 22, 2023 to rename Write to Save and to rename some variables, as well as removing the "NC" prefix from the classes.</para>
    /// </summary>
    public class IniFile
    {
        /// <summary>
        /// The sections of this INI file. 
        /// </summary>
        public List<IniSection> Sections { get; set; }

        /// <summary>
        /// Parser private: used for parsing the current section.
        /// </summary>
        private IniSection? CurSection { get; set; }

        public IniFile()
        {
            Sections = new List<IniSection>();
        }

        /// <summary>
        /// Parses an INI file at <paramref name="path"/>.
        /// </summary>
        /// <param name="path">The path of the INI file that is to be parsed.</param>
        /// <returns></returns>
        /// <exception cref="NCError">An error occurred during the INI parsing. Extended error information is present in the <see cref="NCError.Description"/> property.</exception>
        public static IniFile? Parse(string path)
        {
            if (!File.Exists(path)) Logger.LogError($"INI parsing error: Cannot parse INI file at {path}: File not found!", 21,
                LoggerSeverity.Error);

            IniFile iniFile = new();

            try
            {
                StreamReader iniStream = new(new FileStream(path, FileMode.Open));

                while (!iniStream.EndOfStream)
                {
                    string? iniLine = iniStream.ReadLine();

                    Debug.Assert(iniLine != null);

                    // trim leading and trailing spaces
                    // so first character detection works
                    iniLine = iniLine.Trim();

                    if (iniLine.Length > 0)
                    {
                        char firstCharOfLine = iniLine[0];

                        // Handle comments on the same line after the value
                        string[] iniLineComments = iniLine.Split(';');

                        // if there ARE comments...
                        if (firstCharOfLine != ';'
                            && iniLineComments.Length > 1)
                        {
                            // if the line starts with a ; we will have already ignored it earlier
                            // so simply cut off the comments
                            iniLine = iniLineComments[0];
                            iniLine = iniLine.Trim(); // trim again
                        }

                        // Check potential starts of a line to determine what type of INI content we are referencing
                        switch (firstCharOfLine)
                        {
                            case '[': // Section
                                if (iniLine.Contains(']', StringComparison.InvariantCultureIgnoreCase))
                                {
                                    int beginning = iniLine.IndexOf('[');
                                    int end = iniLine.IndexOf(']');

                                    if (beginning > end)
                                    {
                                        Logger.LogError("INI parsing error: Invalid section entry - ] before [!", 25, LoggerSeverity.Error);
                                        return null;
                                    }

                                    // we add and remove 1 so that the [ and ] markers don't become part of the file name
                                    // trim to remove leading spaces
                                    string sectionName = iniLine.Substring(beginning + 1, iniLine.Length - (iniLine.Length - end) - 1);
                                    sectionName = sectionName.Trim();
                                    
                                    IniSection iniSection = new(sectionName);

                                    iniFile.Sections.Add(iniSection);
                                    iniFile.CurSection = iniSection;
                                }
                                else
                                {
                                    Logger.LogError("INI parsing error: Section name must terminate with ]!", 24, LoggerSeverity.Error);
                                    return null;
                                }
                                continue;
                            case ';' or '\n' or '\r': // Comment
                                continue;
                            default: // Value
                                if (iniLine.Contains('=', StringComparison.InvariantCultureIgnoreCase))
                                {
                                    if (iniFile.CurSection == null)
                                    {
                                        Logger.LogError("INI parsing error: Values must be within a section!", 26, LoggerSeverity.Error);
                                        return null;
                                    }

                                    string[] iniValue = iniLine.Split('=');

                                    // we just ignore anything after the first one. this also works for comments that are on the same line
                                    string iniValueKey = iniValue[0];
                                    string iniValueValue = iniValue[1];

                                    // strip characters that indicate strings

                                    iniValueValue = iniValueValue.Replace("\"", "");

                                    // trim to get rid of leading spaces etc
                                    iniValueKey = iniValueKey.Trim();
                                    iniValueValue = iniValueValue.Trim();

                                    // add it to the values
                                    iniFile.CurSection.Values.Add(iniValueKey, iniValueValue);
                                }
                                else
                                {
                                    Logger.LogError("INI parsing error: An INI item with no value was found!", 23, LoggerSeverity.Error);
                                    return null;
                                }
                                continue;
                        }
                    }

                }

                // in case of success, close the stream and return
                // close the stream
                iniStream.Close();
                return iniFile;
            }
            catch (Exception ex)
            {
                Logger.LogError($"INI parsing error: Cannot parse INI file at {path}: \n\n{ex}", 22, LoggerSeverity.Error);
                return null;
            }
        }

        /// <summary>
        /// Writes the content of this <see cref="IniFile"/> to a physical .ini file.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool Save(string path)
        {
            List<string> iniLines = new();

            foreach (IniSection section in Sections)
            {
                iniLines.Add($"[{section.Name}]");

                foreach (KeyValuePair<string, string> item in section.Values)
                {
                    iniLines.Add($"{item.Key} = \"{item.Value}\"");
                }
            }

            string[] iniLineArray = iniLines.ToArray();

            // write to file

            try
            {
                File.WriteAllLines(path, iniLineArray);
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error writing to INI: {ex.Message}", 110, LoggerSeverity.Error);
                return false;
            }
        }

        /// <summary>
        /// Acquires the INI file section with the name <paramref name="name"/>
        /// </summary>
        /// <param name="name">The name of the INI file section you wish to obtain.</param>
        /// <returns>A <see cref="IniSection"/> instance representing the INI file with section <see cref="Name"/> if the section could be found, otherwise null.</returns>
        public IniSection? GetSection(string name, bool caseSensitive = false)
        {
            foreach (IniSection iniSection in Sections)
            {
                // this ensures case-insensitive comparison while using less code than string.Equals();
                if (!caseSensitive)
                {
                    iniSection.Name = iniSection.Name.ToLowerInvariant();
                    name = name.ToLowerInvariant();
                }

                if (iniSection.Name == name) return iniSection;
            }

            return null;
        }
    }
}
