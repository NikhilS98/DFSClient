using System;
using System.Collections.Generic;
using System.Text;

namespace DFSClient
{
    public static class State
    {
        private static string currentDirectory;
        public static string CurrentDirectory 
        {
            get
            {
                return currentDirectory;
            }
            set 
            { 
                currentDirectory = value;
                Console.Write(currentDirectory + ">");
            } 
        }
    }
}
