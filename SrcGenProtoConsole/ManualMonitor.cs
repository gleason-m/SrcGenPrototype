using SrcGenProtoLib;
using System;

namespace SrcGenProtoConsole
{
    public class ManualMonitor : Monitor
    {
        public override void Speak()
        {
            Console.WriteLine("I'm a manual monitor");
        }
    }
}