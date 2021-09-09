
using Aming.Core;
using HengDa.LiZongMing.REAMS.Wpf.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace HengDa.LiZongMing.REAMS
{
    /// <summary>
    /// PLC帧的结构
    /// </summary>
    public class HDZCQFrame : IFrame
    {
        //private static NLog.ILogger _logger = LogService.Logger();
        #region 字段
        /// <summary>
        /// Hex从站地址
        /// </summary>
        public byte SiteAdd { get; set; } = 0x05;
        /// <summary>
        /// Hex命令码
        /// </summary>
        public HDZCQCmdCode CommandCode { get; set; }
        ///// <summary>
        ///// Hex起始数据地址（寄存器地址）
        ///// </summary>
        //public string BeginDataAddress { get; set; }
        ///// <summary>
        ///// Hex数据条数
        ///// </summary>
        //public string DataNum { get; set; }
        ///// <summary>
        ///// Hex数据字节数
        ///// </summary>
        //public string DataBytesNum { get; set; }
        /// <summary>
        /// Hex数据集合
        /// </summary>
        public List<byte> Datas { get; set; } = new List<byte>();
        /// <summary>
        /// CRC校验码
        /// </summary>
        public byte Crc { get; set; }
        /// <summary>
        /// 原始Hex
        /// </summary>
        public string RAW { get; set; }
        public byte[] Bytes { get; private set; }


        //        public 故障{
        //            舱门状态
        //LCD连接失败
        //大气B超限 
        //大气A超限 
        //气碘超限
        //B电子流量计FS4003故障

        //}

        #endregion 字段
        #region  解析结果
        public ZCQBaseDto DtoData{ get; set; }
        /// <summary>
        /// 数据解析是否正确
        /// <summary>
        /// 错误消息
        /// </summary>
        public string ErrMsg { get; set; }
        #endregion
        public override string ToString()
        {
            //if (string.IsNullOrEmpty(RAW))
            //{
            //    GetBytes();
            //}
            return RAW;
        }

        #region PLC通讯帧的编码与解码
        public IFrame BuildFram(byte[] cmd, params byte[] datas)
        {
            /*
             工作状态查询0xC0
            3.1   发送68 03 03 68 C0 05 00 CH 0D
                实际计算后，内容带校验码是：  68 03 03 68 C0 05 00 C5 0D
            3.2   返回68 04 04 68 C1 05 00 xx CH 0D
                  说明：红色部分xx为机器工作的状态，其中包括：空闲00、定量采样01、定时采样02、等待中03、暂停中04,此例为空闲状态。
             * */

            List<byte> bs= new List<byte>();
            this.CommandCode = (HDZCQCmdCode)cmd[0];
            byte site = 0x05;
            switch (CommandCode)
            {
                case HDZCQCmdCode.文件号查询:
                case HDZCQCmdCode.工作状态查询:
        case HDZCQCmdCode.设备编号查询:
                case HDZCQCmdCode.停止采样:
                case HDZCQCmdCode.故障查询:
                case HDZCQCmdCode.测试命令:
                case HDZCQCmdCode.设置信息查询:
                    {
                        Datas = new List<byte> { 0x00 };
                    }
                    break;
                case HDZCQCmdCode.瞬时数据查询:
                    {
                        //Datas = new List<byte> { 0x01<<7 & 0x01 << 6 & 0x01 << 3 & 0x01 << 1 };
                        Datas = new List<byte> { (0x01 << 7) | (0x01 << 6) | (0x01 << 3) | (0x01 << 1) };//修改不用jps，这里只取目前有的项目
                    }
                    break;
                case HDZCQCmdCode.开始采样:
                case HDZCQCmdCode.读取文件:
                case HDZCQCmdCode.采样瞬时流量设定:
                case HDZCQCmdCode.采样时间设定:
                case HDZCQCmdCode.时间设置:
                case HDZCQCmdCode.滤膜编号设定:
                case HDZCQCmdCode.定量采样量设定:
                case HDZCQCmdCode.采样次数:
                case HDZCQCmdCode.定时采样间隔:
                case HDZCQCmdCode.定时启动时间:
                case HDZCQCmdCode.定时启动使能:
                    {
                        Datas = new List<byte>(datas);
                    }
                    break;
                default:
                    {
                        Datas =new List<byte>( datas);
                        Console.WriteLine("还不认识该命令");
                    }
                    break;
            }
            byte len = (byte)(1 + 1 + Datas.Count);
            bs.AddRange(new byte[] { 0x68, len, len, 0x68 });

            var crcData = new List<byte>(){ (byte)CommandCode, site };
            crcData.AddRange(Datas);
            Crc = CrcTools.CrcAdd8Low(crcData.ToArray());

            bs.AddRange(crcData);
            bs.Add(Crc);
            bs.Add(0x0D);

            this.Bytes = bs.ToArray();
            this.RAW = this.Bytes.ByteToHexStr();

            return this;
        }
        public IFrame BuildFram( byte cmd, byte[] data)
        {
            return BuildFram(new byte[] { cmd }, data);
        }

        public IFrame DecodeFrame(byte[] alldata)
        {
            if (Validate(alldata))
            {
                #region 解析收到的数据
                byte bFN = alldata[4];
                bFN -= 0x01;//根据通信协议文档可以得出一个规律：返回内容的功能是在发送内容功能码上+1；但是有少量几个特殊
                HDZCQCmdCode cFn = (HDZCQCmdCode)bFN;//转换成对应的枚举值
                switch (cFn)
                {

                    case HDZCQCmdCode.文件号查询:
                        DtoData = new ZCQFileNoDto(alldata, cFn);
                        break;
                    case HDZCQCmdCode.读取文件:
                        DtoData = new ZCQFileInfoDto(alldata, cFn);
                        break;
                    case HDZCQCmdCode.工作状态查询:
                        DtoData = new ZCQWorkStatusDto(alldata, cFn);
                        break;
                    case HDZCQCmdCode.瞬时数据查询:
                        DtoData = new ZCQRealDataDto(alldata, cFn);
                        break;
                    case HDZCQCmdCode.设备编号查询:
                        DtoData = new ZCQMacNumberDto(alldata, cFn);
                        break;
                    case HDZCQCmdCode.开始采样:
                        DtoData = new ZCQStartWorkingDto(alldata, cFn);
                        break;
                    case HDZCQCmdCode.停止采样:
                        DtoData = new ZCQStopWorkingDto(alldata, cFn);
                        break;
                    case HDZCQCmdCode.故障查询:
                        DtoData = new ZCQMacAlermDto(alldata, cFn);
                        break;
                    case HDZCQCmdCode.采样瞬时流量设定:
                        DtoData = new ZCQSampllingFlowRateDto(alldata, cFn);
                        break;
                    case HDZCQCmdCode.采样时间设定:
                        DtoData = new ZCQSampllingTimeLongSetDto(alldata, cFn);
                        break;
                    case HDZCQCmdCode.定量采样量设定:
                        DtoData = new ZCQSamplingVSetDto(alldata, cFn);
                        break;
                    case HDZCQCmdCode.时间设置:
                        DtoData = new ZCQTimeSetDto(alldata, cFn);
                        break;
                    case HDZCQCmdCode.测试命令:
                        DtoData = new ZCQCommunicationCheckDto(alldata, cFn);
                        break;
                    case HDZCQCmdCode.设置信息查询:
                        DtoData = new ZCQSettedInfoDto(alldata, cFn);
                        break;
                    case HDZCQCmdCode.滤膜编号设定:
                        DtoData = new ZCQLvMoIDSetDto(alldata, cFn);
                        break;
                    case HDZCQCmdCode.采样次数:
                        DtoData = new ZCQSampllingCountSetDto(alldata, cFn);
                        break;
                    case HDZCQCmdCode.定时采样间隔:
                        DtoData = new ZCQSampllingIntervalSetDto(alldata, cFn);
                        break;
                    case HDZCQCmdCode.定时启动时间:
                        DtoData = new ZCQAutoStartTimeSetDto(alldata, cFn);
                        break;
                    case HDZCQCmdCode.定时启动使能:
                        DtoData = new ZCQAutoStartSetDto(alldata, cFn);
                        break;
                    default:
                        {
                            DtoData = new ZCQBaseDto();
                            DtoData.Cmd = cFn;
                            DtoData.ErrMsg = "反馈数据FN码[" + StringHelper.ByteToHexStr(new byte[] { alldata[4] }) + "]未解析";
                            DtoData.Sucessfully = false;
                        }
                        break;
                }
                #endregion
            }
            else
            {
                DtoData = new ZCQBaseDto();
                DtoData.ErrMsg = this.ErrMsg;//存储帧头校验时的错误
                DtoData.Sucessfully = false;
            }
            return this;
        }
        public bool Validate(byte[] alldata)
        {
            //TODO: JPS改写校验过程
            //校验数据长度是否符合
            if (alldata == null || alldata.Length == 0)
            {
                this.ErrMsg = "返回结果为空！";
                return false;
            }
            //返回的内容应该为：68 N N 68 .....，总长度应该为N+6；
            if (alldata.Length <= 4)
            {
                this.ErrMsg = string.Format("返回的数据不正确：{0}", StringHelper.ByteToHexStr(alldata));
                return false;
            }
            int iLen = (int)alldata[1];//用于读取返回的数据FN+DT+FNC部分的长度
            iLen += 6;//该设备的通讯协议规则是返回的总长度为FN+DT+FNC+6；发送和返回的字节规则都一样
            if (alldata.Length != iLen)
            {
                this.ErrMsg = string.Format("返回的字节长度不是预期的{0}位：{1}", iLen, StringHelper.ByteToHexStr(alldata));
                return false;
            }
            if (alldata[0] != 0x68 || alldata[3] != 0x68)
            {
                this.ErrMsg = string.Format("返回的数据缺少帧头：{1}", iLen, StringHelper.ByteToHexStr(alldata));
                return false;
            }
            return true;
        }
        #endregion

    }
}
