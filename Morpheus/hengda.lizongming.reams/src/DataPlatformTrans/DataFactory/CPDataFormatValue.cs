using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataPlatformTrans.DataFactory
{
    public class CPDataFormatValue
    {
        /// <summary>
        /// 系统所有字段集合
        /// </summary>
        public Dictionary<string, DataFormat> Columns = null;
        /// <summary>
        /// 添加固定长度的字符窜，不足位数前面加0
        /// </summary>
        /// <param name="iFixedLen"></param>
        public void AddFixedLenString(string sColName, int iFixedLen)
        {
            if (this.Columns == null)
                this.Columns = new Dictionary<string, DataFormat>();
            DataFormat format = new DataFormat();
            format.DataType = DataType.String;
            format.FixedLen = true;
            format.MaxLenth = iFixedLen;
            this.Columns.Add(sColName, format);
        }
        public void AddUnFixedLenString(string sColName, int iMaxLen)
        {
            if (this.Columns == null)
                this.Columns = new Dictionary<string, DataFormat>();
            DataFormat format = new DataFormat();
            format.DataType = DataType.String;
            format.FixedLen = false;
            format.MaxLenth = iMaxLen;
            this.Columns.Add(sColName, format);
        }
        /// <summary>
        /// 添加小数类型样式
        /// </summary>
        /// <param name="iIntegerLen">证书部分最大长度</param>
        /// <param name="iDecimalCnt">小数部分长度</param>
        public void AddDecimal(string sColName,int iIntegerLen, int iDecimalCnt)
        {
            if (this.Columns == null)
                this.Columns = new Dictionary<string, DataFormat>();
            DataFormat format = new DataFormat();
            format.DataType = DataType.Decimal;
            format.FixedLen = false;
            format.MaxLenth = iIntegerLen + iDecimalCnt + 2;//加2是因为，一个是小数点，一个是正负符号，可以包含“-”，“+”可以省略
            //这里文档上没确认，但猜测是这样的，如果数值是0.212，不能写成.2944，前面的0不能去除，示例中有xxx-LowValue=0.6,这样的
            //所以这里的掩码一定要注意整数部分最后一个要用0
            if (iIntegerLen > 1)
                format.Format = string.Empty.PadLeft(iIntegerLen - 1, '#');
            format.Format += "0";
            if(iDecimalCnt>0)
            {
                format.Format += "." + string.Empty.PadLeft(iDecimalCnt, '0');//目前无论小数部分是否0，都显示出来
            }
            this.Columns.Add(sColName, format);
        }
        public void AddInteger(string sColName, int iIntegerLen, int iDecimalCnt)
        {
            if (this.Columns == null)
                this.Columns = new Dictionary<string, DataFormat>();
            DataFormat format = new DataFormat();
            format.DataType = DataType.Integer;
            format.FixedLen = false;
            format.MaxLenth = iIntegerLen + iDecimalCnt + 2;//加2是因为，一个是小数点，一个是正负符号，可以包含“-”，“+”可以省略
            format.Format = string.Empty.PadLeft(iIntegerLen, '#');
            if (iDecimalCnt > 0)
            {
                format.Format += "." + string.Empty.PadLeft(iDecimalCnt, '0');//目前无论小数部分是否0，都显示出来
            }
            this.Columns.Add(sColName, format);
        }
        public void AddDateTime(string sColName,string sFormat)
        {
            if (this.Columns == null)
                this.Columns = new Dictionary<string, DataFormat>();
            DataFormat format = new DataFormat();
            format.DataType = DataType.DateTime;
            format.Format = sFormat;
            format.MaxLenth = sFormat.Length;
            this.Columns.Add(sColName, format);
        }
        public void AddBoolean(string sColName)
        {
            if (this.Columns == null)
                this.Columns = new Dictionary<string, DataFormat>();
            DataFormat format = new DataFormat();
            format.DataType = DataType.Boolean1_0;
            format.MaxLenth = 1;
            this.Columns.Add(sColName, format);
        }
        #region 格式化数据
        public static CPDataFormatValue _CPDataFormatValue = null;
        /// <summary>
        /// 添加系统用到的字段类型，该函数可以在用到时调用，也可以系统初始化时调用
        /// </summary>
        private static void InitCPDataFormatValue()
        {
            //这些字段由于是固定的，而且数量也固定，我们就写死在程序中，不进行外部配置，因为引入外部配置反而会认为改动导致出错。
            //字段来源参考协议第12页，7-4；
            _CPDataFormatValue = new CPDataFormatValue();
            _CPDataFormatValue.AddDateTime("SystemTime", "yyyyMMddhhmmss");
            _CPDataFormatValue.AddDateTime("AlarmTime", "yyyyMMddhhmmss");
            _CPDataFormatValue.AddBoolean("AlarmType");
            _CPDataFormatValue.AddDateTime("BeginTime", "yyyyMMddhhmmss");
            _CPDataFormatValue.AddDateTime("EndTime", "yyyyMMddhhmmss");
            _CPDataFormatValue.AddDateTime("DataTime", "yyyyMMddhhmmss");
            _CPDataFormatValue.AddDateTime("ReportTime", "yyyyMMddhhmmss");
            _CPDataFormatValue.AddFixedLenString("WorkID", 19);
            _CPDataFormatValue.AddDecimal("WorkFlow", 14, 2);
            _CPDataFormatValue.AddDecimal("0901040101", 4,2);//温度
            _CPDataFormatValue.AddDecimal("0901040102",4 ,2);//湿度
            _CPDataFormatValue.AddDecimal("0901040103", 4, 2);//气压
            _CPDataFormatValue.AddDecimal("0901040104", 4, 2);//降水（雨、雪）强度
            _CPDataFormatValue.AddDecimal("0901040105", 4, 2);//风向
            _CPDataFormatValue.AddDecimal("0901040106", 4, 2);//风速
            _CPDataFormatValue.AddBoolean("0901040107");//感雨
            _CPDataFormatValue.AddDecimal("0102060301",14,2);//剂量率
            _CPDataFormatValue.AddUnFixedLenString("0102060332", 42);//N42最大长度为42的xml格式，目前是碘化钠的上传数据
            _CPDataFormatValue.AddDecimal("0102069901", 4 ,2);//电池电压
            _CPDataFormatValue.AddDecimal("0102069902", 4, 2);//Garma计高压
            _CPDataFormatValue.AddDecimal("0102069903", 4, 2);//Garma计温度
            _CPDataFormatValue.AddDecimal("0102069904", 14, 2);//瞬时采样流量
            _CPDataFormatValue.AddDecimal("0102069905", 14, 2);//累计采样体积
            _CPDataFormatValue.AddBoolean("0102069906");//门禁状态
            _CPDataFormatValue.AddDecimal("0102069907", 4, 2);//室内温度

            _CPDataFormatValue.AddBoolean("0102069909");//供电状态
            _CPDataFormatValue.AddDecimal("0102069911", 4, 2);//站房内湿度
            _CPDataFormatValue.AddBoolean("0102069912");//站房烟感报警
            _CPDataFormatValue.AddBoolean("0102069913");//站房浸水报警
            _CPDataFormatValue.AddDecimal("0102069914", 4, 2);//电池备时间
        }
        /// <summary>
        /// 格式化数据
        /// </summary>
        /// <param name="objValue"></param>
        /// <param name="sColName"></param>
        /// <param name="sResult"></param>
        /// <param name="sErr"></param>
        /// <returns></returns>
        public static bool TryFormatResult(object objValue, string sColName, out string sResult, out string sErr)
        {
            if (_CPDataFormatValue == null || _CPDataFormatValue.Columns == null) InitCPDataFormatValue();//初始化加载
            DataFormat format;
            if (!_CPDataFormatValue.Columns.TryGetValue(sColName, out format))
            {
                sResult = string.Empty;
                sErr = $"未找到字段[{sColName}]的数据格式！";
                return false;
            }
            if (objValue == null || objValue.ToString() == string.Empty)
            {
                //如果允许空的，在没什么问题，返回NULL就可以了
                if (format.AllowEmpty)
                {
                    sErr = string.Empty;
                    sResult = string.Empty;
                    return true;
                }
                sErr = $"传入的字段[{sColName}]值为空，但已经被定义为不能为空，格式化失败！";
                sResult = string.Empty;
                return false;
            }
            //sErr = string.Empty;
            if (format.DataType == DataType.String)
            {
                sResult = objValue.ToString();
            }
            else if (format.DataType == DataType.DateTime)
            {
                if (format.Format.Length == 0)
                {
                    sErr = $"字段[{sColName}]的数据格式位时间，但还未设置日期掩码内容！";
                    sResult = string.Empty;
                    return false;
                }
                DateTime detValue;
                if (objValue.GetType() == typeof(DateTime))
                {
                    //直接拆箱，因为本来就是DateTime，所以可定不会错；
                    detValue = (DateTime)objValue;
                }
                else
                {
                    //此时可能穿入的是字符窜，有可能格式不正确
                    if (!DateTime.TryParse(objValue.ToString(), out detValue))
                    {
                        sErr = $"字段[{sColName}]的数据内容[{objValue.ToString()}]不是有效的时间格式，格式化失败！";
                        sResult = string.Empty;
                        return false;
                    }
                }
                sResult = detValue.ToString(format.Format);
            }
            else if (format.DataType == DataType.Decimal)
            {
                if (format.Format.Length == 0)
                {
                    sErr = $"字段[{sColName}]的数据格式为decimal，但还未设置数据掩码内容！";
                    sResult = string.Empty;
                    return false;
                }
                decimal decValue;
                if (objValue.GetType() == typeof(decimal))
                {
                    decValue = (decimal)objValue;
                }
                else if (objValue.GetType() == typeof(float))
                {
                    decValue = (decimal)(float)objValue;
                }
                else if (objValue.GetType() == typeof(double))
                {
                    decValue = (decimal)(double)objValue;
                }
                else
                {
                    if (!decimal.TryParse(objValue.ToString(), out decValue))
                    {
                        sErr = $"字段[{sColName}]的数据内容[{objValue.ToString()}]不是有效的数值类型,其类型为[{objValue.GetType()}]，格式化失败！";
                        sResult = string.Empty;
                        return false;
                    }
                }
                sResult = decValue.ToString(format.Format);
            }
            else if (format.DataType == DataType.Boolean1_0)
            {
                //注意：该类型是返回1或0,1表示真，0表示否；
                bool blValue;
                if (objValue.GetType() == typeof(bool))
                {
                    blValue = (bool)objValue;
                    sResult = blValue ? "1" : "0";
                }
                else
                {
                    //目前此种类型必须是布尔型，否则容易出错
                    //超过也是不允许的
                    sErr = $"字段[{sColName}]要求是布尔型，但传入的值类型为[{objValue.GetType()}]，格式化失败！";
                    sResult = string.Empty;
                    return false;
                }
            }
            else if (format.DataType == DataType.Integer)
            {
                //此时为整型，但不明确一定要是几位整型以及是否有符号；
                int iValue;
                if (!int.TryParse(objValue.ToString(), out iValue))
                {
                    sErr = $"字段[{sColName}]要求是整数，但传入的值[{objValue.ToString()}]，类型[{objValue.GetType()}]无法转化成整数，格式化失败！";
                    sResult = string.Empty;
                    return false;
                }
                sResult = iValue.ToString();
            }
            else
            {
                sErr = $"字段[{sColName}]的数据格定义的格式为[{format.DataType}]，但系统还未实现该类型的格式化；";
                sResult = string.Empty;
                return false;
            }
            if (sResult.Length > format.MaxLenth)
            {
                //超过也是不允许的
                sErr = $"字段[{sColName}][{format.DataType}]的数据格式要求不超过[{format.MaxLenth}]位字符窜，但解析后的数据[{sResult}]超过了该长度！";
                return false;
            }
            // 如果是固定长度的则要补充0
            if (format.FixedLen && sResult.Length < format.MaxLenth)
                sResult = sResult.PadLeft(format.MaxLenth, '0');
            sErr = string.Empty;
            return true;
        }
        #endregion
    }
    public class DataFormat
    {
        /// <summary>
        /// 字段类型
        /// </summary>
        public DataType DataType { get; set; }
        /// <summary>
        /// 允许的最大长度
        /// </summary>
        public int MaxLenth { get; set; }
        /// <summary>
        /// 固定长度
        /// </summary>
        public bool FixedLen { get; set; }
        /// <summary>
        /// 数据格式化，除了字符窜外其他都需要
        /// </summary>
        public string Format { get; set; } = "";
        /// <summary>
        /// 是否允许空，默认是允许的
        /// </summary>
        public bool AllowEmpty { get; set; } = true;
    }
    public enum DataType
    {
        String=0,
        Decimal=1,
        Integer = 2,
        DateTime=3,
        /// <summary>
        /// 布尔值，返回1或0
        /// </summary>
        Boolean1_0=4
    }
}
