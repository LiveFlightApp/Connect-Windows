using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LiveFlight
{
    public class BroadcastReceiver
    {
        private UdpClient udp = new UdpClient(15000);

        public event EventHandler DataReceived = delegate { };

        public void StartListening()
        {
            Console.WriteLine("Starting Joystick listening server...");
            this.udp.BeginReceive(Receive, new object());
        }
        private void Receive(IAsyncResult ar)
        {
            try
            {
                IPEndPoint ip = new IPEndPoint(IPAddress.Any, 15000);

                if (udp != null)
                {
                    byte[] bytes = udp.EndReceive(ar, ref ip);
                    Console.WriteLine("Received {0} bytes", bytes.Length);
                    if (bytes.Length != 0)
                    {
                        DataReceived(bytes, EventArgs.Empty);
                    }

                    if (this.udp != null)
                    {
                        this.udp.BeginReceive(Receive, new object());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception while reading UDP data: {0}", ex);
            }
            finally
            {
            }

        }

        internal void Stop()
        {
            Console.WriteLine("Stopping UDP Receiver");
            try
            {
                udp.Close();
                udp = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception while stopping UDP Client: {0}", ex);
            }
        }
    }
}
