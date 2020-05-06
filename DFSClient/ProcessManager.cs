using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace DFSClient
{
    public static class ProcessManager
    {
        public static void StartProcess(string path)
        {
            var process = new Process();
            process.StartInfo = new ProcessStartInfo()
            {
                UseShellExecute = true,
                FileName = path
            };

            process.Start();
            process.WaitForExit();

            Console.WriteLine("Exited");
            //do something on exit like send request to server for saving it.
        }
    }
}
