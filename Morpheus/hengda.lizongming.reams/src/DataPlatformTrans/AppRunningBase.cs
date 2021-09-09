using Cowboy.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DataPlatformTrans
{
    public class AppRunningBase
    {
        public event MyListenStopedCallBack MyListenStopedNotice = null;
        public bool Running = false;
        public bool Listening_IsErr = false;
        public bool Inputered = false;
        public string Listening_Err;
        public string MyListenName = "";
        public int SleepTime = 50;
        
        /// <summary>
        /// 用于标识是否是通过函数StopListenning终止线程的，每次StartListenning都会复位该字段
        /// </summary>
        public bool _Stopped = false;
        public short _StoppedCase = 0;
        public AppRunningBase()
        {
        }
        #region 线程控制Listening
        public virtual bool StartListenning(out string sErr)
        {
            if (this.Running)
            {
                sErr = "[" + this.MyListenName + "]线程已经开启，请勿重复打开。";
                return false;
            }
            Listening_Err = string.Empty;
            if (_Stopped)
                _Stopped = false;
            this.Listen();
            sErr = string.Empty;
            return true;
        }
        public virtual async void Listen()
        {
            this.Running = true;
            await this.Listening();
            this.Running = false;
            this.CallMyListenStopedAsyn(this._StoppedCase);
        }

        public virtual async Task Listening()
        {
            while (true)
            {
                if (!this.Running)
                {
                    //this.ShowLog("[" + this.MyListenName + "]由于Running为false，线程终止。");
                    return;
                }
                await this.ProListening();
                //程序休眠
                if (this.SleepTime > 0)
                    await Task.Delay(this.SleepTime);
            }
        }
        public virtual async Task ProListening()
        {
            await Task.Delay(1000);//无意义的，该函数是必须被重写的
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
        #endregion
        public delegate void MyListenStopedCallBack(short iName);

    }
    
    
}
