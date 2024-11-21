namespace LightningUtil
{
    /// <summary>
    /// NCFile
    /// 
    /// File utilities.
    /// </summary>
    public static class FileUtils
    {
        private readonly static List<string> defaultExcludedPatterns = new()
        { ".tmp", "~$", ".g.cs", ".cache", ".editorconfig", ".props", ".targets", ".vsidx", ".lock", ".v1", ".v2", ".v5.", 
          "dgspec", "AssemblyAttributes", ".AssemblyInfo", "assets.json", ".suo", ".pdb", ".log", "test.wad", ".tlog", ".FileListAbsolute.txt", "BuildWithSkipAnalyzers",
          ".0\\apphost.exe" };

        /// <summary>
        /// Recursively copies files from one directory to another.
        /// </summary>
        /// <param name="sourceDir">The source directory to copy from.</param>
        /// <param name="destinationDir">The destination directory to copy from.</param>
        /// <param name="excludedPatterns">Patterns that are excluded</param>
        public static void RecursiveCopy(string sourceDir, string destinationDir = ".", List<string>? excludedPatterns = null)
        {
            // default exclude VS build artifacts
            excludedPatterns ??= defaultExcludedPatterns;

            foreach (string fileName in Directory.EnumerateFiles(sourceDir, "*", SearchOption.AllDirectories))
            {
                string relativeDestinationPath = fileName.Replace(sourceDir, "");
                // determine if we will copy
                bool performCopy = true;

                foreach (string excludedPattern in excludedPatterns)
                {
                    // skip any excluded pattern
                    if (fileName.Contains(excludedPattern, StringComparison.InvariantCultureIgnoreCase)) performCopy = false;
                }

                if (performCopy)
                {
                    string finalPath = $"{destinationDir}\\{relativeDestinationPath}";
                    string finalDirectory = finalPath[..finalPath.LastIndexOf(Path.DirectorySeparatorChar)];

                    if (!Directory.Exists(finalDirectory)) Directory.CreateDirectory(finalDirectory);
                    File.Copy(fileName, finalPath, true);
                }

            }
        }
    }
}