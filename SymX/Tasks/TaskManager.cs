
namespace SymX
{
    public static class TaskManager
    {
        public static List<Tasks> TaskList { get; set; }

        static TaskManager()
        {
            TaskList = new List<Tasks>();
        }

        public static void GenerateListOfTasks()
        {
            TaskList.Add(Tasks.GenerateListOfUrls);

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

            for (int i = 0; i < TaskList.Count; i++)
            {
                Tasks currentTask = TaskList[i];
                curTask++;
                
                string taskString = $"{SymXVersion.SYMX_APPLICATION_NAME} - Performing task {curTask}/{numTasks} ({currentTask})...";

                Console.Title = taskString;

                if (CommandLine.Verbosity >= Verbosity.Normal) Console.WriteLine(taskString);

                switch (currentTask)
                {
                    // Generate a list of URLs
                    case Tasks.GenerateListOfUrls:
                        GenerateUrlList();
                        continue; 
                    // Exit the program.
                    case Tasks.Exit:
                        Environment.Exit(0);
                        continue; 
                }

                TaskList.Remove(currentTask);
            }

            return (TaskList.Count > 0);
        }

        private static List<string> GenerateUrlList()
        {
            List<string> urlList = new List<string>();

            if (CommandLine.InFile != null)
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
                Console.WriteLine("!!INFILE NOT IMPLEMENTED YET!!");
            }

            return urlList; 
        }
    }
}
