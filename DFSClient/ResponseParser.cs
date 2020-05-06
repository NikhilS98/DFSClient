using DFSUtility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DFSClient
{
    public static class ResponseParser
    {
        //Response Parser. Basically all just print the received data 
        //on screen except for open file, open dir
        public static void Parse(Response response)
        {
            if(response.Request.Command == Command.openFile)
            {
                string path = @"D:\dos\" + ((string)response.Request.Parameters[0]).GetFileName();
                File.WriteAllText(path, response.Data);

                Task.Run(() => ProcessManager.StartProcess(path));
            }
            else if(response.Request.Command == Command.cd)
            {
                State.CurrentDirectory = response.Data;
            }
            else
            {
                Console.WriteLine($"Response for {response.Request.Id}: {response.Data}");
            }
        }
    }
}
