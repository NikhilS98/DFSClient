using DFSUtility;
using System;
using System.Collections.Generic;
using System.Text;

namespace DFSClient
{
    public static class InputParser
    {
        public static int requestId = 0;
        public static string GetUserInput()
        {
            return Console.ReadLine();
        }

        public static Request GetRequestFromInput(string input)
        {
            Request request = new Request()
            {
                Id = requestId++,
            };

            var inputArr = input.Split("-");
            string[] tokens = new string[inputArr.Length - 1];
            Array.Copy(inputArr, 1, tokens, 0, inputArr.Length - 1);

            Command cmd;

            if (Enum.TryParse(tokens[0], out cmd))
            {
                request.Command = cmd;
                switch (cmd)
                {
                    case Command.ls:
                        ListDirectory(ref request);
                        break;
                    case Command.cd:
                        OpenDirectory(ref request, tokens);
                        break;
                    case Command.rmdir:
                        RemoveDirectory(ref request, tokens);
                        break;
                    case Command.rm:
                        RemoveFile(ref request, tokens);
                        break;
                    case Command.mvdir:
                        MoveDirectory(ref request, tokens);
                        break;
                    case Command.mv:
                        MoveFile(ref request, tokens);
                        break;
                    case Command.cpdir:
                        CopyDirectory(ref request, tokens);
                        break;
                    case Command.cp:
                        CopyFile(ref request, tokens);
                        break;
                }
                return request;
            }
            else if (tokens.Length >= 2)
            {
                OpenFile(ref request, tokens);
                return request;
            }

            return null;
        }

        private static void OpenFile(ref Request request, string[] tokens)
        {
            // Need to do something with this. Maybe maintain a local record of requests
            string executablePath = GetResolvedPath(tokens[0]);

            string filePath = GetResolvedPath(tokens[1]);

            request.Parameters = new object[] { filePath };
            request.Method = "OpenFile";
            request.Type = "FileService";
        }

        private static void RemoveFile(ref Request request, string[] tokens)
        {
            string currentPath = GetResolvedPath(tokens[1]);
            string newPath = GetResolvedPath(tokens[2]);

            request.Parameters = new object[] { currentPath, newPath };
            request.Method = "RemoveFile";
            request.Type = "FileService";
        }

        private static void MoveFile(ref Request request, string[] tokens)
        {
            string currentPath = GetResolvedPath(tokens[1]);
            string newPath = GetResolvedPath(tokens[2]);

            request.Parameters = new object[] { currentPath, newPath };
            request.Method = "MoveFile";
            request.Type = "FileService";
        }

        private static void CopyFile(ref Request request, string[] tokens)
        {
            string currentPath = GetResolvedPath(tokens[1]);
            string newPath = GetResolvedPath(tokens[2]);

            request.Parameters = new object[] { currentPath, newPath };

            request.Method = "CopyFile";
            request.Type = "FileService";
        }

        private static void ListDirectory(ref Request request)
        {
            request.Method = "ListContent";
            request.Type = "DirectoryService";
        }
        private static void OpenDirectory(ref Request request, string[] tokens)
        {
            string dirPath = GetResolvedPath(tokens[1]);
            
            request.Parameters = new object[] { dirPath };
            request.Method = "OpenDirectory";
            request.Type = "DirectoryService";
        }
        private static void RemoveDirectory(ref Request request, string[] tokens)
        {
            string filePath = GetResolvedPath(tokens[1]);
            request.Parameters = new object[] { filePath };
            request.Method = "RemoveDirectory";
            request.Type = "DirectoryService";
        }

        private static void MoveDirectory(ref Request request, string[] tokens)
        {
            string currentPath = GetResolvedPath(tokens[1]);
            string newPath = GetResolvedPath(tokens[2]);

            request.Parameters = new object[] { currentPath, newPath };
            request.Method = "MoveDirectory";
            request.Type = "DirectoryService";
        }

        private static void CopyDirectory(ref Request request, string[] tokens)
        {
            string currentPath = GetResolvedPath(tokens[1]);
            string newPath = GetResolvedPath(tokens[2]);

            request.Parameters = new object[] { currentPath, newPath };

            request.Method = "CopyDirectory";
            request.Type = "DirectoryService";
        }

        private static string GetResolvedPath(string path)
        {
            if (path.IsRelativePath())
            {
                //path = State.CurrentDirectory + path;
            }
            return path;
        }
    }
}
