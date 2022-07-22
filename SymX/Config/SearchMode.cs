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
        /// Bruteforce mode -
        /// Bruteforces URLs to find files that exist.
        /// </summary>
        Bruteforce = 0,

        /// <summary>
        /// Default mode when no mode is specified.
        /// </summary>
        Default = Bruteforce,

        /// <summary>
        /// Generate a CSV from an output folder
        /// </summary>
        CsvExport = 1,

        /// <summary>
        /// Import a CSV file and download the URLs it contains.
        /// </summary>
        CsvImport = 2,

        /// <summary>
        /// Parse a /000admin folder and download the files in the transaction folders.
        /// </summary>
        ParseAdmin = 3,

        /// <summary>
        /// Download an image file from a PDB file.
        /// </summary>
        PdbFile = 4,
    }
}
