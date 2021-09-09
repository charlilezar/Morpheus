using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Aming.Core
{
    #region 常见状态码（待定：随时会修改）

    /// <summary>
    /// 通讯状态码
    /// </summary>
    public enum CommunicationStateCode
    {
        /// <summary>
        /// 连接失败
        /// </summary>
        ConnectFailed = 300,
        /// <summary>
        /// 连接成功
        /// </summary>
        ConnectSuccess = 301,
        /// <summary>
        /// 连接不允许
        /// </summary>
        ConnectNotAllowed = 302,
        /// <summary>
        /// 断开连接不允许
        /// </summary>
        DisConnectAllowed = 303,
        /// <summary>
        /// 连接状态已经断开
        /// </summary>
        ConnectionBreaked = 304,
        /// <summary>
        /// 连接状态仍在保持
        /// </summary>
        ConnectionKeeping = 305,

        /// <summary>
        /// 接收数据成功
        /// </summary>
        ReceiveDataSuccess = 400,
        /// <summary>
        /// 接收数据没有异常，但是接收到的数据为空
        /// </summary>
        ReceiveDataIsNull = 401,
        /// <summary>
        /// 接收数据过程中发生了异常
        /// </summary>
        ReceiveDataOccurException = 402,

        /// <summary>
        /// 发送成功
        /// </summary>
        SendDataSucceed = 500,
        /// <summary>
        /// 发送失败
        /// </summary>
        SendDataFailed = 501,
        /// <summary>
        /// 发送数据发生异常
        /// </summary>
        SendDataOccurException = 501,

        /// <summary>
        /// 接收到的帧无差错
        /// </summary>
        FrameComplete = 600,
        /// <summary>
        /// 长度不一致
        /// </summary>
        FrameMissContent = 601,
        /// <summary>
        /// 内容不一致
        /// </summary>
        FrameContentError = 602,
        /// <summary>
        /// 长度和内容都不一致
        /// </summary>
        FrameMissAndError = 603,
        /// <summary>
        /// 帧未知状态
        /// </summary>
        FrameUnknow = 604,

        /// <summary>
        /// 协议未知状态
        /// </summary>
        ProtocolUnknow = 605,

        /// <summary>
        /// 通讯超时
        /// </summary>
        TimeOut = 606,
    }

    #endregion 常见状态码（待定：随时会修改）

    /// <summary>
    /// Socket通讯类：负责底层Socket通讯
    /// </summary>
    public class SocketTransmission // : Aming.Framework.Models.EntityBase
    {
        //private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        #region 字段&属性

        //指定需要使用何种类型协议与服务器通讯
        public ProtocolType ProtocolType { get; private set; }

        public string ProtocolTypeStr
        {
            get { return ProtocolType.ToString().ToUpper(); }
            set
            {

                if (value.ToUpper() == "UDP")
                {
                    if (ProtocolType == ProtocolType.Tcp && IsConnected)
                        DisTcpConnectToServer(); //要先断开Tcp

                    ProtocolType = ProtocolType.Udp;
                }
                else
                    ProtocolType = ProtocolType.Tcp;
                //OnPropertyChanged(() => ProtocolTypeStr);
                //OnPropertyChanged(() => ProtocolType);
                //OnPropertyChanged(() => ServerActStr);
            }
        }

        /// <summary>
        /// 是否与服务器处于连接状态
        /// </summary>
        public bool IsConnected
        {
            get { return _IsConnected; }
            set
            {
                _IsConnected = value;
                //base.SetProperty(ref _IsConnected, value);
                //OnPropertyChanged(() => IsConnected);
                //OnPropertyChanged(() => ServerActStr);
            }
        }
        bool _IsConnected;

        /// <summary>
        /// 是否结束了连接状态
        /// </summary>
        public bool IsEndConnect { get; set; }

        /// <summary>
        /// 需要连接的服务器IP地址
        /// </summary>
        public IPAddress ServerIp
        {
            get { return _ServerIp; }
            set
            {
                _ServerIp = value;
                //OnPropertyChanged(() => ServerIp);
                //OnPropertyChanged(() => ServerIpStr);
            }
        }
        public string ServerIpStr
        {
            get { return _ServerIp.ToString(); }
            set
            {
                _ServerIp = IPAddress.Parse(value);
                //OnPropertyChanged(() => ServerIp);
                //OnPropertyChanged(() => ServerIpStr);
            }
        }
        IPAddress _ServerIp = new IPAddress(0);
        /// <summary>
        /// 服务器连接的端口
        /// </summary>
        public int ServerPort
        {
            get { return _ServerPort; }
            set
            {
                _ServerPort = value;
                //base.SetProperty(ref _ServerPort, value);
            }
        }

        private int _ServerPort = 5000;

        //IP+端口=>网络端点
        public IPEndPoint ServerPoint { get; set; }

        //与服务器端通信的Socket对象
        public System.Net.Sockets.Socket ServerSocket { get; set; }

        /// <summary>
        /// 连接按钮显示合适的文字
        /// </summary>
        public string ServerActStr
        {
            get
            {
                if (this.IsConnected)
                    return "断开连接";
                else if (this.ProtocolType == ProtocolType.Tcp)
                    return "连接服务器";
                else
                    return "开启UDP通讯";
            }
        }
        /// <summary>
        /// 判断通信Socket是否工作
        /// </summary>
        public bool IsBusy
        {
            get { return _IsBusy; }
            set
            {
                _IsBusy = value;
                //base.SetProperty(ref _IsBusy, value);
            }
        }
        private bool _IsBusy;


        #endregion 字段&属性

        #region 构造函数：初始化Socket对象

        /// <summary>
        /// 初始化负责通讯传输Socket对象的一些基本字段信息
        /// </summary>
        /// <param name="ipaddress">IP地址</param>
        /// <param name="port">端口号</param>
        /// <param name="type">通讯协议</param>

        public SocketTransmission(string ipaddress, int port, ProtocolType type)
        {
            ServerIp = IPAddress.Parse(ipaddress);
            ServerPort = port;
            ServerPoint = new IPEndPoint(ServerIp, ServerPort);
            ProtocolType = type;
        }

        /// <summary>
        /// 初始化负责通讯传输Socket对象的一些基本字段信息
        /// </summary>
        /// <param name="ipaddress">IP地址</param>
        /// <param name="port">端口号</param>
        /// <param name="type">通讯协议</param>
        public SocketTransmission(string ipaddress, string port, ProtocolType type)
        {
            ServerIp = IPAddress.Parse(ipaddress);
            ServerPort = Convert.ToInt32(port);
            ServerPoint = new IPEndPoint(ServerIp, ServerPort);
            ProtocolType = type;
        }

        /// <summary>
        /// 初始化负责通讯传输Socket对象的一些基本字段信息
        /// </summary>
        /// <param name="ipaddress">IP地址</param>
        /// <param name="port">端口号</param>
        /// <param name="type">通讯协议</param>
        public SocketTransmission(IPAddress ipaddress, int port, ProtocolType type)
        {
            ServerIp = ipaddress;
            ServerPort = port;
            ServerPoint = new IPEndPoint(ServerIp, ServerPort);
            ProtocolType = type;
        }

        /// <summary>
        /// 初始化负责通讯传输Socket对象的一些基本字段信息
        /// </summary>
        /// <param name="ipaddress">IP地址</param>
        /// <param name="port">端口号</param>
        /// <param name="type">通讯协议</param>
        public SocketTransmission(IPAddress ipaddress, string port, ProtocolType type)
        {
            ServerIp = ipaddress;
            ServerPort = Convert.ToInt32(port);
            ServerPoint = new IPEndPoint(ServerIp, ServerPort);
            ProtocolType = type;
        }

        #endregion 构造函数：初始化Socket对象

        #region UDP客户端初始化

        /// <summary>
        /// 初始化UDP客户端，因为UDP的特性，对方端口未开也不管的。
        /// </summary>
        public void InitUdp()
        {
            if (ProtocolType == ProtocolType.Udp)
            {
                //设置要连接的服务器的IP地址，和连接的端口号
                ServerPoint = new IPEndPoint(ServerIp, ServerPort);
                //定义网络类型，数据连接类型和网络协议UDP
                ServerSocket = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                IsConnected = true; //udp 只能假装连上了
                //初始化设置不忙
                IsBusy = false;
            }
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        /// <returns>是否成功断开连接</returns>
        public bool CloseUdp()
        {
            return DisTcpConnectToServer();
        }

        #endregion UDP客户端初始化

        #region TCP客户端与服务器：建立连接与断开连接

        //public void TcpConnectToServer()
        //{
        //    //判断协议类型
        //    if (ProtocolType == ProtocolType.Tcp)
        //    {
        //        //判断与服务器是否是断开状态，如果不是处于连接状态则进行连接
        //        if (!IsConnected)
        //        {
        //            try
        //            {
        //                IsEndConnect = false;
        //                ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType);
        //                //连接远程TCP服务器
        //                ServerSocket.Connect(ServerPoint);
        //                //如果连接远程服务器成功
        //                if (ServerSocket.Connected)
        //                {
        //                    //标识已连接
        //                    IsConnected = true;
        //                }
        //            }
        //            catch
        //            {
        //                //标识未连接：连接过程发生异常
        //                IsConnected = false;
        //            }
        //            //Socket不忙
        //            IsBusy = false;
        //            //标识已经结束了连接过程
        //            IsEndConnect = true;
        //        }
        //    }
        //}

        /// <summary>
        /// 尝试连接服务器
        /// </summary>
        /// <param name="maxCount">最大连接次数</param>
        public void TcpConnectToServer(int maxCount = 1)
        {
            //判断协议类型
            if (ProtocolType == ProtocolType.Tcp)
            {
                //判断与服务器是否是断开状态，如果不是处于连接状态则进行连接
                if (!IsConnected)
                {
                    //连接尚未结束
                    ServerSocket = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType);
                    int count = 1;
                    do
                    {
                        try
                        {
                            Console.WriteLine($"第{count}次连接。。。", count);
                            ServerPoint = new IPEndPoint(ServerIp, ServerPort);

                            //连接远程TCP服务器
                            ServerSocket.Connect(ServerPoint);
                            //如果连接远程服务器成功
                            if (ServerSocket.Connected)
                            {
                                //标识已连接
                                IsConnected = true;
                                //刚刚建立连接，Socket不忙
                                IsBusy = false;
                                //已经结束连接过程
                                IsEndConnect = true;
                                //_logger.Trace($"成功的建立TCP连接到 {ServerIpStr}:{ServerPort}");
                                break;
                            }
                        }
                        catch(Exception ex)
                        {
                            //休眠0.5s，继续重新连接
                            Thread.Sleep(500);
                        }
                        count++;
                    } while (count < maxCount);
                    //已经结束连接,连接失败
                    IsEndConnect = true;
                }
            }
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        /// <returns>是否成功断开连接</returns>
        public bool DisTcpConnectToServer()
        {
            //判断协议类型
            //if (ProtocolType == ProtocolType.Tcp)
            {
                if (ServerSocket != null && ServerSocket.Connected)
                {
                    try
                    {
                        // 禁用Socket的发送和接收
                        ServerSocket.Shutdown(SocketShutdown.Both);
                        Thread.Sleep(100);
                        //关闭TCP连接，释放Socket
                        ServerSocket.Close();
                    }
                    catch
                    {
                        //断开连接发生异常，返回连接仍在保持
                        return false;
                    }

                }
                //标识与服务器的连接已经断开
                IsConnected = false;
                ServerSocket = null;
                //刷新属性MVVM
                //OnPropertyChanged<bool>(() => IsConnected);
                //OnPropertyChanged(() => ServerActStr);

                return true;//返回连接已经断开
            }
            return false;
        }

        #endregion TCP客户端与服务器：建立连接与断开连接

        #region 发送与接收数据

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="buffer">待发送的数据</param>
        /// <param name="contentLength">需要发送内容的长度</param>
        /// <param name="sendLength">实际发送的长度</param>
        /// <returns>状态码</returns>
        public CommunicationStateCode SendData(ref byte[] buffer, ref Int32 contentLength, ref Int32 sendLength)
        {
            try
            {
                if (ProtocolType == ProtocolType.Tcp)
                {
                    if (!IsConnected)
                    {
                        return CommunicationStateCode.SendDataFailed;
                    }
                    //设置Socket忙碌，准备发送数据
                    IsBusy = true;
                    // _logger.Info("TCP发送："+ ConvertHelper.ByteToHexStr(buffer));
                    sendLength = ServerSocket.Send(buffer, contentLength, SocketFlags.None);
                    //设置Socket不忙碌
                    IsBusy = false;
                    return CommunicationStateCode.SendDataSucceed;
                }
                else if (ProtocolType == ProtocolType.Udp)
                {
                    if (ServerSocket == null || IsBusy)
                    {
                        return CommunicationStateCode.SendDataFailed;//返回发送数据失败
                    }
                    //设置Socket忙碌，准备发送数据
                    IsBusy = true;
                    // _logger.Info("UDP发送：" + ConvertHelper.ByteToHexStr(buffer));
                    sendLength = ServerSocket.SendTo(buffer, 0, contentLength, SocketFlags.None, ServerPoint);
                    //设置Socket不忙碌
                    IsBusy = false;
                    return CommunicationStateCode.SendDataSucceed;
                }

                //协议未知
                return CommunicationStateCode.ProtocolUnknow;
            }
            //数据发送异常
            catch (Exception ex)
            {
                IsConnected = false;
                IsBusy = false;
                //_logger.Info("发送出错：" + ex.ToString());
                DisTcpConnectToServer();
                return CommunicationStateCode.SendDataOccurException;
            }
        }

        /// <summary>
        /// 接收从服务器发送过来的数据
        /// </summary>
        /// <param name="receiveBytes">接收到的字节数组</param>
        /// <param name="receiveLength">接收到的长度</param>
        /// <param name="timeOut">设置接收超时时间</param>
        /// <returns></returns>
        public CommunicationStateCode ReceiveData(ref byte[] receiveBytes, ref Int32 receiveLength, int timeOut)
        {
            //设置接收超时时间
            ServerSocket.ReceiveTimeout = timeOut;
            //定义缓冲区
            byte[] buffer = new byte[1024 * 1024];
            try
            {
                if (ProtocolType == ProtocolType.Tcp && IsConnected)
                {
                    //读取指定数量的数据到缓冲区中，返回实际读取到的数据量
                    receiveLength = ServerSocket.Receive(buffer);
                    //读到了数据
                    if (receiveLength > 0)
                    {
                        //如果传进来的用于接收的字节数组为空或者大小不够，则重新初始化为接收到的长度大小
                        if (receiveBytes == null || receiveBytes.Length < receiveLength)
                        {
                            receiveBytes = new byte[receiveLength];
                        }
                        //拷贝数组
                        Array.Copy(buffer, 0, receiveBytes, 0, receiveLength);
                        //_logger.Info("返回数据：" + ConvertHelper.ByteToHexStr(receiveBytes));
                        //接收数据完成，设置不忙了
                        IsBusy = false;
                        //返回成功信息
                        return CommunicationStateCode.ReceiveDataSuccess;
                    }
                    //_logger.Info("没有读取到返回数据");
                    //没有读取到数据
                    return CommunicationStateCode.ReceiveDataIsNull;
                }
                if (ProtocolType == ProtocolType.Udp)
                {
                    //接收从服务器返回的数据
                    EndPoint p = ServerPoint;
                    receiveLength = ServerSocket.ReceiveFrom(buffer, ref p);
                    ServerPoint = (IPEndPoint)p;
                    //读到了数据
                    if (receiveLength > 0)
                    {
                        //如果传进来的用于接收的字节数组为空或者大小不够，则重新初始化为接收到的长度大小
                        if (receiveBytes == null || receiveBytes.Length < receiveLength)
                        {
                            receiveBytes = new byte[receiveLength];
                        }
                        //拷贝数组
                        Array.Copy(buffer, 0, receiveBytes, 0, receiveLength);
                        //接收数据完成，设置不忙了
                        IsBusy = false;
                        //返回成功信息
                        return CommunicationStateCode.ReceiveDataSuccess;
                    }
                    //没有读取到数据
                    return CommunicationStateCode.ReceiveDataIsNull;
                }
                //返回协议未知
                return CommunicationStateCode.ProtocolUnknow;
            }
            catch (Exception e)
            {
                if (e.HResult == -2147467259)
                {
                    return CommunicationStateCode.TimeOut;//超时
                }
                //发生异常：返回错误信息
                return CommunicationStateCode.ReceiveDataOccurException;
            }
        }

        #endregion 发送与接收数据
    }
}