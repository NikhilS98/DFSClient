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
                Console.Write("Select port to connect to: ");
                string port = Console.ReadLine();

                IPAddress ipAddress = IPAddress.Parse("192.168.0.105");
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, Convert.ToInt32(port));

                // Create a TCP/IP  socket.    
                Socket server = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);

                // Connect the socket to the remote endpoint. Catch any errors.    
                try
                {
                    // Connect to Remote EndPoint  
                    server.Connect(remoteEP);

                    byte[] buff = new byte[100];
                    int bytes = server.Receive(buff);
                    string dir = Encoding.UTF8.GetString(buff, 0, bytes);
                    State.CurrentDirectory = "root";
                    State.Server = server;
                    
                    State.LocalRootDirectory = "D:\\dos";
                    Directory.CreateDirectory(State.LocalRootDirectory);

                    tasks = new Task[]
                    {
                        Task.Run(() => SendRequest(server)),
                        Task.Run(() => ReceiveResponse(server))
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

        static void SendRequest(Socket server)
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
                Network.SendRequest(server, buffer);
            }

        }

        static void ReceiveResponse(Socket server)
        {
            while (true)
            {
                var bytes = Network.ReceiveResponse(server, 100000);
                var response = bytes.Deserialize<Response>();
                Task.Run(() => ResponseParser.Parse(response));
            }        
        }
    }
}
