using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataPlatformTrans.DataEntitys.MessageEntity
{
    public class RtnMessage
    {
        /// <summary>
        /// 实例化一个成功的消息对象
        /// </summary>
        public RtnMessage()
        {
            this.Sucessful = true;//默认就为成功，一般成功无返回消息
        }
        /// <summary>
        /// 实例错误消息对象
        /// </summary>
        /// <param name="sMsg">错误消息</param>
        public RtnMessage(string sMsg)
        {
            this.Sucessful =false;
            this.Msg = sMsg;
        }
        public string Msg { get; set; }
        public bool Sucessful { get; set; }
    }
    public class RtnMessage<T> : RtnMessage
    {
        public RtnMessage()
        {
            
        }
        public RtnMessage(T result)
        {
            this.Msg = string.Empty;
            this.Sucessful = true;
            this.Result = result;
        }
        public RtnMessage(string sErr)
        {
            this.Msg = sErr;
            this.Sucessful = false;
        }
        public T Result { get; set; }
    }
    /// <summary>
    /// 交互过程中的QntRtn值描述
    /// </summary>
    public enum QnRtnValues
    {
        未知 = 0,
        准备执行请求 =1,
        请求被拒绝=2,
        密码错误=3
    }
    public enum ExeRtnValues
    {
        未知=0,
        执行成功 = 1,
        执行失败但不知道原因 = 2,
        数据包校验错误 = 3,
        身份验证时密码错误=4,
        没有数据=5
    }
    public class AppLogger
    {
        public ILogger Logger = null;
        public event ShowMsgAsynCallBack ShowErrAsynNotice = null;
        public event ShowMsgAsynCallBack ShowLogAsynNotice = null;
        public bool IsShowLog { get; set; } = true;
        public bool IsShowErr { get; set; } = true;
        public void ShowLog(object sender,string sMsg)
        {
            if (!this.IsShowLog) return;
            this.ShowLogAsynNotice?.Invoke(sMsg);
            //存储到本地日志文件
            if (this.Logger != null)
                this.Logger.LogDebug(sMsg);
        }
        public void ShowWarning(object sender, string sMsg)
        {
            if (!this.IsShowLog) return;
            this.ShowLogAsynNotice?.Invoke(sMsg);
            //存储到本地日志文件
            if (this.Logger != null)
                this.Logger.LogWarning(sMsg);
        }
        public void ShowErr(object sender,string sMsg)
        {
            if (!this.IsShowErr) return;
            this.ShowErrAsynNotice?.Invoke(sMsg);
            //存储到本地日志文件
            //??
            if (this.Logger != null)
                this.Logger.LogError(sMsg);
        }
        public void ShowErr(object sender, string sTitle,Exception  exception)
        {
            
        }
        #region 相关delegate
        public enum LogLevel
        {
            Debug=1,
            Warn=2,
            Error=3
        }
        public delegate void ShowMsgAsynCallBack(string sMsg);
        public delegate void MyListenStopedCallBack(short iName);
        #endregion
    }
}
