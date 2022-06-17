﻿namespace SymX
{
    /// <summary>
    /// FileInformation
    /// 
    /// Holds file information and download metadata.
    /// </summary>
    public class FileInformation
    {

        /// <summary>
        /// The date this file was last modified on the Symbol Server.
        /// </summary>
        public DateTime LastModifiedDate { get; set; }

        /// <summary>
        /// File size of this file in bytes.
        /// </summary>
        public long FileSize { get; set; }
        
        /// <summary>
        /// The time this file took to download in milliseconds.
        /// </summary>
        public long DownloadTime { get; set; }

        /// <summary>
        /// The speed of this download in kilobytes per second.
        /// </summary>
        public double DownloadSpeed => ((double)FileSize / (double)DownloadTime);
    }
}
