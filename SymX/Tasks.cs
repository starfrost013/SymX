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
        GenerateListOfUrls = 0,

        TryDownload = 1,

        DownloadFiles = 2,

        FatalError = 3,

        SuccessfulDownload = 4,

        GenerateCsv = 5,

        GenerateUrlList = 6,

        ParseCsv = 7,

        Exit = 8
    }
}