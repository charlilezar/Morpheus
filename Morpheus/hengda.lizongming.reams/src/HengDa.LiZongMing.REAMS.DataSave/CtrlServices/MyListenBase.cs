using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aming.DTU
{
    public class MyListenBase
    {
        public event ShowMsgAsynCallBack ShowErrAsynNotice = null;
        public event ShowMsgAsynCallBack ShowLogAsynNotice = null;
        public event MyListenStopedCallBack MyListenStopedNotice = null;
        public Thread _thread = null;
        public bool Running = false;
        public bool Listening_IsErr = false;
        public bool Inputered = false;
        public string Listening_Err;
        public string MyListenName = "";
        public int SleepTime = 100;
        /// <summary>
        /// 用于标识是否是通过函数StopListenning终止线程的，每次StartListenning都会复位该字段
        /// </summary>
        public bool _Stopped = false;
        public short _StoppedCase = 0;
        public MyListenBase()
        {
        }
        #region 线程控制Listening
        public virtual bool StartListenning(out string sErr)
        {
            if (this.Running)
            {
                sErr = "[" + this.MyListenName + "]测试结果的监听已经开启，请勿重复打开。";
                return false;
            }
            Listening_Err = string.Empty;
            if (_Stopped)
                _Stopped = false;
            _thread = new System.Threading.Thread(new System.Threading.ThreadStart(Listen));
            _thread.IsBackground = true;
            try
            {
                _thread.Start();
            }
            catch (Exception ex)
            {
                sErr = string.Format(this.MyListenName + "启动时出错：{0}({1})", ex.Message, ex.Source);
                return false;
            }
            sErr = string.Empty;
            return true;
        }
        public virtual void Listen()
        {
            if (this.IsShowLog)
            {
                if (this.ShowLogAsynNotice != null)
                    this.ShowLogAsynNotice($"监听已启动。");
            }
            this.Running = true;
            this.Listening();
            this.Running = false;
            if (this.IsShowLog)
            {
                if (this.ShowLogAsynNotice != null)
                    this.ShowLogAsynNotice($"监听已退出。");
            }
            this.CallMyListenStopedAsyn(this._StoppedCase);
        }

        public virtual void Listening()
        {
            while (true)
            {
                if (!this.Running)
                {
                    this.ShowLogAsyn("[" + this.MyListenName + "]由于Running为false，监听程序中断。");
                    return;
                }
                this.ListeningProData();
                //程序休眠
                Thread.Sleep(this.SleepTime);

            }
        }
        public virtual void ListeningProData()
        {

        }
        public virtual void StopListenning(short iName)
        {
            this._StoppedCase = iName;
            this._Stopped = true;
            this.Running = false;
        }

        #endregion
        public void SetInputered(bool blInputered)
        {
            if (this.Inputered != blInputered)
                this.Inputered = blInputered;
        }
        #region 通知主线程
        public void CallMyListenStopedAsyn(short iName)
        {
            if (this.MyListenStopedNotice == null) return;
            this.MyListenStopedNotice(iName);
        }
        private void CallMyListenStoped(short iName)
        {
            
        }
        #endregion
        #region 消息
        public bool IsShowLog = true;
        public bool IsShowErr = false;
        public virtual void ShowErrAsyn(string sMsg)
        {
            if (this.ShowErrAsynNotice != null)
                this.ShowErrAsynNotice(sMsg);
        }
       
        public virtual void ShowLogAsyn(string sMsg)
        {
            if (!IsShowLog) return;
            if (this.ShowLogAsynNotice != null)
                this.ShowLogAsynNotice(sMsg);

        }
        #endregion
        #region 相关delegate
        public delegate void ShowMsgAsynCallBack(string sMsg);
        public delegate void MyListenStopedCallBack(short iName);
        #endregion
    }
}
