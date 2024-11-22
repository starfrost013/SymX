// SymX
// A Microsoft Symbol Server bulk download tool
// © 2022-2024 starfrost

using SymX;

Logger.Init();

if (Configuration.Parse(args))
{
    // if we only have help we are done here
    if (Configuration.HelpOnly) return;

    // Initialise NuCore logging with logging based on the status of the -l option
    Logger.Settings.WriteToLog = Configuration.LogToFile;


    Configuration.PrintVersion();
    TaskManager.GenerateListOfTasks();

  
    while (TaskManager.Run()) ;
}