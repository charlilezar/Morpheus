using NetCoreServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HengDa.LiZongMing.REAMS
{
    public class EchoServer : UdpServer
    {
        public EchoServer(string address, int port) : base(address, port) { }

        protected override void OnStarted()
        {
            // Start receive datagrams
            ReceiveAsync();
        }

        protected override void OnReceived(EndPoint endpoint, byte[] buffer, long offset, long size)
        {
            // Continue receive datagrams.
            if (size == 0)
            {
                // Important: Receive using thread pool is necessary here to avoid stack overflow with Socket.ReceiveFromAsync() method!
                ThreadPool.QueueUserWorkItem(o => { ReceiveAsync(); });
            }
            SendAsync(endpoint, buffer, offset, size);
            Console.WriteLine($"Server reciced data ");

            // Echo the message back to the sender
            // SendAsync(endpoint, buffer, offset, size);
        }

        protected override void OnSent(EndPoint endpoint, long sent)
        {
            // Continue receive datagrams.
            // Important: Receive using thread pool is necessary here to avoid stack overflow with Socket.ReceiveFromAsync() method!
            ThreadPool.QueueUserWorkItem(o => { ReceiveAsync(); });
        }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"Server caught an error with code {error}");
        }
    }
}
