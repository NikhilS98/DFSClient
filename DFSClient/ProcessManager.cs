using DFSUtility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

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

           
        }
    }
}
