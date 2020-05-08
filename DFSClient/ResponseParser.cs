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
            string msg = null;
            if(!response.IsSuccess)
                msg = $"{response.Request.Id} failed: {response.Message}";
            else if (response.Request.Command == Command.openFile)
            {
                Directory.CreateDirectory(Path.Combine(State.LocalRootDirectory,
                    PathHelpers.GetDirFromPath((string)response.Request.Parameters[0])));
                string path = Path.Combine(State.LocalRootDirectory, ((string)response.Request.Parameters[0]));
                File.WriteAllText(path, response.Data);

                ProcessManager.StartProcess(path);

                string data = File.ReadAllText(path);
                var request = new Request
                {
                    Id = RequestHelper.GetRequestId(),
                    Command = Command.updateFile,
                    Method = "UpdateFile",
                    Type = "FileService",
                    Parameters = new object[] { (string)response.Request.Parameters[0], data }
                };

                byte[] buffer = request.SerializeToByteArray();
                int bytesSent = 0, totalBytesSent = 0;
                do
                {
                    bytesSent = State.Server.Send(buffer);
                    totalBytesSent += bytesSent;
                }
                while (totalBytesSent < buffer.Length);
            }
            else if(response.Request.Command == Command.cd)
            {
                State.CurrentDirectory = response.Data;
                Console.WriteLine();
                Console.Write(State.CurrentDirectory + ">");
            }
            else
            {
                msg = $"{response.Request.Id} succeeded:\n{response.Message}";
            }

            if (State.WaitingForInput && msg != null)
            {
                Console.WriteLine();
                Console.WriteLine(msg);
                Console.Write(State.CurrentDirectory + ">");
            }
        }
    }
}
