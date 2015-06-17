using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.Networking;

namespace MagicPacketSender
{
    public class RequestInfo
    {
        public string RequestHostName { get;  private set; }
        public uint Port { get; private set; }
        public string MacAddress { get; private set; }

        public RequestInfo(string requestHostName, uint port, string macAddress)
        {
            RequestHostName = requestHostName;
            Port = port;
            MacAddress = macAddress;
        }

        public override bool Equals(object obj)
        {
            if(obj == null)
            {
                return false;
            }

            if(obj as RequestInfo == null)
            {
                return false;
            }

            RequestInfo otherInfo = obj as RequestInfo;
            if(RequestHostName == otherInfo.RequestHostName
                && MacAddress == otherInfo.MacAddress
                && Port == otherInfo.Port)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Equals(RequestInfo otherInfo)
        {
            if(otherInfo == null)
            {
                return false;
            }

            if (RequestHostName == otherInfo.RequestHostName
               && MacAddress == otherInfo.MacAddress
               && Port == otherInfo.Port)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return RequestHostName.GetHashCode() ^ Port.GetHashCode() ^ MacAddress.GetHashCode();
        }

        public static bool operator ==(RequestInfo first, RequestInfo second)
        {
            if(ReferenceEquals(first, second))
            {
                return true;
            }

            if((object)first == null || (object)second == null)
            {
                return false;
            }

            return first.MacAddress == second.MacAddress
                && first.Port == second.Port
                && first.RequestHostName == second.RequestHostName;
        }

        public static bool operator !=(RequestInfo first, RequestInfo second)
        {
            return !(first == second);
        }
    }
}
