// See https://aka.ms/new-console-template for more information
using SymX;

CommandLine.Parse(args);

if (CommandLine.Successful)
{
    CommandLine.PrintVersion();
    TaskManager.GenerateListOfTasks();
    
    while (TaskManager.Run())
    {

    }
}
else
{
    CommandLine.ShowHelp();
}