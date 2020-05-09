using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using DFSUtility;

namespace DFSClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Task[] tasks = null;

            try
            {
                //Console.Write("Select port to connect to: ");
                //string port = Console.ReadLine();

                var ips = File.ReadAllLines("D:\\server\\config.txt");

                int index = new Random().Next(ips.Length);
                string ipPort = ips[index];

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
                    Console.WriteLine(Encoding.UTF8.GetString(buff));

                    State.CurrentDirectory = "root";
                    State.Server = server;
                    
                    State.LocalRootDirectory = "D:\\dos";
                    Directory.CreateDirectory(State.LocalRootDirectory);

                    tasks = new Task[]
                    {
                        Task.Run(() => Send(server)),
                        Task.Run(() => Receive(server))
                    };

                }
                catch (ArgumentNullException ane)
                {
                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                }
                catch (SocketException se)
                {
                    Console.WriteLine("SocketException : {0}", se.ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Task.WaitAll(tasks);
        }

        static void Send(Socket server)
        {
            while (true)
            {
                State.WaitingForInput = true;
                var request = InputParser.GetRequestFromInput(InputParser.GetUserInput());
                State.WaitingForInput = false;

                if(request == null)
                {
                    Console.WriteLine("Command not recognized");
                    continue;
                }

                byte[] buffer = request.SerializeToByteArray();

                try
                {
                    Network.Send(server, buffer);
                    Console.WriteLine($"Request {request.Id} sent");
                }
                catch(Exception e)
                {
                    //connect with another server
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
                    //try connecting with another server from config
                }
            }        
        }
    }
}
