using System;
using System.Runtime.Serialization;
using System.Text;


namespace Aming.Core
{
 
    
    /// <summary>
    /// 控制硬件时的命令执行结果反馈
    /// </summary>
    public class CommandExecResult
    {
        public const string SUCCESS = "SUCCESS";
        public const string FAIL = "FAIL";
        public const string ERROR = "ERROR";

        private object _result;

        public string ErrorCode { get; internal set; }

        public Exception Exception { get; internal set; }

        public bool IsCancelled => ErrorCode == "error.cancelled";

        public bool IsTimeout => ErrorCode == "error.timeout";

        /// <summary>
        /// 连接失败
        /// </summary>
        public bool IsConnectFail => ErrorCode == "error.connectfailed";

        public static CommandExecResult Success()
        {
            return new CommandExecResult(SUCCESS);
        }

        public static CommandExecResult Error(string code = null)
        {
            return new CommandExecResult(ERROR)
            {
                ErrorCode = code
            };
        }

        public static CommandExecResult Null => Success();

        /// <summary>
        /// 已取消
        /// </summary>
        /// <returns></returns>
        public static CommandExecResult Cancelled()
        {
            return Error("error.cancelled");
        }

        /// <summary>
        /// 请求超时
        /// </summary>
        /// <returns></returns>
        public static CommandExecResult Timeout()
        {
            return Error("error.timeout");
        }

        /// <summary>
        /// 连接失败
        /// </summary>
        /// <returns></returns>
        public static CommandExecResult ConnectFailed()
        {
            return Error("error.connectfailed");
        }

        public CommandExecResult(object result = null)
        {
            _result = result;
        }

        public bool IsSuccess
        {
            get
            {
                if (Exception != null) return false;
                if (ErrorCode != null) return false;
                if (_result == null) return false;

                if (_result is string)
                {
                    if (FAIL.Equals(_result.ToString().ToUpper())) return false;
                    if (ERROR.Equals(_result.ToString().ToUpper())) return false;
                }

                // TODO 其他判定位false的条件
                return true;
            }
        }

        public string ErrorMsg
        {
            get
            {
                if (string.IsNullOrEmpty(_ErrorMsg)) return ErrorCode;
                return _ErrorMsg;
            }
            set { _ErrorMsg = value; }
        }
        string _ErrorMsg = string.Empty;

        public T Result<T>()
        {
            if (_result == null) return default(T);
            if (_result is T variable) return variable;
            var res = MyTypeConverter.ChangeType<T>(_result);
            _result = res;
            return res;
        }

        internal void UpdateResult(object res)
        {
            _result = res;
        }

        /// <summary>
        /// 将16进制的结果转换为10进制
        /// 比如:_result=1A2B=>6699
        /// </summary>
        /// <returns></returns>
        public int IntResultFromHexStr(int def = 0)
        {
            if (_result == null) return def;
            var resultStr = _result.ToString();
            return Convert.ToInt32(resultStr, 16);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("Success=").Append(IsSuccess);
            if (!string.IsNullOrEmpty(ErrorCode))
                sb.Append(",ErrorCode=").Append(ErrorCode);
            if (!string.IsNullOrEmpty(ErrorMsg))
                sb.Append(",ErrorMsg=").Append(ErrorMsg);
            if (_result != null)
                sb.Append(",result=").Append(_result);
            return sb.ToString();
        }
    }
}