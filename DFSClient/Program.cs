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
                // Connect to a Remote server  
                // Get Host IP Address that is used to establish a connection  
                // In this case, we get one IP address of localhost that is IP : 127.0.0.1  
                // If a host has multiple addresses, you will get a list of addresses  
                IPHostEntry host = Dns.GetHostEntry("localhost");
                IPAddress ipAddress = host.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);

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
                    State.CurrentDirectory = dir;

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
                //string input = Console.ReadLine();
                //Request request = new Request
                //{
                //    Id = 1,
                //    Method = "OpenFile",
                //    Type = "FileService",
                //    Parameters = new object[] { "D:\\Courses\\DOS\\Lecture 9.pdf" }
                //};

                var request = InputParser.GetRequestFromInput(InputParser.GetUserInput());

                byte[] buffer = request.SerializeToByteArray();
                int bytesSent = 0, totalBytesSent = 0;
                do
                {
                    bytesSent = server.Send(buffer);
                    totalBytesSent += bytesSent;
                }
                while (totalBytesSent < buffer.Length);
            }

        }

        static void ReceiveResponse(Socket server)
        {
            while (true)
            {
                List<byte> bytesList = new List<byte>();
                //1 GB = 1073741824 bytes
                int size = 1000000;

                byte[] buffer = new byte[size];
                int bytesTransferred = 0;
                do
                {
                    bytesTransferred = server.Receive(buffer);
                    for (int i = 0; i < bytesTransferred; i++)
                    {
                        bytesList.Add(buffer[i]);
                    }
                    Console.WriteLine($"total bytes received till now: { bytesList.Count }, iteration: {bytesTransferred}");
                }
                while (bytesTransferred == size);

                var response = bytesList.ToArray().Deserialize<Response>();
                ResponseParser.Parse(response);
            }        
        }
    }
}
