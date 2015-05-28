using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.Networking;

namespace MagicPacketSender
{
    public class RequestInfo
    {
        public string RequestHostName { get; set; }
        public uint Port { get; set; }
        public string MacAddress { get; set; }
    }
}
