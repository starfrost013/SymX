using NuCore.Utilities;
using System;
using System.ComponentModel;
using System.Diagnostics; 
using System.Net.Http;
using System.Net.Http.Headers;
 
namespace SymX
{
    /// <summary>
    /// TaskManager
    /// 
    /// SymX's state machine - handles all of the tasks that SymX must perform
    /// </summary>
    public static class TaskManager
    {
        /// <summary>
        /// The list of tasks that need to run in this session.
        /// </summary>
        public static List<Tasks> TaskList { get; set; }
        
        /// <summary>
        /// Private: List of generated URLs
        /// </summary>
        private static List<string> UrlList { get; set; }

        /// <summary>
        /// Private: HTTP client used for sending requests
        /// </summary>
        private static HttpClient httpClient = new HttpClient();

        /// <summary>
        /// Private: Timer used for measuring how long a download took
        /// </summary>
        private static Stopwatch timer = new Stopwatch();

        /// <summary>
        /// Magic number for how much Microsoft pads their SizeOfImage values to.
        /// </summary>
        private static ulong IMAGESIZE_PADDING = 0x1000;

        /// <summary>
        /// Default filename used for logging successful URLs.
        /// </summary>
        private static string DEFAULT_TEMP_FILE_NAME = "SuccessfulURLs.log";

        static TaskManager()
        {
            TaskList = new List<Tasks>();
            UrlList = new List<string>(); 
        }

        public static void GenerateListOfTasks()
        {
            if (CommandLine.Verbosity == Verbosity.Verbose) Console.WriteLine("Initialising HTTP client...");

            if (!CommandLine.GenerateCsv)
            {
                TaskList.Add(Tasks.GenerateListOfUrls);
            }
            else
            {
                TaskList.Add(Tasks.GenerateCsv);
            }
            
            if (!CommandLine.DontDownload
                && !CommandLine.GenerateCsv) TaskList.Add(Tasks.TryDownload);

            TaskList.Add(Tasks.Exit);
        }

        public static bool Run()
        {
            int numTasks = TaskList.Count;
            int curTask = 1;

            // perform each task in sequence
            for (int i = 0; i < TaskList.Count; i++)
            {
                Tasks currentTask = TaskList[i];

                string taskString = $"Performing task {curTask}/{numTasks} ({currentTask})...";

                Console.Title = $"{SymXVersion.SYMX_APPLICATION_NAME} - {taskString}";

                if (CommandLine.Verbosity >= Verbosity.Normal) NCLogging.Log(taskString);
                curTask++; 

                switch (currentTask)
                {
                    // Generate a list of URLs
                    case Tasks.GenerateListOfUrls:
                        UrlList = GenerateUrlList();
                        continue;
                    case Tasks.TryDownload: 
                        List<string> successfulUrls = TryDownload();
                        if (successfulUrls.Count > 0)
                        {
                            if (!DownloadSuccessfulFiles(successfulUrls)) NCLogging.Log("An error occurred downloading files!\n", ConsoleColor.Red);
                        }
                        continue;
                    case Tasks.GenerateCsv:
                        if (CommandLine.CsvInFolder == null) // generate a csv folder and then download
                        {
                            Console.WriteLine("This feature (generating file list from command line) is not yet implemented");
                        }
                        else
                        {
                            if (!MassView.Run()) 
                            {
                                NCLogging.Log("MassView failed to generate CSV file!", ConsoleColor.Red);
                            }
                        }
                        continue; 
                    case Tasks.Exit:
                        // Exit the program.
                        Environment.Exit(0);
                        continue; 
                }

            }

            return (TaskList.Count > 0);  // if there are no remaining tasks return false.
        }

        private static List<string> GenerateUrlList()
        {
            List<string> urlList = new List<string>();

            if (CommandLine.InFile == null)
            {
                if (CommandLine.ImageSizeMin == 0
                    || CommandLine.ImageSizeMax == 0)
                {
                    for (ulong curTime = CommandLine.Start; curTime < CommandLine.End; curTime++)
                    {
                        string fileUrl = $"https://msdl.microsoft.com/download/symbols/{CommandLine.FileName}/{curTime.ToString("x")}{CommandLine.ImageSize}/{CommandLine.FileName}";
                        if (CommandLine.Verbosity >= Verbosity.Verbose) Console.WriteLine(fileUrl);
                        urlList.Add(fileUrl);
                    }
                }
                else
                {
                    ulong imageSizeMin = CommandLine.ImageSizeMin;
                    ulong imageSizeMax = CommandLine.ImageSizeMax;

                    for (ulong curTime = CommandLine.Start; curTime < CommandLine.End; curTime++)
                    {
                        for (ulong curImageSize = imageSizeMin; curImageSize <= imageSizeMax; curImageSize += IMAGESIZE_PADDING)
                        {
                            string fileUrl = $"https://msdl.microsoft.com/download/symbols/{CommandLine.FileName}/{curTime.ToString("x")}{curImageSize.ToString("x")}/{CommandLine.FileName}";
                            if (CommandLine.Verbosity >= Verbosity.Verbose) Console.WriteLine(fileUrl);
                            urlList.Add(fileUrl);
                        }
                    }
                }

            }
            else
            {
                // massview
                return MassView.ParseUrls(CommandLine.InFile);
            }

            return urlList; 
        }

        /// <summary>
        /// Tries to download the selected URLs using HEAD requests. 
        /// </summary>
        /// <returns>A list of the URLs that successfully downloaded.</returns>
        private static List<string> TryDownload()
        {
            if (CommandLine.Verbosity >= Verbosity.Verbose) NCLogging.Log("Initialising HttpClient...");

            // initialise the http client (we already instantiate it as a private field)
            // this is so we don't have to add a check for dontdownload
            httpClient.BaseAddress = new Uri("https://msdl.microsoft.com");

            // Fake a DbgX UA.
            // Just in case (thanks pivotman319)
            httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Microsoft-Symbol-Server", "10.1710.0.0"));

            StreamWriter tempFile = null;

            // create a temporary file if the user has not explicitly specified to do this
            if (!CommandLine.DontGenerateTempFile)
            {
                tempFile = new StreamWriter(new FileStream(DEFAULT_TEMP_FILE_NAME, FileMode.OpenOrCreate));
            }

            if (CommandLine.Verbosity >= Verbosity.Quiet) NCLogging.Log($"Trying {UrlList.Count} URLs...");

            timer = Stopwatch.StartNew();

            List<string> successfulUrls = new List<string>();

            int noDownloadsAtOnce = CommandLine.NumThreads;
            int failedUrls = 0; // The number of failed URLs

            // create a list of tasks
            // consider having it return the url instead
            List<Task<bool>> tasks = new List<Task<bool>>();

            for (int i = 0; i < UrlList.Count; i += noDownloadsAtOnce)
            {
                // Set up a batch of downloads (default 12, ~numdownloads)
                for (int j = 0; j < noDownloadsAtOnce; j++)
                {
                    int curUrlId = i + j;
                    
                    if (curUrlId < UrlList.Count)
                    {
                        string curUrl = UrlList[i + j];
                        if (CommandLine.Verbosity >= Verbosity.Verbose) NCLogging.Log($"Trying URL {curUrl}...");
                        Task<bool> worker = Task<bool>.Run(() => CheckFileExists(curUrl));
                        tasks.Add(worker);
                    }
                }

                // wait for all current downloads to complete
                bool waiting = true;

                while (waiting)
                {
                    // will exit if all tasks complete
                    bool needToWait = false; 

                    for (int curTask = 0; curTask < tasks.Count; curTask++)
                    {
                        Task<bool> task = tasks[curTask];

                        // we need to wait as not every task is done
                        if (!task.IsCompleted) needToWait = true;
                    }

                    waiting = needToWait;
                }

                for (int curTask = 0; curTask < tasks.Count; curTask++)
                {
                    Task<bool> task = tasks[curTask];

                    string foundUrl = UrlList[i + curTask];
                    // it was successful so...
                    // get the current url 
                    if (task.Result) // get the current url
                    {
                        // If we haven't specified we don't want a temporary file, write it to successful_urls.log
                        if (!CommandLine.DontGenerateTempFile) tempFile.WriteLine(foundUrl);

                        if (CommandLine.Verbosity >= Verbosity.Normal) NCLogging.Log($"Found valid link at {foundUrl}!");
                        successfulUrls.Add(foundUrl); // add it
                    }
                    else
                    {
                        if (task.IsFaulted)
                        {
                            NCLogging.Log($"An error occurred while downloading {foundUrl}!");
                            failedUrls++;
                        }
                        else
                        {
                            if (CommandLine.Verbosity >= Verbosity.Verbose) NCLogging.Log($"URL not found: {foundUrl}");
                        }

                    }
                }

                tasks.Clear();

                double percentageCompletion = (((double)i / (double)UrlList.Count)) * 100;
                string percentageCompletionString = percentageCompletion.ToString("F1");

                // Performance improvement: don't dump to the console so often
                // we should allow the user to control this in future
                if (i % noDownloadsAtOnce == 0 && CommandLine.Verbosity >= Verbosity.Normal) Console.WriteLine($"{percentageCompletionString}% complete ({i}/{UrlList.Count} URLs scanned, {failedUrls} failed), {successfulUrls.Count} files found");
            }
            
            // set some temporary variables
            double timeElapsed = timer.ElapsedMilliseconds / 1000;

            if (CommandLine.Verbosity >= Verbosity.Normal) NCLogging.Log($"Took {timeElapsed}sec to check {UrlList.Count} URLs, found {successfulUrls.Count} files ({successfulUrls.Count / timeElapsed} URLs per second");

            if (!CommandLine.DontGenerateTempFile)
            {
                // delete SuccessfulURLs.log if we created it
                tempFile.Close();
                File.Delete(DEFAULT_TEMP_FILE_NAME);
            }

            return successfulUrls;
        }

        /// <summary>
        /// Try and download a file.
        /// </summary>
        /// <param name="fileName">A URI to try and download.</param>
        /// <returns>A boolean determining if the file downloaded successfully. It will return false and <see cref="Task.IsFaulted"/> will be true if an exception occurred.</returns>
        private static bool CheckFileExists(string fileName)
        {
            try
            {
                HttpRequestMessage headRequest = new HttpRequestMessage(HttpMethod.Head, fileName);
                HttpResponseMessage responseMsg = httpClient.Send(headRequest);

                return responseMsg.IsSuccessStatusCode;
            }
            catch
            {
                return false; 
            }
        }

        private static bool DownloadSuccessfulFiles(List<string> urls)
        {
            try
            {
                if (CommandLine.Verbosity > Verbosity.Quiet) NCLogging.Log("Downloading successful URLs...");

                for (int curUrl = 0; curUrl < urls.Count; curUrl++)
                {
                    string url = urls[curUrl];

                    if (CommandLine.Verbosity > Verbosity.Quiet)
                    {
                        string outFileName = CommandLine.OutFile;

                        // prevent downloading the same file several times 
                        if (urls.Count > 1)
                        {
                            string[] fileNameSplit = url.Split('/');
                            // get the last one
                            string fileNameOnly = fileNameSplit[fileNameSplit.Length - 1];

                            outFileName = $"{curUrl + 1}_{fileNameOnly}";
                        }

                        NCLogging.Log($"Downloading {url}... to {outFileName}");

                        // Get a stream of the file 
                        var stream = httpClient.GetByteArrayAsync(url);

                        // Wait for download to complete (we do this basically synchronously to reduce server load)
                        while (!stream.IsCompleted) { };

                        using (FileStream fileStream = new FileStream(outFileName, FileMode.Create))
                        {
                            fileStream.Write(stream.Result);
                        }
                    }
                }

                return true; 
            }
            catch (Exception ex)
            {
                NCLogging.Log($"An exception occurred: {ex}");
                return false;
            }

        }
    }
}
