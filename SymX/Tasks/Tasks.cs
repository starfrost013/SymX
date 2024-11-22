namespace SymX
{
    /// <summary>
    /// Tasks
    /// 
    /// May 16, 2022
    /// 
    /// Defines the list of tasks that the state machine component of SymX can perform.
    /// </summary>
    public enum Tasks
    {
        /// <summary>
        /// Generate a list of URLs.
        /// </summary>
        GenerateListOfUrls = 0,

        /// <summary>
        /// Find successful URLs (HEAD request). Goes to Download Files.
        /// </summary>
        TryDownload = 1,

        /// <summary>
        /// Generate a CSV file.
        /// </summary>
        GenerateCsv = 5,

        /// <summary>
        /// Load a URL list from a file.
        /// </summary>
        LoadListOfUrls = 6,

        /// <summary>
        /// Report download metadata to a HTML file 
        /// </summary>
        Report = 8,

        /// <summary>
        /// Clear any log files that may exist
        /// </summary>
        ClearLogs = 9,

        /// <summary>
        /// Parse a /000admin directory.
        /// </summary>
        Parse000Admin = 10,

        /// <summary>
        /// Parse a CSV file.
        /// </summary>
        ParseCsv = 12,

        /// <summary>
        /// Exit the application.
        /// </summary>
        Exit = 13
    }
}