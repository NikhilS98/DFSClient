using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using DFSUtility;
using System.Linq;

namespace DFSClient
{
    class Program
    {
        static Task[] tasks = null;
        static void Main(string[] args)
        {
            
            /*string clientDir = "D:\\client";
            int suffix = 1;
            while (Directory.Exists(clientDir))
            {
                clientDir += suffix++;
            }
            State.LocalRootDirectory = clientDir;
            Directory.CreateDirectory(State.LocalRootDirectory);*/

            if (!Directory.Exists(State.LocalRootDirectory))
            {
                Directory.CreateDirectory(State.LocalRootDirectory);
            }

            ConfigurationHelper.Create(State.ConfigFilePath, new string[] { "192.168.0.105:11000" });

            var ips = ConfigurationHelper.Read(State.ConfigFilePath).ToList();

            do
            {
                int index = new Random().Next(ips.Count);
                string ipPort = ips[index];

                Connect(ipPort);

                ips = ConfigurationHelper.Read(State.ConfigFilePath).ToList();
                if (ips.Remove(ipPort))
                    ConfigurationHelper.Update(State.ConfigFilePath, ips);

            } while (ips.Count > 0);

            Console.WriteLine("Server not found");
            if (File.Exists(State.ConfigFilePath))
                File.Delete(State.ConfigFilePath);

        }

        static void Send(Socket server)
        {
            while (true)
            {
                State.WaitingForInput = true;
                var request = InputParser.GetRequestFromInput(InputParser.GetUserInput());
                State.WaitingForInput = false;

                if (request == null)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Command not recognized");
                    Console.ResetColor();
                    continue;
                }

                byte[] buffer = request.SerializeToByteArray();

                try
                {
                    Network.Send(server, buffer);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Request {request.Id} sent");
                    Console.ResetColor();
                }
                catch (Exception e)
                {
                    //connect with another server
                    break;
                }
            }

        }

        static void Receive(Socket server)
        {
            while (true)
            {
                try
                {
                    var bytes = Network.Receive(server, 100000);
                    var response = bytes.Deserialize<Response>();
                    Task.Run(() => ResponseParser.Parse(response));
                }
                catch (Exception e)
                {
                    break;
                }
            }
        }

        static void Connect(string ipPort)
        {
            IPAddress ipAddress = IPAddress.Parse(ipPort.Substring(0, ipPort.LastIndexOf(":")));
            IPEndPoint remoteEP = new IPEndPoint(ipAddress,
                Convert.ToInt32(ipPort.Substring(ipPort.LastIndexOf(":") + 1)));

            // Create a TCP/IP  socket.    
            Socket server = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            // Connect the socket to the remote endpoint. Catch any errors.    
            try
            {
                // Connect to Remote EndPoint  
                server.Connect(remoteEP);

                Request request = new Request { Command = Command.clientConnect };
                Network.Send(server, request.SerializeToByteArray());

                var buff = Network.Receive(server);
                var ipList = buff.Deserialize<List<string>>();
                ConfigurationHelper.Update(State.ConfigFilePath, ipList);

                State.CurrentDirectory = "root";
                State.Server = server;

                bool isConnected = true;
                foreach (var req in State.GetPendingRequests())
                {
                    try
                    {
                        Network.Send(server, req.SerializeToByteArray());
                    }
                    catch(Exception e)
                    {
                        isConnected = false;
                        break;
                    }
                }

                if (isConnected)
                {
                    tasks = new Task[]
                    {
                    Task.Run(() => Send(server)),
                    Task.Run(() => Receive(server))
                    };

                    Task.WaitAll(tasks);
                }

            }
            catch (Exception e)
            {
                //Console.WriteLine($"Unable to connect to server {ipPort} : {e.Message}");

            }
        }
    }
}
