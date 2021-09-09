using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using HengDa.LiZongMing.REAMS.CtrlServices;
using Microsoft.Extensions.Logging;
//using GalaSoft.MvvmLight.Messaging;
using Model.DataService;
using Model.Events;
using Model.Measurements;
//using Model.MvvmMessages;

namespace Model.Notifications
{
    // Token: 0x020000AD RID: 173
    public static class UdpNotifyInput
    {
        /// <summary>
        /// 事件回调时，收到数据触发解析
        /// </summary>
        public static Action<ScalarMeasurement> OnUdpScalarMeasurementReceived { get; set; } = null;


        public static Action<CompositeMeasurement> OnUdpCompositeMeasurementReceived { get; set; } = null;
        public static Action<SentinelState> OnUdpSentinelStateReceived { get; set; } = null;

        // Token: 0x060004DD RID: 1245 RVA: 0x0000B6A4 File Offset: 0x000098A4
        public static void OpenUdpClient(int port/*, Messenger mvvmMessenger*/)
        {
            try
            {
                if (!UdpNotifyInput.NotifyOpen)
                {
                    if (UdpNotifyInput.s_Client != null)
                    {
                        UdpNotifyInput.s_Client.Close();
                    }
                    //UdpNotifyInput.s_MvvmMessenger = mvvmMessenger;
                    UdpNotifyInput.s_Client = new UdpClient(port);
                    IPEndPoint ipendPoint = (IPEndPoint)UdpNotifyInput.s_Client.Client.LocalEndPoint;
                    UdpNotifyInput.NotifyPortNumber = ipendPoint.Port;
                    UdpNotifyInput.NotifyOpen = true;
                    SentryDebug.WriteLine($" 启动 UdpNotify在端口 :" + ipendPoint.Port);
                    UdpNotifyInput.s_Client.BeginReceive(new AsyncCallback(UdpNotifyInput.ReceiveCallback), null);
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }
        }

       static int ii=0;
        // Token: 0x060004DE RID: 1246 RVA: 0x0000B724 File Offset: 0x00009924
        private static void ReceiveCallback(IAsyncResult ar)
        {
            if (!UdpNotifyInput.AppClosing)
            {
                try
                {
                    IPEndPoint ipendPoint = new IPEndPoint(IPAddress.Any, 8000);
                    byte[] array = UdpNotifyInput.s_Client.EndReceive(ar, ref ipendPoint);
                    byte b = array[0];
                    BitConverter.ToInt32(array, 1);
                    int num = array.Length;
                    int num2 = num - 5;
                    byte[] array2 = new byte[num2];
                    Array.Copy(array, 5, array2, 0, num2);

                    string @string = Encoding.ASCII.GetString(array2, 0, num2);
                    if (CtrlHVEService.DebugXml)
                    {
                        Logger.LogDebug((ii++) +$"  ===UDP接收到 UdpNotify : lenght:{num2}\r\n```\r\n" + @string + "\r\n```\r\n");
                    }


                    //if (CtrlHVEService.ServiceDictionary.ContainsKey(ipendPoint.Address.ToString()))
                    {
                        //CtrlHVEService dataService = CtrlHVEService.ServiceDictionary[ipendPoint.Address.ToString()];
                        MemoryStream stream = new MemoryStream(array2, 0, num2, true, true);
                        string stringBetween = @string.GetStringBetween('<', '>');
                        if (stringBetween == "ScalarMeasurement") //标量测量值
                        {
                            ScalarMeasurement scalarMeasurement = ScalarMeasurement.DeserializeFromStream(stream);
                            if (OnUdpScalarMeasurementReceived != null)
                                OnUdpScalarMeasurementReceived(scalarMeasurement); //回发

                            SentryDebug.WriteLine("Error in scalar measurement notify-text recvd=" + @string);

                        }
                        else if (stringBetween == "SentinelState") //哨兵状态,事件回发用的
                        {
                            SentinelState sentinelState = SentinelState.DeserializeFromStream(stream);
                            if (OnUdpSentinelStateReceived != null)
                                OnUdpSentinelStateReceived(sentinelState); //回发

                            SentryDebug.WriteLine("Error in SentinelState notify-text recvd=" + @string);

                        }
                        else if (stringBetween == "WaveformMeasurement")//波形计量
                        {
                            WaveformMeasurement waveformMeasurement = WaveformMeasurement.DeserializeFromStream(stream);
                            if (OnUdpCompositeMeasurementReceived != null)
                                OnUdpCompositeMeasurementReceived(waveformMeasurement); //回发
                        }
                        else if (stringBetween == "BatteryMeasurement") //电池计量
                        {
                            BatteryMeasurement batteryMeasurement = BatteryMeasurement.DeserializeFromStream(stream);
                            if (OnUdpCompositeMeasurementReceived != null)
                                OnUdpCompositeMeasurementReceived(batteryMeasurement); //回发
                        }
                        else
                        {
                            Logger.LogTrace($"  UdpNotify 未处理 ");
                        }
                    }
                }
                catch (Exception ex)
                {
                    SentryDebug.WriteLine($" 接收到了udp通知处理出错:" + ex.ToString());
                }
                UdpNotifyInput.s_Client.BeginReceive(new AsyncCallback(UdpNotifyInput.ReceiveCallback), null);
            }
            else
            {
                //关闭了
                SentryDebug.WriteLine($" 接收到了udp通知，但是程序正在关闭不能处理。");
            }
        }

        // Token: 0x04000337 RID: 823
        private static UdpClient s_Client;

        // Token: 0x04000338 RID: 824
        public static bool AppClosing = false;

        // Token: 0x04000339 RID: 825
        public static bool NotifyOpen = false;

        // Token: 0x0400033A RID: 826
        public static int NotifyPortNumber = 0;

        // Token: 0x0400033B RID: 827
        //private static Messenger s_MvvmMessenger;

        public static  ILogger Logger { get; set;  } = new Microsoft.Extensions.Logging.LoggerFactory().CreateLogger("HVE.UdpNotify");

        //internal static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    }
}
