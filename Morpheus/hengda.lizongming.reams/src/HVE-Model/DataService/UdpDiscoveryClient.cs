using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using CommonLibrary;
//using GalaSoft.MvvmLight.Messaging;
using Model.Commands;

namespace Model.DataService
{
	// Token: 0x0200007C RID: 124
	public class UdpDiscoveryClient
	{
		// Token: 0x0600034C RID: 844 RVA: 0x000090FC File Offset: 0x000072FC
		public static bool StartDiscoveryThread()
		{
			bool result;
			try
			{
				UdpDiscoveryClient._exitThread = false;
				UdpDiscoveryClient._discoveryThread = new Thread(new ThreadStart(UdpDiscoveryClient.DiscoveryThread))
				{
					Priority = ThreadPriority.Normal,
					Name = "Sentinel Communication Thread",
					IsBackground = true
				};
				UdpDiscoveryClient._discoveryThread.Start();
				UdpDiscoveryClient.ThreadStartWaitHandle.WaitOne();
				result = true;
			}
			catch (Exception)
			{
				result = false;
			}
			return result;
		}

		// Token: 0x0600034D RID: 845 RVA: 0x00009170 File Offset: 0x00007370
		private static void DiscoveryThread()
		{
			try
			{
				IPEndPoint localEP = new IPEndPoint(SentryDataService.LocalIpAddress, 0);
				UdpDiscoveryClient._udpServer = new UdpClient(localEP);
				UdpDiscoveryClient._udpServer.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 2000);
				UdpDiscoveryClient._udpServer.EnableBroadcast = true;
				UdpDiscoveryClient.ThreadStartWaitHandle.Set();
				while (!UdpDiscoveryClient._exitThread)
				{
					try
					{
						IPEndPoint ipendPoint = new IPEndPoint(IPAddress.Any, 3005);
						UdpDiscoveryClient._receiveBuffer = null;
						UdpDiscoveryClient._receiveBuffer = UdpDiscoveryClient._udpServer.Receive(ref ipendPoint);
					}
					catch
					{
					}
					if (UdpDiscoveryClient._receiveBuffer != null && UdpDiscoveryClient._receiveBuffer.Length > 0)
					{
						string @string = Encoding.ASCII.GetString(UdpDiscoveryClient._receiveBuffer, 0, UdpDiscoveryClient._receiveBuffer.Length);
						string a = @string.GetStringBetween('<', '>').ToUpper();
						if (a == "DISCOVERYRESPONSE")
						{
							DiscoveryResponse message = Serializers.Deserialize<DiscoveryResponse>(@string);
							//Messenger.Default.Send<DiscoveryResponse>(message);
						}
					}
				}
			}
			catch (Exception ex)
			{
				SentryDebug.WriteLine("2 " + ex.Message);
			}
			finally
			{
				if (UdpDiscoveryClient._udpServer != null)
				{
					UdpDiscoveryClient._udpServer.Close();
				}
			}
		}

		// Token: 0x0600034E RID: 846 RVA: 0x000092B8 File Offset: 0x000074B8
		public static void SendDiscovery()
		{
			SentryDebug.WriteLine("====== SendDiscovery to " + UdpDiscoveryClient.RemoteEndPoint);
			UdpDiscoveryClient._udpServer.Send(UdpDiscoveryClient.DiscoveryCommandBuffer, UdpDiscoveryClient.DiscoveryCommandBuffer.Length, UdpDiscoveryClient.RemoteEndPoint);
		}

		// Token: 0x0600034F RID: 847 RVA: 0x000092EA File Offset: 0x000074EA
		public static void Close()
		{
			UdpDiscoveryClient._exitThread = true;
		}

		// Token: 0x04000252 RID: 594
		private const int DiscoveryPort = 3005;

		// Token: 0x04000253 RID: 595
		private const int InputBufferSize = 1000;

		// Token: 0x04000254 RID: 596
		private static readonly AutoResetEvent ThreadStartWaitHandle = new AutoResetEvent(false);

		// Token: 0x04000255 RID: 597
		private static Thread _discoveryThread;

		// Token: 0x04000256 RID: 598
		private static bool _exitThread;

		// Token: 0x04000257 RID: 599
		private static UdpClient _udpServer;

		// Token: 0x04000258 RID: 600
		private static byte[] _receiveBuffer = new byte[1000];

		// Token: 0x04000259 RID: 601
		private static readonly IPAddress RemoteAddress = IPAddress.Parse("255.255.255.255");

		// Token: 0x0400025A RID: 602
		private static readonly IPEndPoint RemoteEndPoint = new IPEndPoint(UdpDiscoveryClient.RemoteAddress, 3005);

		// Token: 0x0400025B RID: 603
		private static readonly string StrCommand = Serializers.Serializer<Discovery>(new Discovery());

		// Token: 0x0400025C RID: 604
		private static readonly byte[] DiscoveryCommandBuffer = Encoding.ASCII.GetBytes(UdpDiscoveryClient.StrCommand);
	}
}
