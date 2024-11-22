using System;

namespace SymX
{
    /// <summary>
    /// Mode
    /// 
    /// Determines modes used for command-line syntax parsing.
    /// </summary>
    public enum SearchMode
    {
        /// <summary>
        /// Do not search.
        /// Used for displaying help.
        /// </summary>
        None = 0,

        /// <summary>
        /// Bruteforce mode -
        /// Bruteforces URLs to find files that exist.
        /// </summary>
        Bruteforce = 1,

        /// <summary>
        /// Default mode when no mode is specified.
        /// </summary>
        Default = Bruteforce,

        /// <summary>
        /// Generate a CSV from an output folder
        /// </summary>
        CsvExport = 2,

        /// <summary>
        /// Import a CSV file and download the URLs it contains.
        /// </summary>
        CsvImport = 3,

        /// <summary>
        /// Parse a /000admin folder and download the files in the transaction folders.
        /// </summary>
        Parse000Admin = 4,
    }
}
