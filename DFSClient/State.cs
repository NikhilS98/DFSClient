using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace DFSClient
{
    public static class State
    {
        public static string CurrentDirectory { get; set; }
        public static Socket Server { get; set; }
        public static string LocalRootDirectory { get; set; } = "D:\\client";
        public static bool WaitingForInput { get; set; }
        public static string ConfigFilePath { get; set; } = "D:\\client\\config.txt";
    }
}
