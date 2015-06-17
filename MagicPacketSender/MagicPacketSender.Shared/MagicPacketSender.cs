using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;
using Windows.Networking.Sockets;
using System.Linq;
using Windows.Networking;
using Windows.Storage.Streams;
using Windows.Foundation;

namespace MagicPacketSender
{
    public class MagicPacketSender
    {        
        public event TypedEventHandler<DatagramSocket, DatagramSocketMessageReceivedEventArgs> OnSocketMessageReceived;

        /// <summary>
        /// Attempts to send a magic packet to the desination hostname, on the specified port with the specified MAC address.
        /// </summary>
        /// <param name="destination">Destination hostname or IP address.</param>
        /// <param name="destinationPort">Destination port number.</param>
        /// <param name="targetMac">Destination MAC address. Bytes can be separated by colons, dashes, or nothing.</param>
        /// <returns>True if magic packet is sent successfully, false otherwise.</returns>
        public async Task<bool> SendMagicPacket(Windows.Networking.HostName destination, uint destinationPort, string targetMac)
        {
            try
            {
                DatagramSocket _socket = new DatagramSocket();

                _socket.MessageReceived += _socket_MessageReceived;

                using (var stream = await _socket.GetOutputStreamAsync(destination, destinationPort.ToString()))
                {
                    //Split on common MAC separators
                    char? splitChar = null;
                    if (targetMac.Contains('-'))
                        splitChar = '-';
                    else if (targetMac.Contains(':'))
                        splitChar = ':';
                    else if (targetMac.Contains(' '))
                        splitChar = ' ';

                    //Turn MAC into array of bytes
                    byte[] macAsArray;
                    if (splitChar != null)
                    {
                        macAsArray = targetMac.Split((char)splitChar)
                            .Select(b => Convert.ToByte(b, 16))
                            .ToArray<byte>();
                    }
                    else
                    {
                        macAsArray = Enumerable.Range(0, targetMac.Length)
                            .Where(x => x % 2 == 0)
                            .Select(x => Convert.ToByte(targetMac.Substring(x, 2), 16))
                            .ToArray();
                    }

                    List<byte> magicPacket = new List<byte> { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };

                    //Append MAC address to the magic packet 16 times.
                    for (int i = 0; i < 16; i++)
                    {
                        magicPacket = magicPacket.Concat(macAsArray).ToList();
                    }

                    using (DataWriter writer = new DataWriter(stream))
                    {
                        writer.WriteBytes(magicPacket.ToArray<byte>());
                        await writer.StoreAsync();
                        await writer.FlushAsync();
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return false;
            }
        }

        void _socket_MessageReceived(DatagramSocket sender, DatagramSocketMessageReceivedEventArgs args)
        {
            var eventHandler = OnSocketMessageReceived;
            if (eventHandler != null)
            {
                OnSocketMessageReceived(sender, args);
            }
        }       
    }
}
