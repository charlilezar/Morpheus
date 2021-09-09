using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aming.Core;

namespace HengDa.LiZongMing.REAMS.Wpf.Dto
{
    public class SSBaseDto:BaseDto
    {
        [Description("命令值")]
        public HDSSCmdCode Cmd { get; set; }
    }
    #region 实时数据
    public class SSRealDataDto:SSBaseDto
    {
        public SSRealDataDto()
        {

        }
        public SSRealDataDto(byte[] data, HDSSCmdCode cmd, ushort iMacNo)
        {
            /***********
                 * 解析数据，返回的数据长度为5+2*26（请求26个寄存器地址）=57
                 * 返回的存储寄存器值的数据长度=2*26=52个字节，所以返回的第三个字节值为52
                 * ******************/
            this.MacNo = iMacNo;
            this.Cmd = cmd;
            if (data.Length == 57 && 0x03==data[1] && 52==data[2])
            {
             
                this.Sucessfully = true;
            }
            else if(data.Length==5 && data[1]==0x83)
            {
                //此时返回错误消息
                this.ErrMsg = $"请求实时数据时，设备反馈错误，代码[{data[2]}]";
                this.Sucessfully = false;
            }
            else
            {
                this.ErrMsg = $"请求实时数据时，返回的数据不是预期的[{StringHelper.ByteToHexStr(data)}]";
                this.Sucessfully = false;
            }
        }
        #region 属性
        [Description("设备编号")]
        public int DeviceID { get; set; }

        [Description("经度")]
        public float Lng { get; set; }
        [Description("纬度")]
        public float Lat { get; set; }
        [Description("坐标类型")]
        public short CoordinateType { get; set; }
        [Description("节点编号")]
        public int NodeID { get; set; }
        [Description("温度")]
        public float Tem { get; set; }
        [Description("湿度")]
        public float Hum { get; set; }
        [Description("继电器状态")]
        public string RelayStatus { get; set; }
        [Description("浮点型数据")]
        public float FloatValue { get; set; }
        [Description("32位有符号数据")]
        public int SignedInt32Value { get; set; }
        [Description("32位无符号数据")]
        public uint UnSignedInt32Value { get; set; }
        #endregion
    }
    #endregion
    #region 索引日期设置反馈结果
    public class SSRainDetailSearchDateSetDto: SSBaseDto
    {
        public SSRainDetailSearchDateSetDto()
        {

        }
        public SSRainDetailSearchDateSetDto(byte[] data, HDSSCmdCode cmd, ushort iMacNo)
        {
            this.MacNo = iMacNo;
            this.Cmd = cmd;
            if (data.Length == 8)
            {
                if (data[1] != 0x10)
                {
                    this.ErrMsg = $"设置降雨明细索引时间时反馈的功能码[{data[1]}]不是预期的16";
                    this.Sucessfully = false;
                    return;
                }
                else if (data[2] != 0x00 && data[3] != 0x62 && data[4] != 0x00 && data[5] != 0x02)
                {
                    //此时判断读取反馈的寄存地址是否是我们写入的地址
                    this.ErrMsg = $"设置降雨明细索引时间时反馈的设置地址不是预期的{StringHelper.ByteToHexStr(data)}";
                    this.Sucessfully = false;
                    return;
                }
            }
            else if (data.Length == 5 && data[1] == 0x90)
            {
                //此时设备反馈出错
                //此时返回错误消息
                this.ErrMsg = $"写入降雨明细索引日期时，设备反馈错误，代码[{data[2]}]";
                this.Sucessfully = false;
                return;
            }
            else
            {
                this.ErrMsg = $"写入降雨明细索引日期时，返回的数据不是预期的[{StringHelper.ByteToHexStr(data)}]";
                this.Sucessfully = false;
                return;
            }
            this.Sucessfully = true;
        }
    }
    #endregion
    #region 索引日期内查询的降雨数据
    /// <summary>
    /// 索引日期内查询的降雨数据
    /// </summary>
    public class SSRainDataDto : SSBaseDto
    {
        public SSRainDataDto()
        {

        }
        public SSRainDataDto(byte[] data, HDSSCmdCode cmd, ushort iMacNo)
        {
            this.MacNo = iMacNo;
            this.Cmd = cmd;
            /*************
             * 降雨明细地址为
             * 通信协议上是：0x1f--0x27，第27个寄存器是备用的，所以发送请求时是0x1f--0x26；总共8个地址;
             * 通过连接设备测试后发现一定要包含0x27地址，否则设备不会把记录写入寄存器；所以读取的是0x1f--0x27，共9个字节
             * 应该是5+2*9=23个；
             * 返回的第3个字节应该是18，
             * ********************/
            if (data.Length==23 && 0x03==data[1] && 18==data[2])
            {
                //0x03为功能吗，16（十进制的）表示返回的字节数，因为我们请求8个地址的数据
                #region 正确解析数据
                //此时可以确定是反馈的正确数据了，但数据内容可能都是0，因为没有数据可以被查询的时候，设备会向寄存器地址中写入0
                /******
                 * 获取起始时间和结束时间，总共由3部分组成：
                 * 日期：这个是起始时间和结束时间公用的，存储在地址001FH和0020H中，001FH：D8-D15:年 D0-D7：月、0020H：D8-D15:日 D0-D7：无信息
                 * 开始降雨时间：这个是不包含日期的一个时间，存储在地址0021H中，精确到分钟，规则为：D8-D15:时 D0-D7: 分
                 * 结束降雨时间：这个是不包含日期的一个时间，存储在地址0022H中，精确到分钟，规则为：D8-D15:时 D0-D7: 分
                 * 降雨量:Float类型，总共占用2个寄存器地址，4个字节；接收数据为41 B4 CC CD时，实际值是22.6；而实际22.6转换成byte[]的话bytes[0]为CD、bytes[1]为CC、bytes[2]为B4、bytes[3]为41
                 * 降雨强度::Float类型，总共占用2个寄存器地址，4个字节；接收数据为41 2C CC CD时，实际值是10.8；而实际10.8转换成byte[]的话bytes[0]为CD、bytes[1]为CC、bytes[2]为2C、bytes[3]为41
                 * *******/
                int iIndex = 3;//从第三位开始读取
                int iY, iM, iD, iHour, iMinute;
                iY = (int)data[iIndex++] ;//地址001FH中第1个字节为年份
                iM = (int)data[iIndex++];//地址001FH中第2个字节为月份
                iD = (int)data[iIndex++];//地址0020H中第1个字节为日
                if (0 == iY && 0 == iM)
                {
                    //此时年\月为0的话，可以人为是不正确的数据，设备向寄存器中写入了0；
                    this.InvalidData = true;
                    this.ErrMsg = "";
                    return;
                }
                iIndex ++;//地址0020H中第2个字节为空的
                //开始读取起始时间
                iHour = (int)data[iIndex++];//地址0021H中第1个字节为小时
                iMinute = (int)data[iIndex++];//地址0021H中第2个字节为分钟
                //定义起始时间
                this.StartTime = new DateTime(iY + 2000, iM, iD, iHour, iMinute, 0);
                //开始读取结束时间
                iHour = (int)data[iIndex++];//地址0022H中第1个字节为小时
                iMinute = (int)data[iIndex++];//地址0022H中第2个字节为分钟
                //定义结束时间，注意传入的年份要加上2000
                this.EndTime = new DateTime(iY+2000, iM, iD, iHour, iMinute, 0);
                //降雨量：0023H、0024H
                this.Rainfall = (decimal)StringHelper.BytesToFloat(new byte[] { data[iIndex++], data[iIndex++], data[iIndex++], data[iIndex++] }, true);
                //降雨强度：0025H、0026H
                this.Intensity = (decimal)StringHelper.BytesToFloat(new byte[] { data[iIndex++], data[iIndex++], data[iIndex++], data[iIndex++] }, true);
                #endregion
                this.Sucessfully = true;
                return;
            }
            else if (data.Length == 5 && data[1] == 0x83)
            {
                //此时设备反馈出错
                //此时返回错误消息
                this.ErrMsg = $"读取降雨明细记录时，设备反馈错误，代码[{data[2]}]";
                this.Sucessfully = false;
                return;
            }
            else
            {
                this.ErrMsg = $"读取降雨明细记录时，返回的数据不是预期的[{StringHelper.ByteToHexStr(data)}]";
                this.Sucessfully = false;
                return;
            }
        }
        #region 属性
        /// <summary>
        /// 无效数据，该字段是针对寄存器内都为0的情况，true为无效，false为有效数据
        /// </summary>
        [Description("是否无效数据")]
        public bool InvalidData { get; set; } = false;
        /// <summary>
        /// 降雨起始时间，精确到年.月.日.时.分
        /// </summary>
        [Description("降雨开始时间")]
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 降雨结束时间，精确到年.月.日.时.分
        /// </summary>
        [Description("降雨结束时间")]
        public DateTime EndTime { get; set; }
        /// <summary>
        /// 降雨量
        /// </summary>
        [Description("降雨量[单位]")]
        public decimal Rainfall { get; set; }
        /// <summary>
        /// 降雨强度
        /// </summary>
        [Description("降雨强度[单位]")]
        public decimal Intensity { get; set; }
        #endregion
    }
    #endregion
    #region 索引日期内查询的干沉降数据
    /// <summary>
    /// 索引日期内查询的干沉降数据
    /// </summary>
    public class SSDryDepositionDataDto:SSBaseDto
    {
        public SSDryDepositionDataDto()
        {

        }
        public SSDryDepositionDataDto(byte[] data, HDSSCmdCode cmd, ushort iMacNo)
        {
            this.MacNo = iMacNo;
            this.Cmd = cmd;
            /********
             * 干沉降明细地址为
             * 通信协议上是：002CH--0031H，总共6个地址，所以返回的字节数
             * 应该是5+2*6=17个；
             * 返回的第3个字节应该是12，
             * *********************************/
            if(data.Length==17 && 0x03==data[1] && 12 == data[2])
            {
                #region 正确解析数据
                /***************
                 * 起始时间格式，数据存储在3个寄存器地址中002CH、002DH、002EH中
                 * 002CH：D8-D15:年 D0-D7：月，第一个字节存储的是年，第二个字节存储的是月
                 * 002DH：D8-D15:日 D0-D7：时，第一个字节存储的是日，第二个字节存储的是时
                 * 002EH：D8-D15:分 D0-D7:无，第一个字节存储的是分，第二个字没有用
                 * 结束时间格式，数据存储在3个寄存器地址中002CH、002DH、002EH中
                 * 002FH：D8-D15:年 D0-D7：月，第一个字节存储的是年，第二个字节存储的是月
                 * 0030H：D8-D15:日 D0-D7：时，第一个字节存储的是日，第二个字节存储的是时
                 * 0031H：D8-D15:分 D0-D7:无，第一个字节存储的是分，第二个字没有用
                 * ******************/
                int iIndex = 3;//从第三位开始读取
                int iY, iM, iD, iHour, iMinute;
                iY = (int)data[iIndex++];//地址002CH中第1个字节为年份
                iM = (int)data[iIndex++];//地址002CH中第2个字节为月份
                iD = (int)data[iIndex++];//地址002DH中第1个字节为日
                if (0 == iY && 0 == iM)
                {
                    //此时年和月为0的话，可以人为是不正确的数据，设备向寄存器中写入了0；
                    this.InvalidData = true;
                    this.ErrMsg = "";
                    return;
                }
                iHour = (int)data[iIndex++];//地址002DH中第2个字节为小时
                iMinute= (int)data[iIndex++];//地址002EH中第1个字节为分钟
                iIndex++;//地址002EH中第2个字节没用
                //赋值起始时间
                this.StartTime = new DateTime(iY + 2000, iM, iD, iHour, iMinute, 0);
                //开始读取结束时间
                iY = (int)data[iIndex++];//地址002FH中第1个字节为年份
                iM = (int)data[iIndex++];//地址002FH中第2个字节为月份
                iD = (int) data[iIndex++];//地址0030H中第1个字节为日
                iHour = (int) data[iIndex++];//地址0030H中第2个字节为小时
                iMinute = (int)data[iIndex++];//地址0031H中第1个字节为分钟
                //赋值起始时间
                this.EndTime = new DateTime(iY+2000, iM, iD, iHour, iMinute, 0);
                #endregion
                this.Sucessfully = true;
            }
            else if (data.Length == 5 && data[1] == 0x83)
            {
                //此时设备反馈出错
                //此时返回错误消息
                this.ErrMsg = $"读取干沉降明细记录时，设备反馈错误，代码[{data[2]}]";
                this.Sucessfully = false;
            }
            else
            {
                this.ErrMsg = $"读取干沉降明细记录时，返回的数据不是预期的[{StringHelper.ByteToHexStr(data)}]";
                this.Sucessfully = false;
            }
        }
        #region 属性

        /// <summary>
        /// 无效数据，该字段是针对寄存器内都为0的情况，true为无效，false为有效数据
        /// </summary>
        [Description("是否无效数据")]
        public bool InvalidData { get; set; } = false;
        /// <summary>
        /// 干沉降起始日期，精确到年.月.日.时.分
        /// </summary>
        [Description("干沉降起始日期")]
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 干沉降结束日期，精确到年.月.日.时.分
        /// </summary>
        [Description("干沉降结束日期")]
        public DateTime EndTime { get; set; }
        #endregion
    }
    #endregion
    #region 故障索引日期设置反馈结果
    public class SSAlarmSearchDateSetDto : SSBaseDto
    {
        public SSAlarmSearchDateSetDto()
        {

        }
        public SSAlarmSearchDateSetDto(byte[] data, HDSSCmdCode cmd, ushort iMacNo)
        {
            this.MacNo = iMacNo;
            this.Cmd = cmd;
            if (data.Length == 8)
            {
                if (data[1] != 0x10)
                {
                    this.ErrMsg = $"设置故障索引时间时反馈的功能码[{data[1]}]不是预期的16";
                    this.Sucessfully = false;
                    return;
                }
                else if (data[2] != 0x00 && data[3] != 0x62 && data[4] != 0x00 && data[5] != 0x02)
                {
                    //此时判断读取反馈的寄存地址是否是我们写入的地址
                    this.ErrMsg = $"设置故障索引时间时反馈的设置地址不是预期的{StringHelper.ByteToHexStr(data)}";
                    this.Sucessfully = false;
                    return;
                }
                else
                {
                    //此时设置正确
                    this.Sucessfully = true;
                }
            }
            else if (data.Length == 5 && data[1] == 0x90)
            {
                //此时设备反馈出错
                //此时返回错误消息
                this.ErrMsg = $"写入故障索引日期时，设备反馈错误，代码[{data[2]}]";
                this.Sucessfully = false;
            }
            else
            {
                this.ErrMsg = $"写入故障索引日期时，返回的数据不是预期的[{StringHelper.ByteToHexStr(data)}]";
                this.Sucessfully = false;
            }
        }
    }
    #endregion
    #region 故障查询结果数
    public class SSAlarmDataDto: SSBaseDto
    {
        public SSAlarmDataDto()
        {

        }
        public SSAlarmDataDto(byte[] data, HDSSCmdCode cmd, ushort iMacNo)
        {
            this.MacNo = iMacNo;
            this.Cmd = cmd;
            /*************
             * 故障地址为0028H、0029H、002AH、002BH
             * 故障时间说明，数据存储在3个寄存器地址中0028H、0029H、002AH中
                 * 0028H：D8-D15:年 D0-D7：月，第一个字节存储的是年，第二个字节存储的是月
                 * 0029H：D8-D15:日 D0-D7：时，第一个字节存储的是日，第二个字节存储的是时
                 * 002AH：D8-D15:分 D0-D7:无，第一个字节存储的是分，第二个字没有用
               查询正确的话返回字节数为5+2*4=13个字节
             * ********************/
            if (data.Length == 13 && 0x03 == data[1] && 8 == data[2])
            {
                //0x03为功能吗，16（十进制的）表示返回的字节数，因为我们请求4个地址的数据
                #region 正确解析数据
                int iIndex = 3;//从第三位开始读取
                int iY, iM, iD, iHour, iMinute;
                iY = (int)data[iIndex++];//地址0028H中第1个字节为年份
                iM = (int)data[iIndex++];//地址0028H中第2个字节为月份
                iD = (int) data[iIndex++];//地址0029H中第1个字节为日
                iHour = (int) data[iIndex++];//地址0029H中第2个字节为小时
                iMinute = (int)data[iIndex++];//地址002AH中第1个字节为分钟
                iIndex++;//地址002AH中第2个字节无内容，直接跳过
                         //赋值故障时间
                if(0==iY || 0==iM || 0==iD)
                {
                    //经过实际设备测试，无效数据时返回的数据iy会有值；返回01030801100060000000024413，只要查询时搜索的时间不存在报警记录就会这样返回；
                    //此时年\月为0的话，可以人为是不正确的数据，设备向寄存器中写入了0；
                    this.InvalidData = true;
                    this.ErrMsg = "";
                    return;
                }
                this.Time = new DateTime(iY+2000, iM, iD, iHour, iMinute, 0);
                //读取故障类型值
                this.Value = StringHelper.BytesToUShort(new byte[] { data[iIndex++], data[iIndex++] }, 0);
                switch (this.Value)
                {
                    case 1:
                        this.Name = "开盖超限";
                        break;
                    case 2:
                        this.Name = "关盖超限";
                        break;
                    case 3:
                        this.Name = "机箱温控故障";
                        break;
                    case 4:
                        this.Name = "采样桶温控故障";
                        break;
                    case 5:
                        this.Name = "备用";
                        break;
                    case 6:
                        this.Name = "感雨器温控故障";
                        break;
                    case 7:
                        this.Name = "干沉降桶液位满故障";
                        break;
                    case 8:
                        this.Name = "干沉降桶液位低故障";
                        break;
                    case 9:
                        this.Name = "集雨器水满故障";
                        break;
                    case 10:
                        this.Name = "环境温度故障";
                        break;
                    default:
                        this.Name = "未知故障";
                        break;
                }
                #endregion
                this.Sucessfully = true;
                return;
            }
            else if (data.Length == 5 && data[1] == 0x83)
            {
                //此时设备反馈出错
                //此时返回错误消息
                this.ErrMsg = $"读取故障记录时，设备反馈错误，代码[{data[2]}]";
                this.Sucessfully = false;
                return;
            }
            else
            {
                this.ErrMsg = $"读取故障记录时，返回的数据不是预期的[{StringHelper.ByteToHexStr(data)}]";
                this.Sucessfully = false;
                return;
            }

        }
        #region 属性
        /// <summary>
        /// 无效数据，该字段是针对寄存器内都为0的情况，true为无效，false为有效数据
        /// </summary>
        [Description("是否无效数据")]
        public bool InvalidData { get; set; } = false;
        /// <summary>
        /// 故障时间，精确到年.月.日.时.分
        /// </summary>
        [Description("故障时间")]
        public DateTime Time { get; set; }
        /// <summary>
        /// 故障代码
        /// </summary>
        [Description("故障代码")]
        public ushort Value { get; set; }
        /// <summary>
        /// 故障名称
        /// </summary>
        [Description("故障名称")]
        public string Name { get; set; }
        #endregion
    }
    #endregion
    #region 设置降雨明细条号索引的反馈结果
    public class SSRainDetailStartIndexSetDto : SSBaseDto
    {
        public SSRainDetailStartIndexSetDto()
        {

        }
        public SSRainDetailStartIndexSetDto(byte[] data, HDSSCmdCode cmd, ushort iMacNo)
        {
            this.MacNo = iMacNo;
            this.Cmd = cmd;
            if (data.Length == 8)
            {
                if (data[1] != 0x10)
                {
                    this.ErrMsg = $"设置降雨明细条号索引时反馈的功能码[{data[1]}]不是预期的16";
                    this.Sucessfully = false;
                    return;
                }
                else if (data[2] != 0x00 && data[3] != 0x67 && data[4] != 0x00 && data[5] != 0x01)
                {
                    //此时判断读取反馈的寄存地址是否是我们写入的地址
                    this.ErrMsg = $"设置降雨明细条号索引时反馈的设置地址不是预期的{StringHelper.ByteToHexStr(data)}";
                    this.Sucessfully = false;
                    return;
                }
                else
                {
                    //此时设置正确
                    this.Sucessfully = true;
                }
            }
            else if (data.Length == 5 && data[1] == 0x90)
            {
                //此时设备反馈出错
                //此时返回错误消息
                this.ErrMsg = $"写入降雨明细条号索引时，设备反馈错误，代码[{data[2]}]";
                this.Sucessfully = false;
            }
            else
            {
                this.ErrMsg = $"写入降雨明细条号索引时，返回的数据不是预期的[{StringHelper.ByteToHexStr(data)}]";
                this.Sucessfully = false;
            }
        }
    }
    #endregion
    #region 设置干沉降明细条号索引的反馈结果
    public class SSDryDepDetailStartIndexSetDto : SSBaseDto
    {
        public SSDryDepDetailStartIndexSetDto()
        {

        }
        public SSDryDepDetailStartIndexSetDto(byte[] data, HDSSCmdCode cmd, ushort iMacNo)
        {
            this.MacNo = iMacNo;
            this.Cmd = cmd;
            if (data.Length == 8)
            {
                if (data[1] != 0x10)
                {
                    this.ErrMsg = $"设置干沉降明细条号索引时反馈的功能码[{data[1]}]不是预期的16";
                    this.Sucessfully = false;
                    return;
                }
                else if (data[2] != 0x00 && data[3] != 0x68 && data[4] != 0x00 && data[5] != 0x01)
                {
                    //此时判断读取反馈的寄存地址是否是我们写入的地址
                    this.ErrMsg = $"设置干沉降明细条号索引时反馈的设置地址不是预期的{StringHelper.ByteToHexStr(data)}";
                    this.Sucessfully = false;
                    return;
                }
                else
                {
                    //此时设置正确
                    this.Sucessfully = true;
                }
            }
            else if (data.Length == 5 && data[1] == 0x90)
            {
                //此时设备反馈出错
                //此时返回错误消息
                this.ErrMsg = $"写入干沉降明细条号索引时，设备反馈错误，代码[{data[2]}]";
                this.Sucessfully = false;
            }
            else
            {
                this.ErrMsg = $"写入降雨明细条号索引时，返回的数据不是预期的[{StringHelper.ByteToHexStr(data)}]";
                this.Sucessfully = false;
            }
        }
    }
    #endregion
    #region 自定义地址查询
    public class SSCustomAdrDto : SSBaseDto
    {
        public SSCustomAdrDto()
        {

        }
        public SSCustomAdrDto(byte[] data, HDSSCmdCode cmd, ushort iMacNo)
        {
            /***********
                 * 解析数据，返回的数据长度为5+2*26（请求26个寄存器地址）=57
                 * 返回的存储寄存器值的数据长度=2*26=52个字节，所以返回的第三个字节值为52
                 * ******************/
            this.MacNo = iMacNo;
            this.Cmd = cmd;
            this.AllFromMac = StringHelper.ByteToHexStr(data);
            byte[] bs = new byte[data.Length - 5];
            Array.Copy(data, 3, bs, 0, bs.Length);
            this.Value = StringHelper.ByteToHexStr(bs);
        }
        #region 属性
        /// <summary>
        /// 干沉降总条数
        /// </summary>
        [Description("起始地址")]
        public ushort StartAdr { get; set; }
        /// <summary>
        /// 当前仪器状态，0：空闲，1：采样中
        /// </summary>
        [Description("地址数量[]")]
        public ushort AdtCnt { get; set; }
        /// <summary>
        /// 设备实时时钟
        /// </summary>
        [Description("所有结果值")]
        public string AllFromMac { get; set; }
        /// <summary>
        /// 有效数据
        /// </summary>
        [Description("有效数据")]
        public string Value { get; set; }

        #endregion

    
    }
    #endregion

    #region 文件查询反馈
    public class SSFileInfoDto : SSBaseDto
    {
        public SSFileInfoDto()
        {
            this.Title = "文件信息";
        }
        public SSFileInfoDto(byte[] receiveData, HDSSCmdCode cmd)
        {
            this.Cmd = cmd;
            this.Title = "文件信息";
            if (receiveData.Length == 9)
            {
                if (receiveData[6] == 0xFF)
                {
                    //此时不存在
                    this.FileExist = false;
                    #region 清空原先数据
                    this.FileNo = 0;
                    this.BatchNumber = string.Empty;
                    this.StartTime = string.Empty;
                    this.EndTime = string.Empty;
                    this.InstantaneousFlow = 0M;
                    this.StandardVolume = 0M;
                    this.WorkingVolume = 0M;
                    this.Temperature = 0M;
                    this.Humidity = 0M;
                    this.Atmosphere = 0M;
                    #endregion
                }
            }
            else if (receiveData.Length == 55)
            {
                #region 开始赋值
                this.FileExist = true;
                this.FileNo = Aming.Core.StringHelper.BitToInt(new byte[] { receiveData[7], receiveData[6], 0x00, 0x00 });//文件编号
                this.BatchNumber = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}{13}{14}{15}{16}{17}{18}"
                    , (char)receiveData[8], (char)receiveData[9], (char)receiveData[10], (char)receiveData[11], (char)receiveData[12]
                    , (char)receiveData[13], (char)receiveData[14], (char)receiveData[15], (char)receiveData[16], (char)receiveData[17]
                    , (char)receiveData[18], (char)receiveData[19], (char)receiveData[20], (char)receiveData[21], (char)receiveData[22]
                    , (char)receiveData[23], (char)receiveData[24], (char)receiveData[25], (char)receiveData[26]);//滤膜ID号
                                                                                                                  //开始时间，时间用标准时间格式输出：2013-01-01 09:23
                this.StartTime = string.Format("20{0}-{1}-{2} {3}:{4}", Convert.ToString(receiveData[27]).PadLeft(2, '0'), Convert.ToString(receiveData[28]).PadLeft(2, '0'), Convert.ToString(receiveData[29]).PadLeft(2, '0'), Convert.ToString(receiveData[30]).PadLeft(2, '0'), Convert.ToString(receiveData[31]).PadLeft(2, '0'));
                this.EndTime = string.Format("20{0}-{1}-{2} {3}:{4}", Convert.ToString(receiveData[32]).PadLeft(2, '0'), Convert.ToString(receiveData[33]).PadLeft(2, '0'), Convert.ToString(receiveData[34]).PadLeft(2, '0'), Convert.ToString(receiveData[35]).PadLeft(2, '0'), Convert.ToString(receiveData[36]).PadLeft(2, '0'));
                this.InstantaneousFlow = Aming.Core.StringHelper.BytesToShort(receiveData, 37) / 10M;//流量：第37,38位
                this.StandardVolume = Aming.Core.StringHelper.BitToInt(new byte[] { receiveData[42], receiveData[41], receiveData[40], receiveData[39] }) / 10M;//标况和工况比较特殊占4个字节
                this.WorkingVolume = Aming.Core.StringHelper.BitToInt(new byte[] { receiveData[46], receiveData[45], receiveData[44], receiveData[43] }) / 10M;//工况
                this.Temperature = (Aming.Core.StringHelper.BytesToShort(receiveData, 47) - 200) / 10M;//环境温度
                this.Humidity = Aming.Core.StringHelper.BytesToShort(receiveData, 49) / 10M;//环境湿度
                this.Atmosphere = Aming.Core.StringHelper.BytesToShort(receiveData, 51) / 10M;//大气压
                #endregion
            }

        }
        #region 属性
        /// <summary>
        /// 文件是否存在
        /// </summary>
        [Description("文件是否存在")]
        public bool FileExist { get; set; }
        /// <summary>
        ///文件号
        /// <summary>
        [Description("文件号")]
        public int FileNo { get; set; }
        /// <summary>
        ///滤膜ID号
        /// <summary>
        [Description("滤膜ID号")]
        public string BatchNumber { get; set; }
        /// <summary>
        ///开始时间
        /// <summary>
        [Description("开始时间")]
        public string StartTime { get; set; }
        /// <summary>
        ///结束时间
        /// <summary>
        [Description("结束时间")]
        public string EndTime { get; set; }
        /// <summary>
        ///瞬时流量
        /// <summary>
        [Description("瞬时流量[L/min]")]
        public decimal InstantaneousFlow { get; set; }
        /// <summary>
        ///标况体积
        /// <summary>
        [Description("标况体积[m³]")]
        public decimal StandardVolume { get; set; }
        /// <summary>
        ///工况体积
        /// <summary>
        [Description("工况体积[m³]")]
        public decimal WorkingVolume { get; set; }
        /// <summary>
        ///环境温度
        /// <summary>
        [Description("环境温度[℃]")]
        public decimal Temperature { get; set; }
        /// <summary>
        ///环境湿度
        /// <summary>
        [Description("环境湿度[%]")]
        public decimal Humidity { get; set; }
        /// <summary>
        ///大气压
        /// <summary>
        [Description("大气压[kpa]")]
        public decimal Atmosphere { get; set; }

        #endregion
    }
    #endregion
}
