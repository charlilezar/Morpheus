
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
    public class HDNBSFrame : IFrame
    {
        public HDNBSFrame(byte bSiteAdd)
        {
            this.SiteAdd = bSiteAdd;
        }
        //private static NLog.ILogger _logger = LogService.Logger();
        #region 字段
        /// <summary>
        /// Hex从站地址
        /// </summary>
        public byte SiteAdd { get; set; } = 0x05;
        /// <summary>
        /// Hex命令码
        /// </summary>
        public HDNBSCmdCode CommandCode { get; set; }
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
        public NBSBaseDto DtoData{ get; set; }
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
            this.CommandCode = (HDNBSCmdCode)cmd[0];
            byte site = this.SiteAdd;//设备地址
            switch (CommandCode)
            {
                case HDNBSCmdCode.运行状态查询://8
                case HDNBSCmdCode.设定值查询://9
                case HDNBSCmdCode.NBS剩余时间://10
                case HDNBSCmdCode.NBS剩余体积://11
                case HDNBSCmdCode.NBS运行模式查询://12
                case HDNBSCmdCode.NBS运行开始时间://13
                case HDNBSCmdCode.文件号查询:
                    {
                        Datas = new List<byte> { 0x00 };
                    }
                    break;
                case HDNBSCmdCode.瞬时数据查询://14
                    {
                        Datas = new List<byte> { (0x01 << 7) | (0x01 << 6) | (0x01 << 3) | (0x01 << 2) | (0x01 << 1) | 0x01 };//这里只取目前有的项目
                    }
                    break;
                case HDNBSCmdCode.采样启停控制://1
                case HDNBSCmdCode.时间设置://2
                case HDNBSCmdCode.NBS运行模式设置://3
                case HDNBSCmdCode.滤膜编号设定://4
                case HDNBSCmdCode.采样流量设定://5
                case HDNBSCmdCode.采样时长设定://6
                case HDNBSCmdCode.采样体积设定://7
                
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
                bFN -= 0x01;//根据通信协议文档可以得出一个规律：返回内容的功能是在发送内容功能码上+1；
                HDNBSCmdCode cFn = (HDNBSCmdCode)bFN;//转换成对应的枚举值
                switch (cFn)
                {

                    case HDNBSCmdCode.文件号查询:
                        DtoData = new NBSFileNoDto(alldata, cFn);
                        break;
                    case HDNBSCmdCode.读取文件:
                        DtoData = new NBSFileInfoDto(alldata, cFn);
                        break;
                    case HDNBSCmdCode.运行状态查询:
                        DtoData = new NBSWorkStatusDto(alldata, cFn);
                        break;
                    case HDNBSCmdCode.瞬时数据查询:
                        DtoData = new NBSRealDataDto(alldata, cFn);
                        break;
                    case HDNBSCmdCode.采样流量设定:
                        DtoData = new NBSSampllingFlowRateDto(alldata, cFn);
                        break;
                    case HDNBSCmdCode.采样时长设定:
                        DtoData = new NBSSampllingTimeLongSetDto(alldata, cFn);
                        break;
                    case HDNBSCmdCode.采样体积设定:
                        DtoData = new NBSSamplingVSetDto(alldata, cFn);
                        break;
                    case HDNBSCmdCode.时间设置:
                        DtoData = new NBSTimeSetDto(alldata, cFn);
                        break;
                    case HDNBSCmdCode.设定值查询:
                        DtoData = new NBSSettedInfotDto(alldata, cFn);
                        break;
                    case HDNBSCmdCode.滤膜编号设定:
                        DtoData = new NBSLvMoIDSetDto(alldata, cFn);
                        break;
                    default:
                        {
                            DtoData = new NBSBaseDto();
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
                DtoData = new NBSBaseDto();
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
