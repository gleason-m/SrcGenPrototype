using Monitors;
using System;

Console.WriteLine("SrcGen from XML:");
new MyMonitor().Speak();

Console.WriteLine();

var configs = MonitorConfigProvider.LoadConfigs();

Console.WriteLine("Source generated monitor configs (from YAML):");
foreach (var config in configs)
{
    Console.WriteLine(config);
}