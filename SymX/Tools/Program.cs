// SymX
// A Microsoft Symbol Server bulk download tool
// © 2022 starfrost

using NuCore.Utilities;
using SymX;

if (CommandLine.Parse(args))
{
    // Initialise NuCore logging with logging based on the status of the -l option
    NCLogging.Settings.WriteToLog = CommandLine.LogToFile;
    NCLogging.Init();

    CommandLine.PrintVersion();
    TaskManager.GenerateListOfTasks();

    while (TaskManager.Run()) ;
}
else
{
    CommandLine.ShowHelp();
}