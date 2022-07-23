// SymX
// A Microsoft Symbol Server bulk download tool
// © 2022 starfrost

using NuCore.Utilities;
using SymX;

if (Configuration.Parse(args))
{
    if (Configuration.HelpOnly) return;

    // Initialise NuCore logging with logging based on the status of the -l option
    NCLogging.Settings.WriteToLog = Configuration.LogToFile;
    NCLogging.Init();

    Configuration.PrintVersion();
    TaskManager.GenerateListOfTasks();

    while (TaskManager.Run()) ;
}
else
{
    Configuration.ShowHelp();
}