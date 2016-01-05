using Fds.IFAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IFConnect
{
    /// <summary>
    /// Methods for opening and listening to IFConnect server.
    /// </summary>
    /// <remarks>
    /// Credit to Matt Laban @ FDS
    /// </remarks>
    public class IFConnectorClient
    {
        public event EventHandler<CommandReceivedEventArgs> CommandReceived = delegate { };

        private TcpClient client = new TcpClient();
        private NetworkStream NetworkStream { get; set; }

        ReaderWriterLockSlim apiCallQueueLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        private Queue<APICall> apiCallQueue = new Queue<APICall>();

        public void Connect(string host = "localhost", int port = 10111)
        {
            Console.WriteLine("Connecting to: {0}:{1}", host, port);

            try
            {

                client.Connect(host, port);
                client.NoDelay = true;

                this.NetworkStream = client.GetStream();

                Task.Run(() =>
                {

                    while (true)
                    {
                        try
                        {
                            var commandString = ReadCommand();
                            //Console.WriteLine("Reply from Server: {0}", commandString);
                            var response = Serializer.DeserializeJson<APIResponse>(commandString);

                            CommandReceived(this, new CommandReceivedEventArgs(response, commandString));

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error! " + ex.ToString());
                        }
                    }
                });

                Task.Run(() =>
                {

                    while (true)
                    {
                        apiCallQueueLock.EnterReadLock();
                        var pendingItems = apiCallQueue.Any();
                        apiCallQueueLock.ExitReadLock();
                        if (pendingItems)
                        {
                            try
                            {
                                apiCallQueueLock.EnterWriteLock();
                                var apiCall = apiCallQueue.Dequeue();
                                apiCallQueueLock.ExitWriteLock();
                                if (apiCall != null)
                                {
                                    WriteObject(apiCall);
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Error Sending Command: {0}", ex);
                            }
                        }
                        else
                        {
                            Thread.Sleep(60);
                        }
                    }
                });

            }
            catch (System.Net.Sockets.SocketException e)
            {

                Console.WriteLine("Caught exception: {0}", e);

            }

        }

        #region Networking
        private Int32 ReadInt()
        {
            byte[] data = new byte[4];
            NetworkStream.Read(data, 0, 4);
            return BitConverter.ToInt32(data, 0);
        }

        private string ReadCommand()
        {
            var sizeToRead = ReadInt();
            var buffer = new byte[sizeToRead];
            var offset = 0;

            while (sizeToRead != 0)
            {
                var read = NetworkStream.Read(buffer, offset, sizeToRead);
                offset += read;
                sizeToRead -= read;
            }

            string str = Encoding.UTF8.GetString(buffer);
            return str;
        }

        private void WriteObject<T>(T state)
        {
            var stateString = Serializer.SerializeJson(state);
            var data = UTF8Encoding.UTF8.GetBytes(stateString);
            byte[] size = BitConverter.GetBytes(data.Length);
            NetworkStream.Write(size, 0, size.Length);
            NetworkStream.Write(data, 0, data.Length);
        }
        #endregion

        internal void SetValue(string parameter, string value)
        {
            APICall call = new APICall { Command = "SetValue", Parameters = new CallParameter[] { new CallParameter { Name = parameter, Value = value } } };
            QueueCall(call);
        }

        private void QueueCall(APICall call)
        {
            apiCallQueueLock.EnterWriteLock();
            apiCallQueue.Enqueue(call);
            apiCallQueueLock.ExitWriteLock();
        }

        public void ExecuteCommand(string command, CallParameter[] parameter = null)
        {
            APICall call = new APICall { Command = command, Parameters = parameter };
            QueueCall(call);
        }


        public void SendCommand(APICall call)
        {
            QueueCall(call);
        }

        internal void GetValue(string parameter)
        {
            APICall call = new APICall { Command = "GetValue", Parameters = new CallParameter[] { new CallParameter { Name = parameter } } };
            QueueCall(call);
        }
    }

    public class CommandReceivedEventArgs : EventArgs
    {
        public APIResponse Response { get; set; }
        public string CommandString { get; set; }

        public CommandReceivedEventArgs(APIResponse response, string commandString)
        {
            // TODO: Complete member initialization
            this.Response = response;
            this.CommandString = commandString;
        }
    }
}
