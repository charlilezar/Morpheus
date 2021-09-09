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
    public class ZJCBaseDto:BaseDto
    {
        [Description("命令值")]
        public HDZJCCmdCode Cmd { get; set; }
    }
    #region 实时数据
    public class ZJCRealDataDto:ZJCBaseDto
    {
        public ZJCRealDataDto()
        {

        }
        public ZJCRealDataDto(byte[] data, HDZJCCmdCode cmd, ushort iMacNo)
        {
            /***********
                 * 解析数据，返回的数据长度为5+2*26（请求26个寄存器地址）=57
                 * 返回的存储寄存器值的数据长度=2*26=52个字节，所以返回的第三个字节值为52
                 * ******************/
            this.MacNo = iMacNo;
            this.Cmd = cmd;
            if (data.Length == 57 && 0x03==data[1] && 52==data[2])
            {
                #region 正确数据解析
                int iIndex = 3;
                this.DryDepositionCount = StringHelper.BytesToUShort(new byte[] { data[iIndex++], data[iIndex++] }, 0);//第3，4字节为干沉降总条数
                this.Status = StringHelper.BytesToUShort(new byte[] { data[iIndex++], data[iIndex++] }, 0);
                //第4，5字节为设别状态
                 //赋值当前设备实时时间，有3个寄存器地址存储，总共6个字节，从第6个开始到11个
                //规则：第一个寄存器存储：D8-D15:年 D0-D7：月；第二个寄存器存储：D8-D15:日 D0-D7：时；第三个寄存器存储：D8-D15:分 D0-D7: 秒
                ushort iY, iM, iD, iHour, iMinute, iSecond;
                iY = (ushort)data[iIndex++];//寄存器1高位是年份
                iM = (ushort)data[iIndex++];//寄存器1低位是月份
                iD= (ushort)data[iIndex++];//寄存器2高位是日
                iHour = (ushort)data[iIndex++];//寄存器2低位是小时
                iMinute = (ushort)data[iIndex++];//寄存器3高位是分钟
                iSecond = (ushort)data[iIndex++];//寄存器3低位是秒
                //iM = (ushort)data[iIndex++];//寄存器1低位是月份
                //iM = (ushort)data[iIndex++];//寄存器1低位是月份
                //iY = StringHelper.BytesToUShort(new byte[] { data[iIndex++], data[iIndex++] }, 0);//寄存器1高位是年份
                //iHour = StringHelper.BytesToUShort(new byte[] { data[iIndex++], data[iIndex++] }, 0);//寄存器2低位是小时
                //iD = StringHelper.BytesToUShort(new byte[] { data[iIndex++], data[iIndex++] }, 0);//寄存器2高位是日
                //iSecond = StringHelper.BytesToUShort(new byte[] { data[iIndex++], data[iIndex++] }, 0);//寄存器3低位是秒
                //iMinute = StringHelper.BytesToUShort(new byte[] { data[iIndex++], data[iIndex++] }, 0);//寄存器3高位是分钟
                iY += 2000;//寄存器只用1个字节存储年份，所以只能是存储2000后面的累加年份
                this.InstrumentTime = new DateTime(iY, iM, iD, iHour, iMinute, iSecond);//时间赋值
                this.RainfallYesterday = (decimal)StringHelper.BytesToFloat(new byte[] { data[iIndex++], data[iIndex++], data[iIndex++], data[iIndex++] }, true);//0007H、0008H为前一天降雨量
                this.RainfallCurrent = (decimal)StringHelper.BytesToFloat(new byte[] { data[iIndex++], data[iIndex++], data[iIndex++], data[iIndex++] }, true);//0009H、000AH为当前降雨量
                this.Temperature = (decimal)StringHelper.BytesToFloat(new byte[] { data[iIndex++], data[iIndex++], data[iIndex++], data[iIndex++] }, true);//000BH、000CH为当前环境温度
                this.TempOfBox = (decimal)StringHelper.BytesToFloat(new byte[] { data[iIndex++], data[iIndex++], data[iIndex++], data[iIndex++] }, true);//000DH、000EH为当前机箱温度
                this.TempOfBucket = (decimal)StringHelper.BytesToFloat(new byte[] { data[iIndex++], data[iIndex++], data[iIndex++], data[iIndex++] }, true);//000FH、0010H为当前采样桶
                iIndex += 4;//0011H、0012H为备用地址，目前没用
                this.TempOfRainSensor = (decimal)StringHelper.BytesToFloat(new byte[] { data[iIndex++], data[iIndex++], data[iIndex++], data[iIndex++] }, true);//0013H、0014H为当前感雨器温度
                iIndex += 4;//0015H、0016H为备用地址，目前没用
                this.Raintimes = StringHelper.BytesToUShort(new byte[] { data[iIndex++], data[iIndex++] }, 0);//00017H查询日期内降雨总时间
                this.FilledWater = StringHelper.BytesToUShort(new byte[] { data[iIndex++], data[iIndex++] }, 0) == 1;//00018H集雨桶水满信号；0：没满 1：水满
                this.Humidity = StringHelper.BytesToUShort(new byte[] { data[iIndex++], data[iIndex++] }, 0);//00019H湿度
                iIndex += 2;
                //0001AH采样模式，目前就一种降水逢雨采样；
                //0001BH报警信息，存储在位地址中，总共1个地址2个字节

                /*****************
                 第一个字节的8个位地址存储值情况
                    00000001 集雨器水满故障
                   00000010 环境温度故障
                 第二个字节的8个位地址存储值情况
                   00000001 开盖超限
                   00000010 关盖超限
                   00000100 机箱温控故障
                   00001000 采样桶温控故障
                   00010000 备用
                   00100000 感雨器温控故障
                   01000000 干沉降桶液位满故障
                   10000000 干沉降桶液位低故障
                TODO:目前还无法知道究竟顺序是怎么样的，是否是按照返回数据的字节顺序来的？目前假设是按照顺序来的
                 * *******************/
                //第一个字节
                string strAlarm = StringHelper.ByteToBinString8(data[iIndex++]); //该函数返回的0x01值为00000001，也就是最低的位地址在字符串的末尾；
                this.AlarmJyqFilled = strAlarm.Substring(7, 1) == "1";
                this.AlarmTemperature = strAlarm.Substring(6, 1) == "1";
                //第2个字节
                strAlarm = StringHelper.ByteToBinString8(data[iIndex++]); //该函数返回的0x01值为00000001，也就是最低的位地址在字符串的末尾；
                this.AlarmLidOpenedOver = strAlarm.Substring(7, 1) == "1";
                this.AlarmLidClosedOver = strAlarm.Substring(6, 1) == "1";
                this.AlarmBoxTemp = strAlarm.Substring(5, 1) == "1";
                this.AlarmBucketTemp = strAlarm.Substring(4, 1) == "1";
                //第5位备用地址
                this.AlarmRainSensor = strAlarm.Substring(2, 1) == "1";
                this.AlarmDryBucketFilled = strAlarm.Substring(1, 1) == "1";
                this.AlarmDryBucketWaterLess = strAlarm.Substring(0, 1) == "1";
                #endregion
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
        /// <summary>
        /// 干沉降总条数
        /// </summary>
        [Description("干沉降总条数")]
        public ushort DryDepositionCount { get; set; }
        /// <summary>
        /// 当前仪器状态，0：空闲，1：采样中
        /// </summary>
        [Description("当前仪器状态[0空闲，1采样中]")]
        public ushort Status { get; set; }
        /// <summary>
        /// 设备实时时钟
        /// </summary>
        [Description("设备实时时钟")]
        public DateTime InstrumentTime { get; set; }
        /// <summary>
        /// 前一天降雨总量
        /// </summary>
        [Description("前一天降雨总量[单位]")]
        public decimal RainfallYesterday { get; set; }
        /// <summary>
        /// 当前逢雨雨量
        /// </summary>
        [Description("当前逢雨雨量[单位]")]
        public decimal RainfallCurrent { get; set; }
        /// <summary>
        /// 环境温度
        /// </summary>
        [Description("环境温度[℃]")]
        public decimal Temperature { get; set; }
        /// <summary>
        /// 机箱温度
        /// </summary>
        [Description("机箱温度[℃]")]
        public decimal TempOfBox { get; set; }
        /// <summary>
        /// 采样桶温度
        /// </summary>
        [Description("采样桶温度[℃]")]
        public decimal TempOfBucket { get; set; }
        /// <summary>
        /// 雨感器温度
        /// </summary>
        [Description("雨感器温度[℃]")]
        public decimal TempOfRainSensor { get; set; }
        /// <summary>
        /// 查询日期内降雨总时间
        /// </summary>
        [Description("查询日期内降雨总时间[分钟]")]
        public ushort Raintimes { get; set; }
        /// <summary>
        /// 集雨桶水满信号
        /// </summary>
        [Description("集雨桶水满[true水满了，false未满]")]
        public bool FilledWater { get; set; }
        /// <summary>
        /// 湿度
        /// </summary>
        [Description("湿度[单位]")]
        public ushort Humidity { get; set; }
        /// <summary>
        /// 开盖超限
        /// </summary>
        [Description("开盖超限[true超限，false正常]")]
        public bool AlarmLidOpenedOver { get; set; }
        /// <summary>
        /// 关盖超限
        /// </summary>
        [Description("关盖超限[true超限，false正常]")]
        public bool AlarmLidClosedOver { get; set; }
        /// <summary>
        /// 机箱温控故障
        /// </summary>
        [Description("机箱温控故障[true有故障，false正常]")]
        public bool AlarmBoxTemp { get; set; }
        /// <summary>
        /// 采样桶温控故障
        /// </summary>
        [Description("采样桶温控故障[true有故障，false正常]")]
        public bool AlarmBucketTemp { get; set; }
        /// <summary>
        /// 感雨器温控故障
        /// </summary>
        [Description("感雨器温控故障[true有故障，false正常]")]
        public bool AlarmRainSensor { get; set; }
        /// <summary>
        /// 干沉降桶液位满故障
        /// </summary>
        [Description("干沉降桶液位满故障[true有故障，false正常]")]
        public bool AlarmDryBucketFilled { get; set; }
        /// <summary>
        /// 干沉降桶液位低故障
        /// </summary>
        [Description("干沉降桶液位低故障[true有故障，false正常]")]
        public bool AlarmDryBucketWaterLess { get; set; }
        /// <summary>
        /// 集雨器水满故障
        /// </summary>
        [Description("集雨器水满故障[true有故障，false正常]")]
        public bool AlarmJyqFilled { get; set; }
        /// <summary>
        /// 开盖超限
        /// </summary>
        [Description("环境温度故障[true有故障，false正常]")]
        public bool AlarmTemperature { get; set; }
        #endregion
    }
    #endregion
    #region 索引日期设置反馈结果
    public class ZJCRainDetailSearchDateSetDto: ZJCBaseDto
    {
        public ZJCRainDetailSearchDateSetDto()
        {

        }
        public ZJCRainDetailSearchDateSetDto(byte[] data, HDZJCCmdCode cmd, ushort iMacNo)
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
    public class ZJCRainDataDto : ZJCBaseDto
    {
        public ZJCRainDataDto()
        {

        }
        public ZJCRainDataDto(byte[] data, HDZJCCmdCode cmd, ushort iMacNo)
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
                //解析起始时间，因为发现干湿设备如果索引时间内没有记录，返回的不是０，只是不是我们预期的日期格式
                DateTime detTime;
                if(!DateTime.TryParse(string.Format("{0}-{1}-{2} {3}:{4}", iY + 2000, iM, iD, iHour, iMinute),out detTime))
                {
                    this.InvalidData = true;
                    this.ErrMsg = string.Format("起始时间{0}-{1}-{2} {3}:{4}", iY + 2000, iM, iD, iHour, iMinute);
                    return;
                }
                this.StartTime = detTime;
                //开始读取结束时间
                iHour = (int)data[iIndex++];//地址0022H中第1个字节为小时
                iMinute = (int)data[iIndex++];//地址0022H中第2个字节为分钟
                //定义结束时间，注意传入的年份要加上2000
                if (!DateTime.TryParse(string.Format("{0}-{1}-{2} {3}:{4}", iY + 2000, iM, iD, iHour, iMinute), out detTime))
                {
                    this.InvalidData = true;
                    this.ErrMsg = string.Format("结束时间{0}-{1}-{2} {3}:{4}", iY + 2000, iM, iD, iHour, iMinute);
                    return;
                }
                this.EndTime = detTime;
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
    public class ZJCDryDepositionDataDto:ZJCBaseDto
    {
        public ZJCDryDepositionDataDto()
        {

        }
        public ZJCDryDepositionDataDto(byte[] data, HDZJCCmdCode cmd, ushort iMacNo)
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
    public class ZJCAlarmSearchDateSetDto : ZJCBaseDto
    {
        public ZJCAlarmSearchDateSetDto()
        {

        }
        public ZJCAlarmSearchDateSetDto(byte[] data, HDZJCCmdCode cmd, ushort iMacNo)
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
    public class ZJCAlarmDataDto: ZJCBaseDto
    {
        public ZJCAlarmDataDto()
        {

        }
        public ZJCAlarmDataDto(byte[] data, HDZJCCmdCode cmd, ushort iMacNo)
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
    public class ZJCRainDetailStartIndexSetDto : ZJCBaseDto
    {
        public ZJCRainDetailStartIndexSetDto()
        {

        }
        public ZJCRainDetailStartIndexSetDto(byte[] data, HDZJCCmdCode cmd, ushort iMacNo)
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
    public class ZJCDryDepDetailStartIndexSetDto : ZJCBaseDto
    {
        public ZJCDryDepDetailStartIndexSetDto()
        {

        }
        public ZJCDryDepDetailStartIndexSetDto(byte[] data, HDZJCCmdCode cmd, ushort iMacNo)
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
    public class ZJCCustomAdrDto : ZJCBaseDto
    {
        public ZJCCustomAdrDto()
        {

        }
        public ZJCCustomAdrDto(byte[] data, HDZJCCmdCode cmd, ushort iMacNo)
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
}
