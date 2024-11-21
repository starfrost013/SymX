
using System;
using System.Diagnostics;

namespace SymX
{
    /// <summary>
    /// FileDownloader
    /// 
    /// August 7, 2022
    /// 
    /// File downloader class for SymX
    /// </summary>
    public static class FileDownloader
    {
        public static bool DownloadListOfFiles(List<string> urlList)
        {
            try
            {
                int numOfRetries = 0; // number of retries for the current file
                int numFailedUrls = 0; // number of URLs that have failed
                int numDownloads = Configuration.NumDownloads; // number of simultaneous downloads

                List<Task<FileMetadata>> downloads = new List<Task<FileMetadata>>();

                if (Configuration.Verbosity >= Verbosity.Verbose) Console.Clear(); // clear console

                if (Configuration.Verbosity >= Verbosity.Normal) Logger.Log($"Downloading {urlList.Count} successful URLs...");

                // loop through each url set
                for (int curUrl = 0; curUrl < urlList.Count; curUrl += numDownloads)
                {
                    // Perform the download.

                    // Download every file, downloading (numDownloads) at once.
                    for (int curTask = 0; curTask < numDownloads; curTask++)
                    {
                        string url = urlList[curUrl];

                        // current URL ID in task
                        int curUrlWithinTask = curUrl + curTask;

                        string outFileName = null;

                        try
                        {
                            // don't try to download urls outside of the link
                            if (curUrlWithinTask < urlList.Count)
                            {
                                url = urlList[curUrl + curTask];

                                outFileName = GetOutFileName(curUrlWithinTask, urlList);

                                if (Configuration.Verbosity >= Verbosity.Normal) Logger.Log($"Downloading {url} to {outFileName}...");

                                Task<FileMetadata> downloadTask = Task<FileMetadata>.Run(() => DownloadSuccessfulFile(url, outFileName));
                                downloads.Add(downloadTask);
                            }

                            bool waiting = true;

                            while (waiting)
                            {
                                bool needToWait = false;

                                foreach (Task download in downloads)
                                {
                                    if (!download.IsCompleted) needToWait = true;
                                }

                                waiting = needToWait;
                            }
                        }
                        catch
                        {
                            if (numOfRetries >= Configuration.MaxRetries)
                            {
                                // reset the number of retries. we will skip the url by doing this
                                Logger.Log($"Reached {Configuration.MaxRetries} tries, giving up on {url}...", ConsoleColor.Red);
                                numFailedUrls++;
                                numOfRetries = 0;
                            }
                            else
                            {
                                numOfRetries++;
                                Logger.Log($"An error occurred while downloading. Retrying ({numOfRetries}/{Configuration.MaxRetries})...", ConsoleColor.Yellow);
                                // delete any partially downloaded files
                                if (File.Exists(outFileName)) File.Delete(outFileName);

                                // decrement curURL to retry the current URL
                                curUrl--;
                            }
                        }
                    }

                    for (int curDownloadTask = 0; curDownloadTask < downloads.Count; curDownloadTask++)
                    {
                        Task<FileMetadata> download = downloads[curDownloadTask];

                        FileMetadata metadata = download.Result;

                        // check if the download was successful
                        if (metadata.Successful)
                        {
                            if (Configuration.Verbosity >= Verbosity.Verbose)
                            {
                                bool printLastModifiedDate = false;

                                Logger.Log($"Metadata for file {metadata.FileName}:");

                                if (Configuration.IsMsdl())
                                {
                                    // Microsoft moved to an Azure-based symbol server in June of 2017, so dates before this date are completely invalid
                                    if (metadata.LastModifiedDate > new DateTime(2017, 06, 11, 23, 59, 59, 999))
                                    {
                                        printLastModifiedDate = true;
                                    }
                                    else
                                    {
                                        Logger.Log("Warning: Invalid last modified date - file was uploaded before Azure move!", ConsoleColor.Yellow);
                                    }
                                }
                                else
                                {
                                    printLastModifiedDate = true;
                                }

                                if (printLastModifiedDate 
                                    && Configuration.Verbosity >= Verbosity.Normal) Logger.Log($"Last modified date: {metadata.LastModifiedDate.ToString("yyyy-MM-dd HH:mm:ss")}");

                                downloads.Remove(download);
                                curDownloadTask--; // don't skip the next one
                            }

                            if (Configuration.Verbosity >= Verbosity.Normal) Logger.Log($"File size: {metadata.FileSize} (took {metadata.DownloadTime}ms to download, {metadata.DownloadSpeed.ToString("F1")} KB/s)");
                        }
                    }

                    // reset the number of retries for each file
                    numOfRetries = 0;

                    continue;
                }

                if (numFailedUrls > 0) Logger.Log($"{numFailedUrls}/{urlList.Count} URLs failed to download!", ConsoleColor.Yellow);

                return true;
            }
            catch (Exception ex)
            {
                Logger.Log($"A fatal error occurred while downloading files: {ex}", ConsoleColor.Red);
                return false;
            }
        }

        private static string GetOutFileName(int curUrl, List<string> urlList)
        {
            string url = urlList[curUrl];

            string outFileName = Configuration.OutFile;

            int duplicateFileNumber = 0;

            string[] fileNameSplit = url.Split('/');

            // get the last section of the path (the filename)
            string inFileName = fileNameSplit[fileNameSplit.Length - 1];

            // prevent downloading the same file several times 
            if (urlList.Count > 1)
            {
                duplicateFileNumber = curUrl + 1;

                outFileName = $"{duplicateFileNumber}_{inFileName}";
            }

            // Prepend the output folder.
            outFileName = $"{Configuration.OutFolder}\\{outFileName}";

            // Prevent files with the same number and filename in the folder overwriing each other.
            // If the filename exists, increment it.
            while (File.Exists(outFileName))
            {
                duplicateFileNumber++;
                outFileName = $"{Configuration.OutFolder}\\{duplicateFileNumber}_{inFileName}";
            }

            return outFileName;
        }

        private static FileMetadata DownloadSuccessfulFile(string url, string outFileName)
        {
            FileMetadata metadata = new FileMetadata();

            try
            {
                if (Configuration.Verbosity >= Verbosity.Verbose)
                {
                    // send a GET request
                    var stream = HttpManager.Client.GetAsync(url);

                    while (!stream.IsCompleted) { };

                    HttpResponseMessage message = stream.Result;

                    if (message.Content.Headers.LastModified != null)
                    {
                        DateTimeOffset dateTimeOffset = (DateTimeOffset)message.Content.Headers.LastModified;
                        DateTime lastModified = dateTimeOffset.UtcDateTime;

                        metadata.LastModifiedDate = lastModified;
                    }
                }

                Stopwatch downloadStopwatch = new Stopwatch();
                downloadStopwatch.Start();

                var downloadStream = HttpManager.Client.GetByteArrayAsync(url);

                while (!downloadStream.IsCompleted) { };

                downloadStopwatch.Stop();

                byte[] fileArray = downloadStream.Result;

                metadata.FileSize = fileArray.LongLength;
                metadata.DownloadTime = downloadStopwatch.ElapsedMilliseconds;
                metadata.FileName = outFileName;

                using (FileStream fileStream = new FileStream(outFileName, FileMode.Create))
                {
                    fileStream.Write(fileArray);
                }

                metadata.Successful = true;

                return metadata;
            }
            catch (Exception ex)
            {
                Logger.Log($"An exception occurred while downloading {url}!\n\n{ex.Message}");
                return metadata; //successful always false here
            }
        }
    }
}
