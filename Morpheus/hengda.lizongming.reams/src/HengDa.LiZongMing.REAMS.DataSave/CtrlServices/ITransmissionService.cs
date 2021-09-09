using Aming.Core;
using Aming.DTU.Config;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aming.DTU
{
    /// <summary>
    /// 数据传输层接口
    /// </summary>
    public interface ITransmissionService : IDisposable
    {
        IDataEndPoint Config { get; set; }
        ILogger Logger { get; set; }

        //void Dispose();

        bool IsConnected { get; }

        /// <summary>
        /// 分包和粘包操作时用的包完整检查。mqtt不用
        /// </summary>
        Func<List<byte>, int> funcCheckPackFormat { get; set; }


        /// <summary>
        /// 打开端口
        /// </summary>
        /// <returns></returns>
        Task<bool> TryOpenAsync();

        /// <summary>
        /// 发送信息并等待返回
        /// </summary>
        /// <param name="data">数据主体内容</param>
        /// <param name="title">说明性标题</param>
        /// <param name="waitTimeout">最长等待时间</param>
        /// <returns></returns>
        [Obsolete("请用异步方法，不要阻塞线程等回复.")]
        Task<IotResult<UniMessageBase>> SendAndWaitReply(byte[] data, string title = "", int waitTimeout = 3000);

        /// <summary>
        /// 发送信息，不等返回
        /// </summary>
        /// <param name="data"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        Task<IotResult<UniMessageBase>> SendAsync(byte[] data, string title = "");

        /// <summary>
        /// 事件回调时，收到数据触发解析
        /// </summary>
        event ReceivedUniMessageCallBack OnUniMessageReceived;

        /// <summary>
        /// 将本端数据转发到别的端口的处理程序
        /// </summary>
        event ReceivedUniMessageCallBack OnDataForwardTo;


        Task<IotResult<UniMessageBase>> SendUniMessage(UniMessageBase msg);

    }
}