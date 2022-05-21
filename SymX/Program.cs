// See https://aka.ms/new-console-template for more information
using SymX;

if (CommandLine.Parse(args))
{
    CommandLine.PrintVersion();
    TaskManager.GenerateListOfTasks();

    while (TaskManager.Run()) ;
}
else
{
    CommandLine.ShowHelp();
}