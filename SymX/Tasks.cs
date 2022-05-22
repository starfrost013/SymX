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
        /// Parse a CSV file contaiing a list of URLs.
        /// </summary>
        ParseCsv = 7,

        /// <summary>
        /// Report download information
        /// </summary>
        Report = 8,

        /// <summary>
        /// Exit the application.
        /// </summary>
        Exit = 9
    }
}