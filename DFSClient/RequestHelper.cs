using System;
using System.Collections.Generic;
using System.Text;

namespace DFSClient
{
    public static class RequestHelper
    {
        private static int requestId = 0;
        private static readonly object requestIdLock = new object();

        public static int GetRequestId()
        {
            lock (requestIdLock)
            {
                requestId++;
            }
            return requestId;
        }
    }
}
