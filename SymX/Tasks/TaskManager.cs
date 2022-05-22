using NuCore.Utilities;
using System;
using System.ComponentModel;
using System.Diagnostics; 
using System.Net.Http;
using System.Net.Http.Headers;
 
namespace SymX
{
    public static class TaskManager
    {
        public static List<Tasks> TaskList { get; set; }

        private static List<string> UrlList { get; set; }

        private static HttpClient httpClient = new HttpClient();

        private static Stopwatch timer = new Stopwatch();

        /// <summary>
        /// Magic number for how much Microsoft pads their SizeOfImage values to.
        /// </summary>
        private static ulong IMAGESIZE_PADDING = 0x1000; 

        static TaskManager()
        {
            TaskList = new List<Tasks>();
            UrlList = new List<string>(); 
        }

        public static void GenerateListOfTasks()
        {
            if (CommandLine.Verbosity == Verbosity.Verbose) Console.WriteLine("Initialising HTTP client...");

            if (CommandLine.CsvInFolder == null)
            {
                TaskList.Add(Tasks.GenerateListOfUrls);
            }
            else
            {
                TaskList.Add(Tasks.ParseCsv);
            }

            if (CommandLine.CsvGenerate) TaskList.Add(Tasks.GenerateCsv);

            if (CommandLine.InFile != null) TaskList.Add(Tasks.ParseCsv);
            
            if (!CommandLine.DontDownload) TaskList.Add(Tasks.TryDownload);

            TaskList.Add(Tasks.Exit);
        }

        public static bool Run()
        {
            // if there are no remaining tasks return false.

            int numTasks = TaskList.Count;
            int curTask = 1;

            // perform each task in sequence
            for (int i = 0; i < TaskList.Count; i++)
            {
                Tasks currentTask = TaskList[i];

                string taskString = $"{SymXVersion.SYMX_APPLICATION_NAME} - Performing task {curTask}/{numTasks} ({currentTask})...";

                Console.Title = taskString;

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
                    case Tasks.FatalError:
                        // A fatal error occurred so we can't continue.
                        // Nuke the program
                    case Tasks.Exit:
                        // Exit the program.
                        Environment.Exit(0);
                        continue; 
                }

            }

            return (TaskList.Count > 0);
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
                        for (ulong curImageSize = imageSizeMin; curImageSize <= imageSizeMax; curImageSize++)
                        {
                            string fileUrl = $"https://msdl.microsoft.com/download/symbols/{CommandLine.FileName}/{curTime.ToString("x")}{curImageSize.ToString("x")}/{CommandLine.FileName}";
                            if (CommandLine.Verbosity >= Verbosity.Verbose) Console.WriteLine(fileUrl);
                            urlList.Add(fileUrl);

                            curImageSize += (IMAGESIZE_PADDING - 1);
                        }

                        
                    }
                }

            }
            else
            {
                // placeholder until NuCore
                NCLogging.Log("-infile not implemented yet!", ConsoleColor.Red);
            }

            return urlList; 
        }

        /// <summary>
        /// Tries to download the selected URLs using HEAD requests. 
        /// </summary>
        /// <returns>A list of the URLs that successfully downloaded.</returns>
        private static List<string> TryDownload()
        {
            if (CommandLine.Verbosity >= Verbosity.Verbose) NCLogging.Log("Initialising HttpClient");

            // initialise the http client (we already instantiate it as a private field)
            // this is so we don't have to add a check for dontdownload
            httpClient.BaseAddress = new Uri("https://msdl.microsoft.com");

            // Fake a DbgX UA.
            // Just in case (thanks pivotman319)
            httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Microsoft-Symbol-Server", $"10.1710.0.0"));

            TaskList.Add(Tasks.GenerateListOfUrls);

            if (CommandLine.Verbosity >= Verbosity.Quiet) NCLogging.Log($"Trying {UrlList.Count} URLs...");

            timer = Stopwatch.StartNew();

            List<string> successfulUrls = new List<string>();

            int noDownloadsAtOnce = CommandLine.NumOfDownloadsAtOnce;

            // create a list of tasks
            // consider having it return the url instead
            List<Task<bool>> tasks = new List<Task<bool>>();

            for (int i = 0; i < UrlList.Count; i++)
            {
                // Set up a batch of downloads (default 12, ~numdownloads)
                for (int j = 0; j < noDownloadsAtOnce; j++)
                {
                    int curUrlId = i + j;

                    if (curUrlId < UrlList.Count)
                    {
                        string curUrl = UrlList[i + j];
                        Task<bool> worker = Task<bool>.Run(() => TryDownloadFile(curUrl));
                        tasks.Add(worker);
                    }

                }

                bool waiting = true;

                // wait for all of our tasks to complete
                while (waiting)
                {
                    // will exit if all tasks complete
                    bool needToWait = false; 

                    for (int curTask = 0; curTask < tasks.Count; curTask++)
                    {
                        Task<bool> task = tasks[curTask];

                        if (!task.IsCompleted)
                        {
                            needToWait = true; // we need to wait as not every task is done
                        }
                        else // add to successful url list
                        {
                            if (task.Result) // get the current url
                            {
                                successfulUrls.Add(UrlList[i + curTask]); // add it
                            }
                        }
                    }

                    waiting = needToWait;
                }

                tasks.Clear();

                
                double percentageCompletion = (((double)i / (double)UrlList.Count)) * 100;
                string percentageCompletionString = percentageCompletion.ToString("F1");

                // Performance improvement: don't dump to the console so often
                // allow user to control this in futrue
                if (i % noDownloadsAtOnce == 0 && CommandLine.Verbosity > Verbosity.Quiet) Console.WriteLine($"{percentageCompletionString}% complete ({i}/{UrlList.Count} URLs scanned), {successfulUrls.Count} files found");

                i += (noDownloadsAtOnce - 1);
            }

            if (CommandLine.Verbosity > Verbosity.Quiet) NCLogging.Log($"Took {timer.ElapsedMilliseconds / 1000}sec to check {UrlList.Count} URLs, found {successfulUrls.Count} files");

            return successfulUrls;
        }

        private static bool TryDownloadFile(string fileName)
        {
            if (CommandLine.Verbosity >= Verbosity.Verbose) NCLogging.Log($"Trying URL {fileName}...");

            HttpRequestMessage headRequest = new HttpRequestMessage(HttpMethod.Head, fileName);

            HttpResponseMessage responseMsg = httpClient.Send(headRequest);

            if (CommandLine.Verbosity >= Verbosity.Verbose) NCLogging.Log($"HTTP response = {responseMsg.StatusCode}");

            return responseMsg.IsSuccessStatusCode;
        }

        private static bool DownloadSuccessfulFiles(List<string> urls)
        {
            try
            {
                if (CommandLine.Verbosity > Verbosity.Quiet) NCLogging.Log("Downloading successful URLs...");

                foreach (string url in urls)
                {
                    if (CommandLine.Verbosity > Verbosity.Quiet)
                    {
                        Console.WriteLine($"Downloading {url}... to {CommandLine.OutFile}");

                        // Get a stream of the file 
                        var stream = httpClient.GetByteArrayAsync(url);

                        // Wait for download to complete (we do this basically synchronously to reduce server load)
                        while (!stream.IsCompleted) { };

                        using (FileStream fileStream = new FileStream(CommandLine.OutFile, FileMode.Create))
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
