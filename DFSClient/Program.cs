using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                Socket sender = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);

                // Connect the socket to the remote endpoint. Catch any errors.    
                try
                {
                    // Connect to Remote EndPoint  
                    sender.Connect(remoteEP);

                    byte[] buff = new byte[100];
                    int bytes = sender.Receive(buff);
                    Console.WriteLine(Encoding.UTF8.GetString(buff, 0, bytes));

                    /*string read = File.ReadAllText(@"C:\Users\Nikhil\source\repos\ReflectionDemo\ReflectionDemo\input.json");
                    read = "{ 'id':'1', 'type':'Program', 'method':'Test', 'parameters':['hello']}";

                    byte[] buffer = Encoding.UTF8.GetBytes(read);
                    Console.WriteLine(Encoding.UTF8.GetString(buffer));*/

                    // Send the data through the socket. 
                    Task.Run(() => SendRequest(sender));
                    Task.Run(() => ReceiveResponse(sender));

                    // Release the socket.    
                    //sender.Shutdown(SocketShutdown.Both);
                    //sender.Close();

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

            while (true)
            {

            }
        }

        static void SendRequest(Socket sender)
        {
            while (true)
            {
                string input = Console.ReadLine();
                Request request = new Request
                {
                    Id = 1,
                    Message = "open",
                    Method = "OpenFile",
                    Type = "FileService",
                    Parameters = new object[] { "D:\\Courses\\DOS\\Lecture 9.pdf" }
                };
                
                //byte[] buffer = Encoding.UTF8.GetBytes(input);

                byte[] buffer = request.SerializeToByteArray();
                int bytesSent = 0, totalBytesSent = 0;
                do
                {
                    bytesSent = sender.Send(buffer);
                    totalBytesSent += bytesSent;
                }
                while (totalBytesSent < buffer.Length);
            }

        }

        static void ReceiveResponse(Socket sender)
        {
            while (true)
            {
                List<byte> bytesList = new List<byte>();
                int size = 100;

                byte[] buffer = new byte[size];
                int bytesTransferred = 0;
                do
                {
                    bytesTransferred = sender.Receive(buffer);
                    for (int i = 0; i < bytesTransferred; i++)
                    {
                        bytesList.Add(buffer[i]);
                    }
                }
                while (bytesTransferred == size);

                var response = bytesList.ToArray().Deserialize<Response>();
                File.WriteAllBytes(@"D:\test.txt", response.Data);
                Process.Start(@"D:\test.txt");
            }
        }
    }
}
