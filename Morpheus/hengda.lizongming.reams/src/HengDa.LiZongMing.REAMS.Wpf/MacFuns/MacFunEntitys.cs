using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HengDa.LiZongMing.REAMS.Wpf.MacFuns.ZCQMacEntitys
{
    [AddINotifyPropertyChangedInterface]
    public class ZCQReceiveBase
    {
        public string Title { get; set; }
        /// <summary>
        /// 解析传入的数据，判断是否是当前的
        /// </summary>
        /// <param name="receiveData"></param>
        /// <returns></returns>
        public virtual bool IsMyData(ref byte[] receiveData)
        {
            return false;
        }
        /// <summary>
        /// 解析数据
        /// </summary>
        /// <param name="receiveData">接收的数据</param>
        /// <param name="sErr">错误消息</param>
        /// <returns>是否解析成功</returns>
        public virtual bool AnalyzeData(ref byte[] receiveData, out string sErr)
        {
            //校验数据长度是否符合
            if (receiveData == null || receiveData.Length == 0)
            {
                sErr = "返回结果为空！";
                return false;
            }
            //返回的内容应该为：68 N N 68 .....，总长度应该为N+6；
            if (receiveData.Length <= 4)
            {
                sErr = string.Format("返回的数据不正确：{0}", Funs.GetByteToHex(receiveData));
                return false;
            }
            int iLen = (int)receiveData[1];//用于读取返回的数据FN+DT+FNC部分的长度
            iLen += 6;//该设备的通讯协议规则是返回的总长度为FN+DT+FNC+6；发送和返回的字节规则都一样
            if(receiveData.Length!=iLen)
            {
                sErr = string.Format("返回的字节长度不是预期的{0}位：{1}", iLen, Funs.GetByteToHex(receiveData));
                return false;
            }
            if(receiveData[0]!=0x68 || receiveData[3] != 0x68)
            {
                sErr = string.Format("返回的数据缺少帧头：{1}", iLen, Funs.GetByteToHex(receiveData));
                return false;
            }
            return this.ProData(ref receiveData, out sErr);
        }
        /// <summary>
        /// 解析数据
        /// </summary>
        /// <param name="receiveData">接收到的数据</param>
        /// <param name="sErr">解析错误时的反馈消息</param>
        /// <returns>解析是否正确</returns>
        public virtual bool ProData(ref byte[] receiveData, out string sErr)
        {
            sErr = string.Empty;
            return true;
        }
    }
    #region 瞬时数据
    /// <summary>
    /// 瞬时数据，功能码D0,反馈：D1
    /// </summary>

    public class ZCQReceive_RealData : ZCQReceiveBase
    {
        #region 属性
        decimal mGongKuang = 0;
        /// <summary>
        /// 工况，单位：立方米
        /// </summary>
        public decimal GongKuang{get;set;}

        /// <summary>
        /// 标况，单位：立方米
        /// </summary>
        public decimal BiaoKuang
        {
            get;
            set;
        }
        decimal mDaQiYa = 0;
        public decimal DaQiYa
        {
            get
            {
                return this.mDaQiYa;
            }
            set
            {
                this.mDaQiYa = value;
            }
        }
        decimal mLiuLiang = 0;
        /// <summary>
        /// 流量（瞬时值）,单位L/min
        /// </summary>
        public decimal LiuLiang
        {
            get
            {
                return this.mLiuLiang;
            }
            set
            {
                this.mLiuLiang = value;
            }
        }
        decimal mHuanJingWd = 0;
        /// <summary>
        /// 环境温度，单位℃
        /// </summary>
        public decimal HuanJingWd
        {
            get;set;
        }
        #endregion
        public override bool IsMyData(ref byte[] receiveData)
        {
            if (receiveData == null || receiveData.Length < 5) return false;
            return receiveData[4] == 0xD1;
        }
        public override bool ProData(ref byte[] receiveData, out string sErr)
        {
            //解析瞬时数据
            /*************
             * 该过程要注意：测试的项目选择不同，则反馈的数据长度和内容也就不同，但还是有规律的：
             * 1、返回的数据中是按照项目固定顺序排列的，依次从前到后为：瞬时工况与标况、大气压、流量（瞬时值）、环境温度
             * **************************/
            if (receiveData.Length < 7)
            {
                sErr = string.Format("瞬时数据返回长度不正确：{0}", Funs.GetByteToHex(receiveData));
                return false;
            }
            //获取测试的项目
            byte bTestItems = receiveData[6];
            bool blTested0, blTested1, blTested2, blTested3, blTested4, blTested5, blTested6, blTested7;
            blTested0 = (bTestItems & 0x01) == 0x01;
            blTested1 = ((bTestItems >> 1) & 0x01) == 0x01;
            blTested2 = ((bTestItems >> 2) & 0x01) == 0x01;
            blTested3 = ((bTestItems >> 3) & 0x01) == 0x01;
            blTested4 = ((bTestItems >> 4) & 0x01) == 0x01;
            blTested5 = ((bTestItems >> 5) & 0x01) == 0x01;
            blTested6 = ((bTestItems >> 6) & 0x01) == 0x01;
            blTested7 = ((bTestItems >> 7) & 0x01) == 0x01;
            //以上blTested1对应的是环境温度是否测试，blTested7对应的是瞬时工况与标况是否测试；
            //这里注意，返回数据的顺序是瞬时工况与标况在前面，环境温度在后面；
            int iStartIndex = 7;
            int iLen;
            int iTempValue;
            if(blTested7)
            {
                //此时为瞬时工况与标况，且前的工况，后面的标况；
                iLen = 4;
                if (!this.ProData_Format(ref receiveData, iStartIndex, iLen, out iTempValue, out sErr)) return false;
                iStartIndex += iLen;
                this.GongKuang = iTempValue / 10M;
                //此时为瞬时工况与标况，且前的工况，后面的标况；
                if (!this.ProData_Format(ref receiveData, iStartIndex, iLen, out iTempValue, out sErr)) return false;
                iStartIndex += iLen;
                this.BiaoKuang = iTempValue / 10M;
            }
            if(blTested6)
            {
                //大气压,小数点后一位，单位kpa；长度为2；
                iLen = 2;
                if (!this.ProData_Format(ref receiveData, iStartIndex, iLen, out iTempValue, out sErr)) return false;
                iStartIndex += iLen;
                this.DaQiYa = iTempValue / 10M;
            }
            if (blTested5)
            {
                //无项目，但有可能会包含；长度为2；
                iLen = 2;
                if (!this.ProData_Format(ref receiveData, iStartIndex, iLen, out iTempValue, out sErr)) return false;
                iStartIndex += iLen;
            }
            if (blTested4)
            {
                //无项目，但有可能会包含；长度为2；
                iLen = 2;
                if (!this.ProData_Format(ref receiveData, iStartIndex, iLen, out iTempValue, out sErr)) return false;
                iStartIndex += iLen;
            }
            if (blTested3)
            {
                //流量（瞬时值）,单位L/min
                iLen = 2;
                if (!this.ProData_Format(ref receiveData, iStartIndex, iLen, out iTempValue, out sErr)) return false;
                iStartIndex += iLen;
                this.LiuLiang = (decimal)iTempValue;
            }

            if (blTested2)
            {
                //无项目，长度是2；
                iLen = 2;
                if (!this.ProData_Format(ref receiveData, iStartIndex, iLen, out iTempValue, out sErr)) return false;
                iStartIndex += iLen;
            }
            if (blTested1)
            {
                //环境温度，小数点后一位，单位℃，注意，需要减200再除以10，才是实际温度，长度是2；
                iLen = 2;
                if (!this.ProData_Format(ref receiveData, iStartIndex, iLen, out iTempValue, out sErr)) return false;
                iStartIndex += iLen;
                this.HuanJingWd = (iTempValue - 200) / 10M;
            }
            //blTested不用读取了，目前没项目；
            sErr = string.Empty;
            return true;
        }
        private bool ProData_Format(ref byte[] receiveData,int iStart,int iLen,out int iValue, out string sErr)
        {
            if(receiveData.Length<(iStart+iLen))
            {
                sErr = string.Format("瞬时数据解析是长度不符合：{0}", Funs.GetByteToHex(receiveData));
                iValue = 0;
                return false;
            }
            //将数据转换成INT，瞬时数据采集的返回数据有个特点，返回的数据，前面字节是高位，后面的是高位；
            if (iLen == 4)
            {
                iValue = BitConverter.ToInt32(new byte[] { receiveData[iStart + 3], receiveData[iStart + 2], receiveData[iStart + 1], receiveData[iStart] });//注意：byte[]数值，高位在后，低位在前
            }
            else if (iLen == 2)
            {
                iValue = BitConverter.ToInt32(new byte[] { receiveData[iStart + 1], receiveData[iStart], 0x00, 0x00 });//注意：byte[]数值，高位在后，低位在前
            }
            else
            {
                sErr = string.Format("传入的字节长度只能是2或4");
                iValue = 0;
                return false;
            }
            sErr = string.Empty;
            return true;
        }
    }
    #endregion
    #region 设备故障
    /// <summary>
    /// 设备报警，功能码0x70，反馈0x71
    /// </summary>
    public class ZCQReceive_MacAlerm : ZCQReceiveBase
    {
        public ZCQReceive_MacAlerm()
        {
            this.Title = "设备报警";
        }
        #region 属性
        bool mDoorStauts = false;
        /// <summary>
        /// 舱门状态，true为报警，false为正常
        /// </summary>
        public bool DoorStauts
        {
            get
            {
                return this.mDoorStauts;
            }
            set
            {
                this.mDoorStauts = value;
            }
        }
        bool mLCDStatus = false;
        /// <summary>
        /// LCD连接状态，true为报警，false为正常
        /// </summary>
        public bool LCDStatus
        {
            get
            {
                return this.mLCDStatus;
            }
            set
            {
                this.mLCDStatus = value;
            }
        }
        bool mDaQiB = false;
        /// <summary>
        /// 大气B是否超限，true为报警，false为正常
        /// </summary>
        public bool DaQiB
        {
            get
            {
                return this.mDaQiB;
            }
            set
            {
                this.mDaQiB = value;
            }
        }
        bool mDaQiA = false;
        /// <summary>
        /// 大气A是否超限，true为报警，false为正常
        /// </summary>
        public bool DaQiA
        {
            get
            {
                return this.mDaQiA;
            }
            set
            {
                this.mDaQiA = value;
            }
        }
        bool mQiDian = false;
        /// <summary>
        /// 气碘超限，true为报警，false为正常
        /// </summary>
        public bool QiDian
        {
            get
            {
                return this.mQiDian;
            }
            set
            {
                this.mQiDian = value;
            }
        }
        bool mB_FS4003 = false;
        /// <summary>
        /// B电子流量计FS4003故障，true为报警，false为正常
        /// </summary>
        public bool B_FS4003
        {
            get
            {
                return this.mB_FS4003;
            }
            set
            {
                this.mB_FS4003 = value;
            }
        }
        bool mA_FS4003 = false;
        /// <summary>
        /// A电子流量计FS4003故障，true为报警，false为正常
        /// </summary>
        public bool A_FS4003
        {
            get
            {
                return this.mA_FS4003;
            }
            set
            {
                this.mA_FS4003 = value;
            }
        }
        bool mYcSensor = false;
        /// <summary>
        /// 差压传感器故障，true为报警，false为正常
        /// </summary>
        public bool YcSensor
        {
            get
            {
                return this.mYcSensor;
            }
            set
            {
                this.mYcSensor = value;
            }
        }
        bool mDaQiModule = false;
        /// <summary>
        /// 大气压模块故障，true为报警，false为正常
        /// </summary>
        public bool DaQiModule
        {
            get
            {
                return this.mDaQiModule;
            }
            set
            {
                this.mDaQiModule = value;
            }
        }
        bool mTemModule = false;
        /// <summary>
        /// 温度模块故障，true为报警，false为正常
        /// </summary>
        public bool TemModule
        {
            get
            {
                return this.mTemModule;
            }
            set
            {
                this.mTemModule = value;
            }
        }
        #endregion
        public override bool IsMyData(ref byte[] receiveData)
        {
            if (receiveData == null || receiveData.Length < 5) return false;
            return receiveData[4] == 0x71;
        }
        public override bool ProData(ref byte[] receiveData, out string sErr)
        {
            if (receiveData.Length < 10)
            {
                sErr = string.Format("设备报警返回数据长度不正确:{0}", Funs.GetByteToHex(receiveData));
                return false;
            }
            byte b1, b2;
            b1 = receiveData[6];
            b2 = receiveData[7];
            //第1个字节解析
            //舱门状态
            if (((b1 >> 1) & 0x01) == 0x01)
            {
                //报警
                if (!this.DoorStauts)
                    this.DoorStauts = true;
            }
            else
            {
                //正常
                if (this.DoorStauts)
                    this.DoorStauts = (((b1 >> 1) & 0x01) == 0x01);
            }
            //LCD状态
            if ((b1 & 0x01) == 0x01)
            {
                //报警
                if (!this.LCDStatus)
                    this.LCDStatus = true;
            }
            else
            {
                //正常
                if (this.LCDStatus)
                    this.LCDStatus = false;
            }
            //第二个字节解析
            for (int i = 7; i >= 0; i--)
            {
                if (((b2 >> i) & 0x01) == 0x01)
                {
                    #region 此时为报警状态
                    if (i==0)
                    {
                        if (!this.TemModule)
                            this.TemModule = true;
                    }
                    else if (i == 1)
                    {
                        if (!this.DaQiModule)
                            this.DaQiModule = true;
                    }
                    else if (i == 2)
                    {
                        if (!this.YcSensor)
                            this.YcSensor = true;
                    }
                    else if (i == 3)
                    {
                        if (!this.A_FS4003)
                            this.A_FS4003 = true;
                    }
                    else if (i == 4)
                    {
                        if (!this.B_FS4003)
                            this.B_FS4003 = true;
                    }
                    else if (i == 5)
                    {
                        if (!this.QiDian)
                            this.QiDian = true;
                    }
                    else if (i == 6)
                    {
                        if (!this.DaQiA)
                            this.DaQiA = true;
                    }
                    else if (i == 7)
                    {
                        if (!this.DaQiB)
                            this.DaQiB = true;
                    }
                    #endregion
                }
                else
                {
                    #region 此时为正常状态
                    if (i == 0)
                    {
                        if (this.TemModule)
                            this.TemModule = false;
                    }
                    else if (i == 1)
                    {
                        if (this.DaQiModule)
                            this.DaQiModule = false;
                    }
                    else if (i == 2)
                    {
                        if (this.YcSensor)
                            this.YcSensor = false;
                    }
                    else if (i == 3)
                    {
                        if (this.A_FS4003)
                            this.A_FS4003 = false;
                    }
                    else if (i == 4)
                    {
                        if (this.B_FS4003)
                            this.B_FS4003 = false;
                    }
                    else if (i == 5)
                    {
                        if (this.QiDian)
                            this.QiDian = false;
                    }
                    else if (i == 6)
                    {
                        if (this.DaQiA)
                            this.DaQiA = false;
                    }
                    else if (i == 7)
                    {
                        if (this.DaQiB)
                            this.DaQiB = false;
                    }
                    #endregion
                }
            }
            sErr = string.Empty;
            return true;
        }
    }
    #endregion
    #region 当前最大文件号
    public class ZCQReceive_FileNo: ZCQReceiveBase
    {
        public ZCQReceive_FileNo()
        {
            this.Title = "文件号查询";
        }
        #region 属性
        /// <summary>
        /// 当前最大文件号
        /// </summary>
        public short MaxFileNo { get; set; }
        #endregion
        public override bool IsMyData(ref byte[] receiveData)
        {
            if (receiveData == null || receiveData.Length < 5) return false;
            return receiveData[4] == 0xA1;
        }
        public override bool ProData(ref byte[] receiveData, out string sErr)
        {
            if (receiveData.Length != 11)
            {
                sErr = string.Format("{0}返回数据长度不正确:{1}", this.Title, Funs.GetByteToHex(receiveData));
                return false;
            }
            this.MaxFileNo = BitConverter.ToInt16(new byte[] { receiveData[8], receiveData[7] });//高位在后
            sErr = string.Empty;
            return true;
        }
    }
    #endregion
    #region 文件查询
    public class ZCQReceive_FileInfo : ZCQReceiveBase
    {
        public ZCQReceive_FileInfo()
        {
            this.Title = "文件信息";
        }
        #region 属性
        /// <summary>
        /// 文件是否存在
        /// </summary>
        public bool FileExist { get; set; }
        /// <summary>
        ///文件号
        /// <summary>
        public short FileNo { get; set; }
        /// <summary>
        ///滤膜ID号
        /// <summary>
        public string LvMoID { get; set; }
        /// <summary>
        ///开始时间
        /// <summary>
        public string StartTime { get; set; }
        /// <summary>
        ///结束时间
        /// <summary>
        public string EndTime { get; set; }
        /// <summary>
        ///瞬时流量
        /// <summary>
        public decimal LiuLiang { get; set; }
        /// <summary>
        ///标况体积
        /// <summary>
        public decimal BiaoKuangV { get; set; }
        /// <summary>
        ///工况体积
        /// <summary>
        public decimal GongKuangV { get; set; }
        /// <summary>
        ///环境温度
        /// <summary>
        public decimal HuanJingWd { get; set; }
        /// <summary>
        ///环境湿度
        /// <summary>
        public decimal HuanJingSd { get; set; }
        /// <summary>
        ///大气压
        /// <summary>
        public decimal DaQiYa { get; set; }

        #endregion
        public override bool IsMyData(ref byte[] receiveData)
        {
            if (receiveData == null || receiveData.Length < 5) return false;
            return receiveData[4] == 0xB1;
        }
        public override bool ProData(ref byte[] receiveData, out string sErr)
        {
            /*************
             * 文件详细信息的返回数总长度为55个；
             * 但如果查无此文件时会返回固定的内容：68 03 03 68 B1 05 FF B5 0D
             * **************************/
            if(receiveData.Length==9)
            {
                if(receiveData[6]==0xFF)
                {
                    //此时不存在
                    this.FileExist = false;
                    //清空原先数据
                    this.ClearFileInfo();
                    sErr = string.Empty;
                    return true;
                }
                else
                {
                    sErr = string.Format("{0}数据返回不正确：{1}", this.Title, Funs.GetByteToHex(receiveData));
                    return false;
                }
            }
            else if (receiveData.Length != 55)
            {
                sErr = string.Format("{0}返回数据长度不正确:{1}", this.Title, Funs.GetByteToHex(receiveData));
                return false;
            }
            #region 此时开始解析数据
            this.FileExist = true;
            this.FileNo = BitConverter.ToInt16(new byte[] { receiveData[7], receiveData[6] });
            this.LvMoID = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}{13}{14}{15}{16}{17}{18}"
                , (char)receiveData[8], (char)receiveData[9], (char)receiveData[10], (char)receiveData[11], (char)receiveData[12]
                , (char)receiveData[13], (char)receiveData[14], (char)receiveData[15], (char)receiveData[16], (char)receiveData[17]
                , (char)receiveData[18], (char)receiveData[19], (char)receiveData[20], (char)receiveData[21], (char)receiveData[22]
                , (char)receiveData[23], (char)receiveData[24], (char)receiveData[25], (char)receiveData[26]);
            this.StartTime = this.GetTimeString(receiveData[27], receiveData[28], receiveData[29], receiveData[30], receiveData[31]);
            this.EndTime = this.GetTimeString(receiveData[32], receiveData[33], receiveData[34], receiveData[35], receiveData[36]);
            this.LiuLiang = BitConverter.ToInt16(new byte[] { receiveData[38], receiveData[37] }) / 10M;
            this.BiaoKuangV = BitConverter.ToInt32(new byte[] { receiveData[42], receiveData[41], receiveData[40], receiveData[39] }) / 10M;
            this.GongKuangV = BitConverter.ToInt32(new byte[] { receiveData[46], receiveData[45], receiveData[44], receiveData[43] }) / 10M;
            this.HuanJingWd = (BitConverter.ToInt16(new byte[] { receiveData[48], receiveData[47] }) - 200) / 10M;
            this.HuanJingSd = BitConverter.ToInt16(new byte[] { receiveData[50], receiveData[49] }) / 10M;
            #endregion
            sErr = string.Empty;
            return true;
        }
        /// <summary>
        /// 当文件不存在时清空文件内容；
        /// </summary>
        private void ClearFileInfo()
        {
            this.FileNo = 0;
            this.LvMoID = string.Empty;
            this.StartTime = string.Empty;
            this.EndTime = string.Empty;
            this.LiuLiang = 0M;
            this.BiaoKuangV = 0M;
            this.GongKuangV = 0M;
            this.HuanJingWd = 0M;
            this.HuanJingSd = 0M;
            this.DaQiYa = 0M;
        }
        private string GetTimeString(byte bYear,byte bMonth,byte bDay,byte bHour,byte bMinute)
        {
            string strYear = Convert.ToString(bYear);
            while (strYear.Length < 2)
                strYear = "0" + strYear;
            string strMonth = Convert.ToString(bMonth);
            while (strMonth.Length < 2)
                strMonth = "0" + strMonth;
            string strDay = Convert.ToString(bDay);
            while (strDay.Length < 2)
                strDay = "0" + strDay;
            string strHour = Convert.ToString(bHour);
            while (strHour.Length < 2)
                strHour = "0" + strHour;
            string strMinute = Convert.ToString(bMinute);
            while (strMinute.Length < 2)
                strMinute = "0" + strMinute;
            return string.Format("{0}-{1}-{2} {3}:{4}", strYear, strMonth, strDay, strHour, strMinute);
        }
    }
    #endregion
    #region 开始采样
    /// <summary>
    /// 开始采样
    /// </summary>
    public class ZCQReceive_StartWorking:ZCQReceiveBase
    {
        public ZCQReceive_StartWorking()
        {
            this.Title = "开始采样";
        }
        /// <summary>
        /// 采样模式，0：定量，1：定时；-1，未定义
        /// </summary>
        public short WorkMode { get; set; } = -1;
        /// <summary>
        /// 启动是否成功
        /// </summary>
        public bool Sucessful { get; set; }
        public override bool IsMyData(ref byte[] receiveData)
        {
            if (receiveData == null || receiveData.Length < 5) return false;
            return receiveData[4] == 0xF1;
        }
        public override bool ProData(ref byte[] receiveData, out string sErr)
        {
            if(receiveData.Length!=9)
            {
                sErr = string.Format("{0}数据返回不正确：{1}", this.Title, Funs.GetByteToHex(receiveData));
                return false;
            }
            //读取是否成功
            byte b = receiveData[6];
            if(b==0x00)
            {
                if (!this.Sucessful)
                    this.Sucessful = true;
                if (this.WorkMode != 0)
                    this.WorkMode = 0;
            }
            else if (b == 0x01)
            {

                if (!this.Sucessful)
                    this.Sucessful = true;
                if (this.WorkMode != 1)
                    this.WorkMode = 1;
            }
            else if (b == 0xFF)
            {
                if (this.Sucessful)
                    this.Sucessful = false;
                if (this.WorkMode != -1)
                    this.WorkMode = -1;
            }
            else
            {
                sErr = string.Format("开始采样返回数据中采样类型[{0}]不是预期的。", b);
                return false;
            }
            sErr = string.Empty;
            return true;
        }
    }
    #endregion
    #region 停止采样
    /// <summary>
    /// 停止采样
    /// </summary>
    public class ZCQReceive_StopWorking : ZCQReceiveBase
    {
        public ZCQReceive_StopWorking()
        {
            this.Title = "停止采样";
        }
        /// <summary>
        /// 停止是否成功
        /// </summary>
        public bool Sucessful { get; set; }
        public override bool IsMyData(ref byte[] receiveData)
        {
            if (receiveData == null || receiveData.Length < 5) return false;
            return receiveData[4] == 0x21;
        }
        public override bool ProData(ref byte[] receiveData, out string sErr)
        {
            if (receiveData.Length != 9)
            {
                sErr = string.Format("{0}数据返回不正确：{1}", this.Title, Funs.GetByteToHex(receiveData));
                return false;
            }
            //读取是否成功
            byte b = receiveData[6];
            if (b == 0x00)
            {
                if (!this.Sucessful)
                    this.Sucessful = true;
            }
            else if (b == 0xFF)
            {
                if (this.Sucessful)
                    this.Sucessful = false;
            }
            else
            {
                sErr = string.Format("停止采样返回数据中采样类型[{0}]不是预期的。", b);
                return false;
            }
            sErr = string.Empty;
            return true;
        }
    }
    #endregion
    #region 工作状态查询
    /// <summary>
    /// 工作状态查询
    /// </summary>
    public class ZCQReceive_WorkStatus: ZCQReceiveBase
    {
        public ZCQReceive_WorkStatus()
        {
            this.Title = "工作状态";
        }
        /// <summary>
        /// 设备状态：-1:未定义；0空闲；1定量采样中，2：定时采样中，3：等待中，4：暂停中
        /// </summary>
        public short Working { get; set; } = -1;
        public override bool IsMyData(ref byte[] receiveData)
        {
            if (receiveData == null || receiveData.Length < 5) return false;
            return receiveData[4] == 0xC1;
        }
        public override bool ProData(ref byte[] receiveData, out string sErr)
        {
            if (receiveData.Length != 10)
            {
                sErr = string.Format("{0}数据返回不正确：{1}", this.Title, Funs.GetByteToHex(receiveData));
                return false;
            }
            //读取是否成功
            byte b = receiveData[7];
            if (b == 0x00 && b!=0x01 && b!=0x02 && b!= 0x03 && b != 0x04)
            {
                sErr = string.Format("工作状态返回数据的状态值[{0}]不是预期的。", b);
                if (this.Working != -1)
                    this.Working = -1;
                return false;
            }
            if (this.Working != b)
                this.Working = b;
            sErr = string.Empty;
            return true;
        }
    }
    #endregion
    #region 设备编号查询
    public class ZCQReceive_MacNumber : ZCQReceiveBase
    {
        public ZCQReceive_MacNumber()
        {
            this.Title = "设备编号查询";
        }
        /// <summary>
        /// 设备编号
        /// </summary>
        public string Number { get; set; } = string.Empty;
        public override bool IsMyData(ref byte[] receiveData)
        {
            if (receiveData == null || receiveData.Length < 5) return false;
            return receiveData[4] == 0xE1;
        }
        public override bool ProData(ref byte[] receiveData, out string sErr)
        {
            //设备编号应该不是一个固定数值，同样的款式的也可能编号的长度不同
            if (receiveData.Length < 2)
            {
                sErr = string.Format("{0}数据返回不正确：{1}", this.Title, Funs.GetByteToHex(receiveData));
                return false;
            }
            //读取是否成功
            short iLen = receiveData[1];
            //通过iLen推算出编号的位数，这个参照协议说明文件
            int iStart = 7;
            int iNumberLen = iLen - 3;//多出来的那3个时最后的尾帧、校验位
            if(receiveData.Length<=(iNumberLen + iStart))
            {
                sErr = string.Format("{0}数据返回不正确：{1}", this.Title, Funs.GetByteToHex(receiveData));
                return false;
            }
            StringBuilder sb = new StringBuilder();
            for(int i=0;i<iNumberLen;i++)
            {
                sb.Append((char)receiveData[iStart+i]);//设备编号为ASCII值
            }
            if (this.Number != sb.ToString())
                this.Number = sb.ToString();
            sErr = string.Empty;
            return true;
        }
    }
    #endregion
    public class Funs
    {
        public static string GetByteToHex(byte[] bs)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in bs)
            {
                string shex = Convert.ToString(b, 16);
                if (shex.Length < 2) shex = "0" + shex;
                sb.Append(string.Format("{0} ", shex));
            }
            return sb.ToString().Trim().ToUpper();
        }
        public static string GetPropertysValue(object obj)
        {
            System.Reflection.PropertyInfo[] propertys = obj.GetType().GetProperties();
            StringBuilder sb = new StringBuilder();
            foreach (System.Reflection.PropertyInfo pinfo in propertys)
            {
                sb.Append(string.Format("{0}={1}\r\n", pinfo.Name, pinfo.GetValue(obj, null)));
                //Response.Write("<br>" + pinfo.Name + "," + pinfo.GetValue(myobj, null));
            }
            return sb.ToString();
        }
    }

}
