using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;


namespace Aming.Core
{

    /// <summary>
    /// 控制硬件时的命令执行结果反馈
    /// </summary>
    public class IotResult<T>
    {


        public IotResult(bool ok=false, string msg = "")
        {
            this.Ok = ok;
            Message = msg ?? "";
            //this.Dic = new Dictionary<string, object>();
        }
        public bool Ok { get; set; } = false;
        public string Message { get; set; }
        //public string Url { get; set; }

        public T Result { get; set; } 

        #region 动态属性
        ///// <summary>
        ///// 获取运行时动态解析对象的内部字典数据。
        ///// 要想能序列化，这个属性必须public
        ///// </summary>
        //public Dictionary<string, object> Dic
        //{
        //    get;
        //    private set;
        //}
        #endregion
        public static IotResult<T> Error(string msg = "")
        {
            return new IotResult<T>(false,msg);
    
        }
        public static IotResult<T> Success(T rs, string msg = "")
        {
            return new IotResult<T>(true, msg) {
                Result = rs
            };

        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("Success=").Append(this.Ok?"true":"false");
            if (!string.IsNullOrEmpty(Message))
                sb.Append(",ErrorMsg=").Append(Message);
            if (Result != null)
                sb.Append(",result=").Append(Result);
            return sb.ToString();
        }
    }
}