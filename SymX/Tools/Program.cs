// See https://aka.ms/new-console-template for more information
using NuCore.Utilities;
using SymX;

if (CommandLine.Parse(args))
{
    // Initialise NuCore logging with default settings
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