using System;
using System.Net;
using System.Net.Sockets;

namespace IFConnect
{
    public class BroadcastReceiver
    {
        private UdpClient udp = new UdpClient(15000);

        public event EventHandler DataReceived = delegate { };

        public void StartListening()
        {
            Console.WriteLine("Starting Joystick listening server...");
            if(this.udp==null) { udp = new UdpClient(15000); }
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

        public void Stop()
        {
            Console.WriteLine("Stopping UDP Receiver");
            try
            {
                if (udp != null)
                {
                    udp.Close();
                    udp = null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception while stopping UDP Client: {0}", ex);
            }
        }
    }
}
