using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Cowboy.Sockets;
using DataPlatformTrans.DataEntitys.MessageEntity;

namespace DataPlatformTrans.DataEntitys
{
    public class TransConnect
    {
        Action<string, TransConnect> _ActionReceive = null;
        TransConfig _TransConfig = null;
        AsyncTcpSocketClient _client = null;
        /// <summary>
        /// 当前是否连接
        /// </summary>
        public bool Connected
        {
            get {
                if (this._client == null) return false;
                return this._client.State == TcpSocketConnectionState.Connected;
            }
        }
        public TransConnect(TransConfig config, Action<string, TransConnect> actionRecieve)
        {
            this._TransConfig = config;
            this._ActionReceive = actionRecieve;
        }
        public async Task<RtnMessage> ConnectAsync()
        {
            if (this._client == null)
            {
                if (this._TransConfig == null)
                {
                    return new RtnMessage("服务器连接配置信息为空，无法连接服务器。");
                }
                var tcpConfig = new AsyncTcpSocketClientConfiguration();
                tcpConfig.FrameBuilder = new RawBufferFrameBuilder();
                tcpConfig.ConnectTimeout = new TimeSpan(this._TransConfig.TimeOut * 1000);//将毫秒转换成ticks，1ticks等于100纳秒，1纳秒=1000,000毫秒；所以这里是*10000
                _client = new AsyncTcpSocketClient(this._TransConfig.IPEndPoint, OnServerDataReceived, OnServerConnected, OnServerDisconnected, tcpConfig);
            }
            if (_client.State == TcpSocketConnectionState.Connected)
            {
                return new RtnMessage<string>();
            }
            //此时打开连接
            try
            {
                await this._client.Connect();
            }
            catch (Exception ex)
            {
                return new RtnMessage($"Sockt连接出错：{ex.Message}({ex.StackTrace})");
            }
            return new RtnMessage();
        }
        #region CowByStock所需事件
        DateTime? _LastReceivedTime = null;
        string _RecievedData = string.Empty;
        public async Task OnServerDataReceived(AsyncTcpSocketClient client, byte[] dataBuff, int offset, int count)
        {
            if (this._LastReceivedTime != null && !String.IsNullOrEmpty(this._RecievedData) && (DateTime.Now - (DateTime)this._LastReceivedTime).TotalMilliseconds > this._TransConfig.ReceivedTimeOut)
            {
                this._RecievedData = string.Empty;
            }
            byte[] data = new byte[count];
            Array.Copy(dataBuff, offset, data, 0, count); //只能从缓冲区取一段
            this._RecievedData += Encoding.ASCII.GetString(data);
            if (this._RecievedData.EndsWith("\r\n"))
            {
                //如果结尾是回车符号的，则表示接收完成了
                if (this._ActionReceive != null)
                    _ActionReceive(this._RecievedData, this);
                //清空数据
                this._RecievedData = string.Empty;
                //时间也要清空，这样下次再收到数据就是新的开始了；
                if (this._LastReceivedTime != null)
                    this._LastReceivedTime = null;
            }
            else
            {
                this._LastReceivedTime = DateTime.Now;//此时记录下时间，以便接收超时判断
            }
            await Task.CompletedTask;
        }
        public async Task OnServerConnected(AsyncTcpSocketClient client)
        {
            await Task.CompletedTask;
        }
        public async Task OnServerDisconnected(AsyncTcpSocketClient client)
        {
            await Task.CompletedTask;
        }
        #endregion
        public async Task<RtnMessage> SendDataAsync(string sData)
        {
            if(String.IsNullOrEmpty(sData))
            {
                return new RtnMessage("要发送是的数据为空，发送终止！");
            }
            try
            {
                await _client.SendAsync(Encoding.ASCII.GetBytes(sData));
            }
            catch (Exception ex)
            {
                return new RtnMessage(string.Format($"发送数据出错：{0}({1})，数据内容[{2}]", ex.Message, ex.Source, sData));
            }
            return new RtnMessage();
        }
        
    }
    public class TransConfig
    {
        /// <summary>
        /// 目标地址的IP和端口号信息
        /// </summary>
        public IPEndPoint IPEndPoint { get; set; }
        /// <summary>
        /// socket通讯超时，单位：毫秒
        /// </summary>
        public int TimeOut { get; set; } = 3000;
        /// <summary>
        /// 接收超时时间设置，单位：毫秒，如果距离上一次接收时间间隔超过该设定值，则被认定为新的数据；
        /// </summary>
        public int ReceivedTimeOut { get; set; } = 2000;
    }
}
