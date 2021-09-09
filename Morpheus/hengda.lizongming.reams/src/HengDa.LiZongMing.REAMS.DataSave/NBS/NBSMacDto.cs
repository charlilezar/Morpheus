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
    public class NBSBaseDto:BaseDto
    {
        /// <summary>
        /// 命令值
        /// </summary>
        [Description("命令值")]
        public HDNBSCmdCode Cmd { get; set; }
    }
    #region 采样启停控制反馈1
    /// <summary>
    /// 开始采样
    /// </summary>
    public class NBSStartControllerDto : NBSBaseDto
    {
        public NBSStartControllerDto()
        {
            this.Title = "开始采样";
        }
        public NBSStartControllerDto(byte[] receiveData, HDNBSCmdCode cmd)
        {
            this.Cmd = cmd;
            this.Title = "开始采样";
            byte b = receiveData[6];
            if (receiveData.Length == 9 && receiveData[6] == 0xFF)
            {
                this.Sucessfully = false;
                this.ErrMsg = "设备反馈0xFF！";
            }
            else if (receiveData.Length == 9)
            {
                this.ControllerType = receiveData[6];//控制结果
            }
            else
            {
                this.Sucessfully = false;
                this.ErrMsg = $"采样启停控制反馈数据[{StringHelper.ByteToHexStr(receiveData)}]不是预期的。";
            }
        }
        #region 属性
        /// <summary>
        /// 控制类型，0x01,0x02,0x03，分别表示启动，停止，暂停
        /// </summary>
        [Description("控制类型[0x01启动,0x02停止,0x03暂停]")]
        public byte ControllerType { get; set; }
        #endregion
    }
    #endregion
    #region 时间设置反馈2
    public class NBSTimeSetDto : NBSBaseDto
    {
        public NBSTimeSetDto()
        {

        }
        public NBSTimeSetDto(byte[] receiveData, HDNBSCmdCode cmd)
        {
            /*****************
             *发送68 08 08 68 60 05 XX XX XX XX XX XX CH 0D
              成功返回68 08 08 68 61 05 XX XX XX XX XX XX CH 0D 
              失败返回：68 03 03 68 61 01 FF CH 0D
              说明：格式为十六进制的年月日时分秒。
            例：0D 05 09 10 2D 1E代表2013年05月09日16点45分30秒。
                * ***************/
            this.Cmd = cmd;
            this.Title = "设备时间设定反馈";
            short iLen = receiveData[1];
            if (receiveData[1] == 0x03 && receiveData[6] == 0xFF)
            {
                //此时为设备反馈设置失败
                this.Sucessfully = false;
                this.ErrMsg = "设备反馈设定失败";
            }
            else if (receiveData[1] == 0x08)
            {
                //获取小时部分，注意实际值=获取值/10；实际值是保留小数点一位的数值
                string strY = StringHelper.BytesToShort(new byte[] { 0x00, receiveData[6] }, 0).ToString().PadLeft(2, '0');
                string strM = StringHelper.BytesToShort(new byte[] { 0x00, receiveData[7] }, 0).ToString().PadLeft(2, '0');
                string strD = StringHelper.BytesToShort(new byte[] { 0x00, receiveData[8] }, 0).ToString().PadLeft(2, '0');
                string strHour = StringHelper.BytesToShort(new byte[] { 0x00, receiveData[9] }, 0).ToString().PadLeft(2, '0');
                string strMin = StringHelper.BytesToShort(new byte[] { 0x00, receiveData[10] }, 0).ToString().PadLeft(2, '0');
                string strSec = StringHelper.BytesToShort(new byte[] { 0x00, receiveData[11] }, 0).ToString().PadLeft(2, '0');
                this.Time = string.Format("20{0}-{1}-{2} {3}:{4}:{5}", strY, strM, strD, strHour, strMin, strSec);
            }
            else
            {
                this.Sucessfully = false;
                this.ErrMsg = string.Format("数据解析失败:{0}", StringHelper.ByteToHexStr(receiveData));
            }
        }
        #region 属性值
        /// <summary>
        /// 时间设定值
        /// </summary>
        [Description("时间设定值")]
        public string Time { get; set; } = "";
        #endregion
    }
    #endregion
    #region 设备运行模式反馈3
    /// <summary>
    /// 设备运行模式反馈
    /// </summary>
    public class NBSWorkModeSetDto : NBSBaseDto
    {
        public NBSWorkModeSetDto()
        {
            this.Title = "开始采样";
        }
        public NBSWorkModeSetDto(byte[] receiveData, HDNBSCmdCode cmd)
        {
            this.Cmd = cmd;
            this.Title = "运行模式设定反馈";
            byte b = receiveData[6];
            if (receiveData.Length == 9 && receiveData[6] == 0xFF)
            {
                this.Sucessfully = false;
                this.ErrMsg = "设备反馈0xFF！";
            }
            else if (receiveData.Length == 9)
            {
                this.WorkMode = receiveData[6];//控制结果
            }
            else
            {
                this.Sucessfully = false;
                this.ErrMsg = $"运行模式设定反馈数据[{StringHelper.ByteToHexStr(receiveData)}]不是预期的。";
            }
        }
        #region 属性
        /// <summary>
        /// 运行模式设定反馈
        /// </summary>
        [Description("运行模式设定反馈[0x00半自动定量，0x01半自动定时]")]
        public byte WorkMode { get; set; }
        #endregion
    }
    #endregion
    #region 滤膜编号设定反馈4
    /// <summary>
    /// 滤膜编号设定的反馈信息
    /// </summary>
    public class NBSLvMoIDSetDto : NBSBaseDto
    {
        public NBSLvMoIDSetDto()
        {

        }
        public NBSLvMoIDSetDto(byte[] receiveData, HDNBSCmdCode cmd)
        {
            /*****************
            空闲有效；
            PC发送正确膜号：68 15 15 68 64 05 30 31 32 33 34 35 36 37 38 39 30 31 32 33 34 35 36 37 38 CH 0D
            返回：          68 15 15 68 65 05 30 31 32 33 34 35 36 37 38 39 30 31 32 33 34 35 36 37 38 CH 0D 
            PC发送错误膜号：68 15 15 68 64 05 30 31 32 33 34 35 46 37 38 39 30 31 32 33 34 35 36 37 38 CH 0D
            回复：68 03 03 68 65 01 FF CH 0D
            说明：滤膜编号为19位的ASCII码字符串，该字符串只能包含阿拉伯数字0到9 ，且必须是19位，
            若不足19位，则需要前面用‘0’补全。若收到的编号不是所规定的格式，可能导致操作失败。
            （若启动采样前进行了滤膜编号设置，则采样结果以所设置的编号进行记录；若采样开始前未设置滤膜编号，
            则启动采样后的记录中，滤膜编号按上一条记录的滤膜编号加1来作为本条记录的滤膜编号）
             ***************/
            this.Cmd = cmd;
            this.Title = "设置信息查询反馈";
            short iLen = receiveData[1];
            if (receiveData[1] == 0x03 && receiveData[6] == 0xFF)
            {
                //此时为设备反馈设置失败
                this.Sucessfully = false;
                this.ErrMsg = "设备反馈设定失败";
            }
            else if (receiveData[1] == 0x15)
            {
                for (int i = 6; i < 25; i++)
                {
                    this.LvMoID += (char)receiveData[i];
                }
            }
            else
            {
                this.Sucessfully = false;
                this.ErrMsg = string.Format("数据解析失败:{0}", StringHelper.ByteToHexStr(receiveData));
            }
        }
        #region 属性值
        /// <summary>
        /// 设定的滤膜ID值
        /// </summary>
        [Description("设定的滤膜ID")]
        public string LvMoID { get; set; } = string.Empty;
        #endregion
    }
    #endregion
    #region 采样瞬时流量设定反馈5
    /// <summary>
    /// 采样瞬时流量设定反馈
    /// </summary>
    public class NBSSampllingFlowRateDto : NBSBaseDto
    {
        public NBSSampllingFlowRateDto()
        {

        }
        public NBSSampllingFlowRateDto(byte[] receiveData, HDNBSCmdCode cmd)
        {
            this.Cmd = cmd;
            this.Title = "采样瞬时流量设定反馈";
            short iLen = receiveData[1];
            if (receiveData[1] == 0x03 && receiveData[6] == 0xFF)
            {
                this.Sucessfully = false;
                this.ErrMsg = "设备反馈设定失败";
            }
            else if (receiveData[1] == 0x04)
            {
                this.FlowRateSet = StringHelper.BytesToUShort(new byte[] { receiveData[6], receiveData[7] }, 0) / 10M;
            }
            else
            {
                this.Sucessfully = false;
                this.ErrMsg = string.Format("数据解析失败:{0}", StringHelper.ByteToHexStr(receiveData));
            }
        }
        #region 属性值
        /// <summary>
        /// 采样流量设定值
        /// </summary>
        [Description("采样流量设定值[m³/h]")]
        public decimal FlowRateSet { get; set; } = 0M;
        #endregion
    }
    #endregion
    #region 采样时间设定反馈6
    public class NBSSampllingTimeLongSetDto : NBSBaseDto
    {
        public NBSSampllingTimeLongSetDto()
        {

        }
        public NBSSampllingTimeLongSetDto(byte[] receiveData, HDNBSCmdCode cmd)
        {
            /*****************
                * 成功返回68 05 05 68 41 05 XX XX XX CH 0D
                * 采样时间前两字节代表小时（最大999），第三个字节代表分钟（最大59）。
                    例：00 02 1E代表2小时30分钟
                  失败返回：68 03 03 68 41 01 FF CH 0D
                * ***************/
            this.Cmd = cmd;
            this.Title = "采样时间设定反馈";
            short iLen = receiveData[1];
            if (receiveData[1] == 0x03 && receiveData[6] == 0xFF)
            {
                //此时为设备反馈设置失败
                this.Sucessfully = false;
                this.ErrMsg = "设备反馈设定失败";
            }
            else if (receiveData[1] == 0x05)
            {

                //获取小时部分
                this.Hour = StringHelper.BytesToShort(new byte[] { receiveData[6], receiveData[7] }, 0);
                //获取分钟部分
                this.Minute = StringHelper.BytesToShort(new byte[] { 0x00, receiveData[8] }, 0);
            }
            else
            {
                this.Sucessfully = false;
                this.ErrMsg = string.Format("数据解析失败:{0}", StringHelper.ByteToHexStr(receiveData));
            }
        }
        #region 属性值
        /// <summary>
        /// 设定值小时部分值
        /// </summary>
        [Description("设定值小时部分")]
        public short Hour { get; set; } = 0;
        /// <summary>
        /// 设定值分钟部分值
        /// </summary>
        [Description("设定值分钟部分")]
        public short Minute { get; set; } = 0;
        #endregion
    }
    #endregion
    #region NBS运行状态查询反馈8
    /// <summary>
    /// 工作状态查询
    /// </summary>
    public class NBSWorkStatusDto : NBSBaseDto
    {
        public NBSWorkStatusDto()
        {
            this.Title = "工作状态";
        }
        public NBSWorkStatusDto(byte[] receiveData, HDNBSCmdCode cmd)
        {
            this.Cmd = cmd;
            this.Title = "运行状态";
            byte b = receiveData[7];
            if (b != 0x00 && b != 0x01 && b != 0x02 && b != 0x03 && b != 0x04)
            {
                this.Sucessfully = false;
                this.ErrMsg = string.Format("工作状态返回数据的状态值[{0}]不是预期的。", b);
                this.WorkStatus = -1;
            }
            else
            {
                this.Sucessfully = true;
                this.ErrMsg = string.Empty;
                this.WorkStatus = b;
                switch (b)
                {
                    case 0x00:
                        this.WorkStatusName = "空闲";
                        break;
                    case 0x01:
                        this.WorkStatusName = "采样中";
                        break;
                    case 0x02:
                        this.WorkStatusName = "等待";
                        break;
                    case 0x03:
                        this.WorkStatusName = "暂停";
                        break;
                    case 0x04:
                        this.WorkStatusName = "采样完成";
                        break;
                }
            }
        }
        /// <summary>
        /// 设备状态：-1:未定义；0空闲；1定量采样中，2：定时采样中，3：等待中，4：暂停中
        /// </summary>
        [Description("设备状态[0x00空闲，0x01采样中，0x02等待，0x03暂停，0x04完成采样]")]
        public short WorkStatus { get; set; } = -1;
        /// <summary>
        /// 设备状态描述
        /// </summary>
        [Description("设备状态描述")]
        public string WorkStatusName { get; set; }
    }
    #endregion
    #region NBS设置信息查询反馈9
    public class NBSSettedInfotDto : NBSBaseDto
    {
        public NBSSettedInfotDto()
        {

        }
        public NBSSettedInfotDto(byte[] receiveData, HDNBSCmdCode cmd)
        {
            /*****************
             发送 68 03 03 68 80 05 00 CH 0D
            返回 68 0c 0c 68 81 05 00 XX XX YY YY YY ZZ ZZ ZZ ZZ CH 0D
            说明：红色部分XX代表瞬时采样设定的流量，YY代表定时采样设定的时间，ZZ代表定量采样设定的体积。
            值类型说明：
            1、瞬时采样设定的流量，为整型，范围是：20~260，例：00 78表示120ml；
            2、设定时间，2个都为整型：前两字节代表小时（最大999），第三个字节代表分钟（最大59）， 例：00 02 1E代表2小时30分钟，所以个有2个值来对应；
            3、采样设定的体积，四字节，为数值类型：，小数点后一位，例：00 00 07 D0代表200.0立方，最大980.0。也就是说获取值除以10才是实际值；
                * ***************/
            this.Cmd = cmd;
            this.Title = "设定值查询反馈";
            short iLen = receiveData[1];
            if (receiveData[1] == 0x0C)
            {
                //获取小时部分，注意实际值=获取值/10；实际值是保留小数点一位的数值
                this.SettingFlow = StringHelper.BytesToShort(new byte[] { receiveData[7], receiveData[8] }, 0);
                this.SettingHour = StringHelper.BytesToShort(new byte[] { receiveData[9], receiveData[10] }, 0);
                this.SettingMin = (short)receiveData[11];//分钟只有1个字节
                this.SettingTotalFlow = StringHelper.BitToInt(new byte[] { receiveData[15], receiveData[14], receiveData[13], receiveData[12] }) / 10M;
            }
            else
            {
                this.Sucessfully = false;
                this.ErrMsg = string.Format("数据解析失败:{0}", StringHelper.ByteToHexStr(receiveData));
            }
        }
        #region 属性值
        /// <summary>
        /// 瞬时采样流量设定值
        /// </summary>
        [Description("瞬时采样流量设定值[L/Min]")]
        public int SettingFlow { get; set; } = 0;
        /// <summary>
        /// 设定的采样时间的小时部分
        /// </summary>
        [Description("设定的采样时间的小时部分值[小时]")]
        public int SettingHour { get; set; } = 0;
        /// <summary>
        /// 设定的采样时间的分钟部分
        /// </summary>
        [Description("已设定的采样时间的分钟部分值[分钟]")]
        public short SettingMin { get; set; } = 0;
        /// <summary>
        /// 已设定的采样体积值
        /// </summary>
        [Description("已设定的采样体积值[m³]")]
        public decimal SettingTotalFlow { get; set; } = 0;
        #endregion
    }
    #endregion
    #region NBS剩余时间查询反馈10
    public class NBSRemainWorkTimesDto : NBSBaseDto
    {
        public NBSRemainWorkTimesDto()
        {

        }
        public NBSRemainWorkTimesDto(byte[] receiveData, HDNBSCmdCode cmd)
        {
            /*****************
             （3）返回内容说明：68 07 07 68 83 05 AA XX XX YY ZZ CH 0D，
            AA固定是0x00，定义为占位符，XX XX为小时部分，范围是0-999，高字节在前，低字节在后；
            YY是分钟部分，范围是0-59，ZZ是秒部分，范围是0-59；
            设备返回错误信息：68 03 03 68 83 05 FF CH 0D
                * ***************/
            this.Cmd = cmd;
            this.Title = "剩余时间查询";
            short iLen = receiveData[1];
            if (7==receiveData[1])
            {
                //获取小时部分，注意实际值=获取值/10；实际值是保留小数点一位的数值
                this.Hours = StringHelper.BytesToShort(new byte[] { receiveData[7], receiveData[8] }, 0);
                this.Minutes = (short)receiveData[9];
                this.Seconds = (short)receiveData[10];
            }
            else if (9==receiveData.Length && 0xFF == receiveData[6])
            {
                this.Sucessfully = false;
                this.ErrMsg = string.Format("设备返回0xFF");
            }
            else
            {
                this.Sucessfully = false;
                this.ErrMsg = string.Format("数据解析失败:{0}", StringHelper.ByteToHexStr(receiveData));
            }
        }
        #region 属性值
        /// <summary>
        /// 剩余小时部分
        /// </summary>
        [Description("剩余小时部分[小时]")]
        public short Hours { get; set; } = 0;
        /// <summary>
        /// 剩余分钟部分
        /// </summary>
        [Description("剩余分钟部分[分钟]")]
        public short Minutes { get; set; } = 0;
        /// <summary>
        /// 剩余时间秒部分
        /// </summary>
        [Description("剩余时间秒部分[秒]")]
        public short Seconds { get; set; } = 0;
        /// <summary>
        /// 剩余总时间
        /// </summary>
        [Description("剩余总时间[秒]")]
        public int TotalSeconds
        {
            get
            {
                return this.Hours * 3600 + this.Minutes * 60 + this.Seconds;
            }
        }
        #endregion
    }
    #endregion
    #region NBS剩余体积查询反馈11
    public class NBSRemainWorkVDto : NBSBaseDto
    {
        public NBSRemainWorkVDto()
        {

        }
        public NBSRemainWorkVDto(byte[] receiveData, HDNBSCmdCode cmd)
        {
            /*****************
             （3）返回内容说明：68 07 07 68 83 05 AA XX XX XX XX CH 0D，
            AA为占位符，固定值0x00，XX XX XX XX为剩余体积值，占4个字节，范围是0-9999999，高字节在前，低字节在后，
            数据类型为int32；实际值为设备存储值除以10，即实际值可以保存一位小数；
            返回错内容：68 03 03 68 83 05 FF CH 0D，返回的编码内容是固定的（CH未计算）
                * ***************/
            this.Cmd = cmd;
            this.Title = "剩余体积查询";
            if (7 == receiveData[1])
            {
                //获取小时部分，注意实际值=获取值/10；实际值是保留小数点一位的数值
                this.RemainV = StringHelper.BitToInt(new byte[] { receiveData[7], receiveData[8], receiveData[9], receiveData[10] })/10M;
            }
            else if (9 == receiveData.Length && 0xFF == receiveData[6])
            {
                this.Sucessfully = false;
                this.ErrMsg = string.Format("设备返回0xFF");
            }
            else
            {
                this.Sucessfully = false;
                this.ErrMsg = string.Format("数据解析失败:{0}", StringHelper.ByteToHexStr(receiveData));
            }
        }
        #region 属性值
        /// <summary>
        /// 剩余体积
        /// </summary>
        [Description("剩余体积[m³]")]
        public decimal RemainV { get; set; }
        #endregion
    }
    #endregion
    #region NBS运行模式查询反馈12
    public class NBSWorkModeVDto : NBSBaseDto
    {
        public NBSWorkModeVDto()
        {

        }
        public NBSWorkModeVDto(byte[] receiveData, HDNBSCmdCode cmd)
        {
            /*****************
             返回内容说明：68 04 04 68 83 05 aa XX CH 0D，aa是占位符，固定的0值，
            XX为当前运行模式值，00为半自动定量；01位半自动定时；
             返回错内容：68 03 03 68 83 05 FF CH 0D，返回的编码内容是固定的（CH未计算）
                * ***************/
            this.Cmd = cmd;
            this.Title = "剩余体积查询";
            if (4 == receiveData[1])
            {
                //运行模式值
                this.WorkMode = (short)receiveData[7];
            }
            else if (9 == receiveData.Length && 0xFF == receiveData[6])
            {
                this.Sucessfully = false;
                this.ErrMsg = string.Format("设备返回0xFF");
            }
            else
            {
                this.Sucessfully = false;
                this.ErrMsg = string.Format("数据解析失败:{0}", StringHelper.ByteToHexStr(receiveData));
            }
        }
        #region 属性值
        /// <summary>
        /// 运行模式
        /// </summary>
        [Description("运行模式[00定量，01定时]")]
        public short WorkMode { get; set; }
        #endregion
    }
    #endregion
    #region NBS运行开始时间查询反馈13
    public class NBSWorkStartTimeVDto : NBSBaseDto
    {
        public NBSWorkStartTimeVDto()
        {

        }
        public NBSWorkStartTimeVDto(byte[] receiveData, HDNBSCmdCode cmd)
        {
            /*****************
             返回内容说明：68 07 07 68 8A 05 YY MM DD HH mm CH 0D，YY为年份，MM为月份 DD为日，HH 为时，mm为分；
            注意看一下协议《NBS-2.0通讯协议-20200331.pdf》，关于“运行开始时间查询”这一段暂时有点问题，暂定5位数据字节；
            返回错误内容说明：68 03 03 68 8A 05 FF CH 0D，返回的编码内容是固定的（CH未计算）
                * ***************/
            this.Cmd = cmd;
            this.Title = "运行开始时间查询";
            if (7 == receiveData[1])
            {
                //解析时间
                this.Year = (short)receiveData[6];
                this.Month = (short)receiveData[7];
                this.Day = (short)receiveData[8];
                this.Hour = (short)receiveData[9];
                this.Month = (short)receiveData[10];
            }
            else if (9 == receiveData.Length && 0xFF == receiveData[6])
            {
                this.Sucessfully = false;
                this.ErrMsg = string.Format("设备返回0xFF");
            }
            else
            {
                this.Sucessfully = false;
                this.ErrMsg = string.Format("数据解析失败:{0}", StringHelper.ByteToHexStr(receiveData));
            }
        }
        #region 属性值
        /// <summary>
        /// 运行开始时间年份部分
        /// </summary>
        [Description("年份")]
        public short Year { get; set; }
        /// <summary>
        /// 运行开始时间月份部分
        /// </summary>
        [Description("月份")]
        public short Month { get; set; }
        /// <summary>
        /// 运行开始时间日部分
        /// </summary>
        [Description("日")]
        public short Day { get; set; }
        /// <summary>
        /// 运行开始时间小时部分
        /// </summary>
        [Description("小时")]
        public short Hour { get; set; }
        /// <summary>
        /// 运行开始时间分钟部分
        /// </summary>
        [Description("分钟")]
        public short Minute { get; set; }
        public string StartTime
        {
            get
            {
                return new DateTime(this.Year, this.Month, this.Day, this.Hour, this.Minute, 0).ToString("yyyy-MM-dd HH:mm:ss");
            }
        }
        #endregion
    }
    #endregion
    #region 瞬时数据反馈14
    /// <summary>
    /// 瞬时数据，功能码D0,反馈：D1
    /// </summary>

    public class NBSRealDataDto : NBSBaseDto
    {
        public NBSRealDataDto()
        {
            this.Title = "瞬时数据";
        }
        public NBSRealDataDto(byte[] receiveData, HDNBSCmdCode cmd)
        {
            this.Cmd = cmd;
            this.Title = "瞬时数据";
            /********************
             * 
             * *****************/
            //获取测试的项目
            byte bTestItems = receiveData[6];//测试项目都存储在了一个字节中
            string strBitStr = Aming.Core.StringHelper.ByteToBinString8(bTestItems);
            /*********
             * strBitStr获取的字节对应的值说明：
            0;瞬时工况与标况是否测试
            1;//大气压是否测试
            2;//无项目，也要检测是否测试
            3;//无项目，也要检测是否测试
            4;//流量（瞬时值）是否测试
            5;//箱体温度是否测试
            6;//环境温度是否测试
            7; //计前温度是否测试
            值解析如下：
            （4）返回内容说明：这个值和气碘解析是一致的；68 15 15 68 D1 05 CF AA AA AA AA BB BB BB BB CC CC DD DD EE EE FF FF GG GG CH 0D(CH未计算) 分析如下:
                AA AA AA AA：工况值 ，单位m³，左边是高位，右边是低位，实际值要除以10；
                BB BB BB BB：标况值，单位：m³，左边是高位，右边是低位，实际值要除以10；
                CC CC：大气压，单位kpa；左边是高位，右边是低位，实际值要除以10；
                DD DD：流量瞬时值，单位：m³/h，，左边是高位，右边是低位，实际值要除以10；
                EE EE：箱体温度，单位：℃，左边是高位，右边是低位，实际值=（获取值-200）/10
                FF FF：环境温度，单位：℃，左边是高位，右边是低位，实际值=（获取值-200）/10
                GG GG：计前温度，单位：℃，左边是高位，右边是低位，实际值=（获取值-200）/10
                有效数据长度从AA到GG共18个字节，这个和协议表格中各项目长度之和一致；所以返回字节的第二位和第三位值=1+18+2=21（hex：15）
             * ***************/
            //这里注意，返回数据的顺序是瞬时工况与标况在前面，相关温度在后面；
            int iStartIndex = 7;
            int iTempValue;
            if (strBitStr[0] == '1')
            {
                //读取标况数据、工况数据；这两个数据是一起的，要有一起有，要无一起无
                iTempValue = Aming.Core.StringHelper.BitToInt(new byte[] { receiveData[iStartIndex + 3], receiveData[iStartIndex + 2], receiveData[iStartIndex + 1], receiveData[iStartIndex] });
                this.WorkingVolume = iTempValue / 10M;
                iStartIndex += 4;
                //读取标况数据
                iTempValue = Aming.Core.StringHelper.BitToInt(new byte[] { receiveData[iStartIndex + 3], receiveData[iStartIndex + 2], receiveData[iStartIndex + 1], receiveData[iStartIndex] });
                this.StandardVolume = iTempValue / 10M;
                iStartIndex += 4;
            }
            for (int i = 1; i < 8; i++)
            {
                if (strBitStr[i] == '1')
                {
                    iTempValue = Aming.Core.StringHelper.BitToInt(new byte[] { receiveData[iStartIndex + 1], receiveData[iStartIndex], 0x00, 0x00 });
                    iStartIndex += 2;
                    switch (i)
                    {
                        case 1:
                            this.Atmosphere = iTempValue / 10M;//大气压
                            break;
                        case 4:
                            this.InstantaneousFlow = iTempValue / 10M;//流量（瞬间）
                            break;
                        case 5:
                            this.BoxTemperature = (iTempValue - 200) / 10M;//箱体温度
                            break;
                        case 6:
                            this.Temperature = (iTempValue - 200) / 10M;//环境温度
                            break;
                        case 7:
                            this.BeforeWorkTemperature = (iTempValue - 200) / 10M;//计前温度
                            break;
                    }
                }
            }
        }
        #region 属性
        /// <summary>
        /// 工况，单位：立方米
        /// </summary>
        [Description("工况[(m³)]")]
        public decimal WorkingVolume { get; set; }
        /// <summary>
        /// 标况，单位：立方米
        /// </summary>
        [Description("标况[(m³)]")]
        public decimal StandardVolume { get; set; }
        /// <summary>
        /// 大气压
        /// </summary>
        [Description("大气压[(kpa)]")]
        public decimal Atmosphere { get; set; }
        /// <summary>
        /// 流量（瞬时值）,单位L/min
        /// </summary>
        [Description("流量[(L/min)]")]
        public decimal InstantaneousFlow { get; set; }
        /// <summary>
        /// 箱体温度
        /// </summary>
        [Description("箱体温度[(℃)]")]
        public decimal BoxTemperature { get; set; }
        /// <summary>
        /// 环境温度
        /// </summary>
        [Description("环境温度[(℃)]")]
        public decimal Temperature { get; set; }
        /// <summary>
        /// 计前温度
        /// </summary>
        [Description("计前温度[(℃)]")]
        public decimal BeforeWorkTemperature { get; set; }
        #endregion
    }
    #endregion
    #region NBS当前最大文件号反馈15
    public class NBSFileNoDto : NBSBaseDto
    {
        public NBSFileNoDto()
        {
            this.Title = "文件号查询";
        }
        public NBSFileNoDto(byte[] receiveData, HDNBSCmdCode cmd)
        {
            this.Cmd = cmd;
            this.Title = "文件号查询";
            this.MaxFileNo = Aming.Core.StringHelper.BitToInt(new byte[] { receiveData[8], receiveData[7], 0x00, 0x00 });
        }
        #region 属性
        /// <summary>
        /// 当前最大文件号
        /// </summary>
        [Description("最大文件号")]
        public int MaxFileNo { get; set; }
        #endregion
    }
    #endregion
    #region NBS文件查询反馈16
    public class NBSFileInfoDto : NBSBaseDto
    {
        public NBSFileInfoDto()
        {
            this.Title = "文件信息";
        }
        public NBSFileInfoDto(byte[] receiveData, HDNBSCmdCode cmd)
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
    #region 设备故障反馈17
    /// <summary>
    /// 设备报警，功能码0x70，反馈0x71
    /// </summary>
    public class NBSMacAlermDto : NBSBaseDto
    {
        public NBSMacAlermDto()
        {
            this.Title = "设备故障";
        }
        public NBSMacAlermDto(byte[] receiveData, HDNBSCmdCode cmd)
        {
            this.Cmd = cmd;
            this.Title = "设备故障";
            if(receiveData.Length==9 && 0xFF == receiveData[6])
            {
                //此时为读取失败
                this.Sucessfully = false;
                this.ErrMsg = "设备反馈0xFF!";
            }
            else if (receiveData.Length == 9)
            {
                //此时设备返回成功
                #region  解析数据
                this.AlarmValue = receiveData[6];
                switch (this.AlarmValue)
                {
                    case 0x00:
                        this.AlarmDesc = "无故障";
                        break;
                    case 0x01:
                        this.AlarmDesc = "保留";
                        break;
                    case 0x02:
                        this.AlarmDesc = "压膜失败";
                        break;
                    case 0x03:
                    case 0x04:
                        this.AlarmDesc = "变频器故障";
                        break;
                    case 0x05:
                        this.AlarmDesc = "上电故障";
                        break;
                    case 0x06:
                        this.AlarmDesc = "堵转或过热保护";
                        break;
                    default:
                        this.AlarmDesc = "未知故障";
                        break;
                }
                #endregion
            }
            else
            {
                this.Sucessfully = false;
                this.ErrMsg = string.Format("数据解析失败:{0}", StringHelper.ByteToHexStr(receiveData));
            }
        }
        #region 属性
        /// <summary>
        /// 报警故障值
        /// </summary>
        public byte AlarmValue { get; set; }
        /// <summary>
        /// 故障描述
        /// </summary>
        public string AlarmDesc { get; set; }
        #endregion
    }
    #endregion

    
    #region 停止采样反馈
    /// <summary>
    /// 停止采样
    /// </summary>
    public class NBSStopWorkingDto : NBSBaseDto
    {
        public NBSStopWorkingDto()
        {
            this.Title = "停止采样";
        }
        public NBSStopWorkingDto(byte[] receiveData, HDNBSCmdCode cmd)
        {
            this.Cmd = cmd;
            this.Title = "停止采样";
            byte b = receiveData[6];
            if (b == 0x00)
            {
                this.StopSucessful = true;
                this.Sucessfully = true;
                this.ErrMsg = string.Empty;
            }
            else if (b == 0xFF)
            {
                this.StopSucessful = false;
                this.Sucessfully = true;
                this.ErrMsg = "设备反馈失败";
            }
            else
            {
                this.Sucessfully = false;
                this.ErrMsg = string.Format("判断是否成功的标识位[{0}]不是预期的。", b);
            }
        }
        #region 属性
        /// <summary>
        /// 停止是否成功
        /// </summary>
        [Description("是否成功停止")]
        public bool StopSucessful { get; set; }
        #endregion
    }
    #endregion
    #region 设备编号查询反馈
    public class NBSMacNumberDto : NBSBaseDto
    {
        public NBSMacNumberDto()
        {
            this.Title = "设备编号查询";
        }
        public NBSMacNumberDto(byte[] receiveData, HDNBSCmdCode cmd)
        {
            this.Cmd = cmd;
            this.Title = "设备编号查询";
            short iLen = receiveData[1];
            //通过iLen推算出编号的位数，这个参照协议说明文件
            int iStart = 7;
            int iNumberLen = iLen - 3;//多出来的那3个时最后的尾帧、校验位
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < iNumberLen; i++)
            {
                sb.Append((char)receiveData[iStart + i]);//设备编号为ASCII值
            }
            this.Number = sb.ToString();
        }
        #region  属性
        /// <summary>
        /// 设备编号
        /// </summary>
        [Description("设备编号")]
        public string Number { get; set; } = string.Empty;
        #endregion
    }
    #endregion
    
    
    #region 定量采样量设定反馈
    public class NBSSamplingVSetDto : NBSBaseDto
    {
        public NBSSamplingVSetDto()
        {

        }
        public NBSSamplingVSetDto(byte[] receiveData, HDNBSCmdCode cmd)
        {
            /*****************
                * 发送68 06 06 68 50 05 XX XX XX XX CH 0D
                成功返回68 06 06 68 55 01 XX XX XX XX CH 0D
                失败返回：68 03 03 68 51 01 FF 07 0D
                说明:红色部分为定量采样量，四字节，小数点后一位。
                例：00 00 07 D0代表200.0立方，最大980.0。
                * ***************/
            this.Cmd = cmd;
            this.Title = "定量采样量设定反馈";
            short iLen = receiveData[1];
            if (receiveData[1] == 0x03 && receiveData[6] == 0xFF)
            {
                //此时为设备反馈设置失败
                this.Sucessfully = false;
                this.ErrMsg = "设备反馈设定失败";
            }
            else if (receiveData[1] == 0x06)
            {
                //获取小时部分，注意实际值=获取值/10；实际值是保留小数点一位的数值
                this.Volume = StringHelper.BitToInt(new byte[] { receiveData[9], receiveData[8], receiveData[7], receiveData[6] }) / 10M;
            }
            else
            {
                this.Sucessfully = false;
                this.ErrMsg = string.Format("数据解析失败:{0}", StringHelper.ByteToHexStr(receiveData));
            }
        }
        #region 属性值
        /// <summary>
        /// 采样量体积设定值
        /// </summary>
        [Description("采样量体积设定值[m³]")]
        public decimal Volume { get; set; } = 0;
        #endregion
    }
    #endregion
    
    #region 测试命令反馈
    /// <summary>
    /// 检查设备通讯是否正常
    /// </summary>
    public class NBSCommunicationCheckDto : NBSBaseDto
    {
        public NBSCommunicationCheckDto()
        {

        }
        public NBSCommunicationCheckDto(byte[] receiveData, HDNBSCmdCode cmd)
        {
            /*****************
            13.1 发送 68 03 03 68 FA 05 00 CH 0D
            13.2 返回 68 03 03 68 FB 05 00 CH 0D
            说明：该命令主要用于检测设备是否通讯正常（建议定时检测该命令）
                * ***************/
            this.Cmd = cmd;
            this.Title = "测试命令反馈";
            short iLen = receiveData[1];
            if (receiveData[1] == 0x03 && receiveData[6] == 0x00)
            {
                this.Sucessfully = true;
            }
            else
            {
                this.Sucessfully = false;
                this.ErrMsg = string.Format("设别通讯返回错误:{0}", StringHelper.ByteToHexStr(receiveData));
            }
        }
    }
    #endregion
    
    #region 采样次数设定反馈
    /// <summary>
    /// 采样次数设定反馈信息
    /// </summary>
    public class NBSSampllingCountSetDto : NBSBaseDto
    {
        public NBSSampllingCountSetDto()
        {

        }
        public NBSSampllingCountSetDto(byte[] receiveData, HDNBSCmdCode cmd)
        {
            /*****************
            发送68 06 06 68 51 05 XX XX XX XX 5C   0D
            成功返回68 06 06 68 52 05 00 00 00 06 5D 0D
            失败返回68 03 03 68 52 05 FF 56 0D
            说明:红色部分为采样次数，四字节。
            例：00 00 00 06 代表6次     最大9次最小1次
             ***************/
            this.Cmd = cmd;
            this.Title = "设置信息查询反馈";
            short iLen = receiveData[1];
            if (receiveData[1] == 0x03 && receiveData[6] == 0xFF)
            {
                //此时为设备反馈设置失败
                this.Sucessfully = false;
                this.ErrMsg = "设备反馈设定失败";
            }
            else if (receiveData[1] == 0x06)
            {
                this.SampllingCount = StringHelper.BitToInt(new byte[] { receiveData[9], receiveData[8], receiveData[7], receiveData[6] });
            }
            else
            {
                this.Sucessfully = false;
                this.ErrMsg = string.Format("数据解析失败:{0}", StringHelper.ByteToHexStr(receiveData));
            }
        }
        #region 属性值
        /// <summary>
        /// 采样次数设定值
        /// </summary>
        [Description("采样次数设定值")]
        public int SampllingCount { get; set; } = 0;
        #endregion
    }
    #endregion
    #region 定时采样间隔反馈信息
    /// <summary>
    /// 定时采样间隔反馈信息
    /// </summary>
    public class NBSSampllingIntervalSetDto : NBSBaseDto
    {
        public NBSSampllingIntervalSetDto()
        {

        }
        public NBSSampllingIntervalSetDto(byte[] receiveData, HDNBSCmdCode cmd)
        {
            /*****************
            发送68 04 04 68 52  05 XX XX 79 0D
            成功返回68 06 06 68 53 05 02 20 7A 0D
            失败返回68 03 03 68 53 05 FF 57 0D
            说明:红色部分为定时采样间隔，二字节。
            例：02 20 代表2和小时32分钟。
            这里要设置2个属性，一个存储小时部分，一个存储分钟部分，且都为整型
             ***************/
            this.Cmd = cmd;
            this.Title = "采样间隔设置反馈";
            short iLen = receiveData[1];
            if (receiveData[1] == 0x03 && receiveData[6] == 0xFF)
            {
                //此时为设备反馈设置失败
                this.Sucessfully = false;
                this.ErrMsg = "设备反馈设定失败";
            }
            else if (receiveData[1] == 0x04)
            {
                this.IntervalHour = StringHelper.BytesToShort(new byte[] { 0x00, receiveData[6] }, 0);
                this.IntervalMinute = StringHelper.BytesToShort(new byte[] { 0x00, receiveData[7] }, 0);
            }
            else
            {
                this.Sucessfully = false;
                this.ErrMsg = string.Format("数据解析失败:{0}", StringHelper.ByteToHexStr(receiveData));
            }
        }
        #region 属性值
        /// <summary>
        /// 定时采样间隔的小时部分值
        /// </summary>
        [Description("定时采样间隔的小时部分值[小时]")]
        public short IntervalHour { get; set; } = 0;
        /// <summary>
        /// 定时采样间隔的分钟部分值
        /// </summary>
        [Description("定时采样间隔的分钟部分值[分钟]")]
        public short IntervalMinute { get; set; } = 0;
        #endregion
    }
    #endregion
    #region 定时启动时间设置反馈
    /// <summary>
    /// 定时启动时间设置反馈
    /// </summary>
    public class NBSAutoStartTimeSetDto : NBSBaseDto
    {
        public NBSAutoStartTimeSetDto()
        {

        }
        public NBSAutoStartTimeSetDto(byte[] receiveData, HDNBSCmdCode cmd)
        {
            /*****************
            发送68 07 07 68 53 05 XX XX XX XX XX 95   0D
            成功返回68 07 07 68 54 05 11 05 11 11 05 96 0D
            失败返回68 03 03 68 54 05 FF 58 0D
            说明：格式为十六进制的年月日时分。
            例：11 05 11  11 05  代表17年05月17日17时5分
            这里无需精确到秒；
                * ***************/
            this.Cmd = cmd;
            this.Title = "定时启动时间设定反馈";
            short iLen = receiveData[1];
            if (receiveData[1] == 0x03 && receiveData[6] == 0xFF)
            {
                //此时为设备反馈设置失败
                this.Sucessfully = false;
                this.ErrMsg = "设备反馈设定失败";
            }
            else if (receiveData[1] == 0x07)
            {
                //获取小时部分，注意实际值=获取值/10；实际值是保留小数点一位的数值
                string strY = StringHelper.BytesToShort(new byte[] { 0x00, receiveData[6] }, 0).ToString().PadLeft(2, '0');
                string strM = StringHelper.BytesToShort(new byte[] { 0x00, receiveData[7] }, 0).ToString().PadLeft(2, '0');
                string strD = StringHelper.BytesToShort(new byte[] { 0x00, receiveData[8] }, 0).ToString().PadLeft(2, '0');
                string strHour = StringHelper.BytesToShort(new byte[] { 0x00, receiveData[9] }, 0).ToString().PadLeft(2, '0');
                string strMin = StringHelper.BytesToShort(new byte[] { 0x00, receiveData[10] }, 0).ToString().PadLeft(2, '0');
                this.Time = string.Format("20{0}-{1}-{2} {3}:{4}", strY, strM, strD, strHour, strMin);
            }
            else
            {
                this.Sucessfully = false;
                this.ErrMsg = string.Format("数据解析失败:{0}", StringHelper.ByteToHexStr(receiveData));
            }
        }
        #region 属性值
        /// <summary>
        /// 自启动时间设定值
        /// </summary>
        [Description("自启动时间设定值")]
        public string Time { get; set; } = "";
        #endregion
    }
    #endregion
    #region 定时启动使能设置反馈
    /// <summary>
    /// 定时启动使能设置反馈
    /// </summary>
    public class NBSAutoStartSetDto : NBSBaseDto
    {
        public NBSAutoStartSetDto()
        {

        }
        public NBSAutoStartSetDto(byte[] receiveData, HDNBSCmdCode cmd)
        {
            /*****************
            发送68 03 03 68 54  05 XX  5a 0D
            成功返回68 03 03 68 55 05 01 5B 0D
            失败返回68 03 03 68 55 05 FF 59 0D
            说明:红色部分为开关量，一个字节。
            例：01代表定时启动使能 否则代表禁止定时启动。
                * ***************/
            this.Cmd = cmd;
            this.Title = "定时启动使能设定反馈";
            short iLen = receiveData[1];
            if (receiveData[1] == 0x03)
            {
                switch (receiveData[6])
                {
                    case 0xFF:
                        this.Sucessfully = false;
                        this.ErrMsg = "设备反馈设定失败";
                        break;
                    case 0x00:
                        this.AutoStart = false;
                        break;
                    case 0x01:
                        this.AutoStart = true;
                        break;
                    default:
                        this.Sucessfully = false;
                        this.ErrMsg = string.Format("数据解析失败:{0}", StringHelper.ByteToHexStr(receiveData));
                        break;
                }
            }
            else
            {
                this.Sucessfully = false;
                this.ErrMsg = string.Format("数据解析失败:{0}", StringHelper.ByteToHexStr(receiveData));
            }
        }
        #region 属性值
        /// <summary>
        /// 时间设定值
        /// </summary>
        [Description("定时启动使能[0：不启用，1启用]")]
        public bool AutoStart { get; set; } = false;
        #endregion
    }
    #endregion
}
