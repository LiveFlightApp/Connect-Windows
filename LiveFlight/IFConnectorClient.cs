using Fds.IFAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Fds.IFAPI
{
    public class IFConnectorClient
    {
        public event EventHandler<CommandReceivedEventArgs> CommandReceived = delegate { };

        private TcpClient client = new TcpClient();
        private NetworkStream NetworkStream { get; set; }

        ReaderWriterLockSlim apiCallQueueLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        private Queue<APICall> apiCallQueue = new Queue<APICall>();

        public Boolean Connect(string host, int port = 10111)
        {
            Console.WriteLine("Connecting to: {0}:{1}", host, port);

            try {

                client.Connect(host, port);

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

                            //Console.WriteLine("Response: {0}", response.Result);

                            CommandReceived(this, new CommandReceivedEventArgs(response, commandString));

                        }
                        catch (Exception ex)
                        {

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

                return true;

            }
            catch (System.Net.Sockets.SocketException e)
            {

                Console.WriteLine("Caught exception: {0}", e);
                return false;

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

            Console.WriteLine(stateString);
            Console.WriteLine("size: " + Encoding.UTF8.GetString(size));
            Console.WriteLine("data: " + Encoding.UTF8.GetString(data));
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

        internal void ExecuteCommand(string command, CallParameter[] parameter = null)
        {
            APICall call = new APICall { Command = command, Parameters = parameter };
            QueueCall(call);
        }

        //internal void ExecuteCommand(string command, string[] parameters)
        //{
        //    var parameterList = new List<CallParameter>();
        //    parameterList.AddRange(parameters.Select(x => new CallParameter { Value = x }));

        //    APICall call = new APICall { Command = command, Parameters = parameterList.ToArray() };
        //    QueueCall(call);
        //}

        internal void SendCommand(APICall call)
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
