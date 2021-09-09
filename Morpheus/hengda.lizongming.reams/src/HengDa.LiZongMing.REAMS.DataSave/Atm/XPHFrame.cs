using Aming.Core;
using HengDa.LiZongMing.REAMS.Wpf.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HengDa.LiZongMing.REAMS.Wpf.IotFrames
{
    /// <summary>
    /// 新普惠气象站
    /// </summary>
    public class XPHFrame : IFrame
    {
        public XPHFrame()
        {
            //站点默认是01
            SiteNo = 1;
        }
        /// <summary>
        /// 初始化帧对象
        /// </summary>
        /// <param name="iStationNo">当前设备站点号</param>

        #region 公共属性

        /// <summary>
        /// Modbus协议用的站点号
        /// </summary>
        public byte SiteNo { get; set; } = 0x01;

        /// <summary>
        /// Hex操作码，对应Modbus的03、04、10、20等读写操作
        /// </summary>
        public byte Operate { get; set; } = 0x03;

        /// <summary>
        /// Hex命令码
        /// </summary>
        public XPHCmdCode CommandCode { get; set; }


        /// <summary>
        /// Hex数据集合
        /// </summary>
        public List<byte> Datas { get; set; } = new List<byte>();
        ///// <summary>
        ///// CRC校验码
        ///// </summary>
        //public byte Crc { get; set; }
        /// <summary>
        /// 原始Hex
        /// </summary>
        public string RAW { get { return (Bytes != null && Bytes.Length > 0) ? Bytes.ByteToHexStr() : ""; } }

        /// <summary>
        /// 发送的完整消息
        /// </summary>
        public byte[] Bytes { get; private set; }

        /// <summary>
        /// 收到的回应消息
        /// </summary>
        public byte[] RespBytes { get; private set; }

        public string RespRAW { get { return (RespBytes != null && RespBytes.Length > 0) ? RespBytes.ByteToHexStr() : ""; } }
        /// <summary>
        /// 更新发送时的时间，用来暂时记录等待回应的命令,方便识别串口(含网络)返回的数据
        /// </summary>
        public DateTime SendTime { get; private set; }
        #endregion

        #region  解析结果
        public List<XPHBaseDto> DtoDatas { get; set; } = new List<XPHBaseDto>();
        /// <summary>
        /// 数据解析是否正确
        /// <summary>
        /// 错误消息
        /// </summary>
        public string ErrMsg { get; set; }
        #endregion



        public override string ToString()
        {
            return RAW;
        }

        #region PLC通讯帧的编码与解码
        public IFrame SetSiteNo(byte siteNo)
        {
            this.SiteNo = siteNo;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IFrame SetTime()
        {
            this.SendTime = DateTime.Now;
            return this;
        }
        public IFrame BuildFram(byte[] cmd, params byte[] datas)
        {

            /****************
            # 读气象站地址（广播）
            00 20 00 68 

            #写气象站地址命令:
            00 10 00 81 Add CRC16

            #复位气象站（没有返回内容，复位后处于XPH专用协议模式）
            01 80 01 80 

            #将气象站通讯协议切换成Modbus协议
            发送：(4字节) 00 50 01 8C
             
            #在Modbus协议读取前16个通道的值(第一个01是站点号，最后4406是校验码)
             01 03 00 00 00 10 44 06
             */


            List<byte> bs = new List<byte>();
             CommandCode = (XPHCmdCode)cmd[0];

            switch (CommandCode)
            {
                case XPHCmdCode.复位气象站:
                    this.Bytes = new byte[] { this.SiteNo, 0x80, 0x01, 0x80 };  //还不确定第3字节是不是也是站点号,没校验码等
                    return this;
                case XPHCmdCode.读气象站地址:
                    this.Bytes = StringHelper.HexStrToHexByte("00 20 00 68");
                    return this;
                case XPHCmdCode.协议切换成Modbus:
                    throw new NotImplementedException("为了安全，代码不允许修改协议，如有需要可手动修改。");
                //this.Bytes = StringHelper.HexStrToHexByte("00 50 01 8C");
                //return this;
                case XPHCmdCode.改写气象站地址: //00 10 00 81 Add CRC16 （7字节），返回：（5字节）  00 10 01 BD C0
                    throw new NotImplementedException("为了安全，代码不允许修改地址，如有需要可手动修改。");
                //Datas = new List<byte>(datas);
                //SiteNo = 00;

                case XPHCmdCode.实时环境参数数据: //01 03 00 00 F1 D8
                case XPHCmdCode.读历史数据: // 01 03 00 37 B0 0E
                case XPHCmdCode.历史数据重读: // 01 03 00 38 F0 0A
                    Datas = new List<byte>(); //没有数据部分
                    break;
                default:
                    {
                        Datas = new List<byte>(datas);
                        Console.WriteLine("还不认识该命令");
                    }
                    break;
            }
            byte len = (byte)(Datas.Count);
            var crcData = new List<byte>();
            crcData.AddRange(new byte[] { SiteNo, Operate, 0x00, (byte)CommandCode });
            if (Datas != null && Datas.Count > 0)
            {
                crcData.Add((byte)Datas.Count);
                crcData.AddRange(Datas);
            }

            var Crc = CrcTools.ModBusCRC16Bytes(crcData.ToArray());

            bs.AddRange(crcData);
            bs.AddRange(Crc);

            this.Bytes = bs.ToArray();

            return this;
        }
        public IFrame BuildFram(byte cmd, byte[] data)
        {
            return BuildFram(new byte[] { cmd }, data);
        }

        public IFrame DecodeFrame(byte[] alldata)
        {
            RespBytes = alldata;

            if (alldata == null || alldata.Length == 0 || alldata.Length <4)
            {
                this.ErrMsg = "返回结果为空或是太短！";
                return this;
            }
            //因为已经缓存了发送时的消息，所以有可以读到些老参数

            // 新普惠自动气象站通讯协议和Modbus相似但不兼容，返回的数据少量含有CommandCode但更多的没有CommandCode。
            {
                byte op = alldata[1]; //03是读操作
                #region 解析收到的数据
                if(op== 0x03) //03是读回指定地址的数据操作
                {

                    //只能从上一个发出的操作来分别是否有CommandCode段
                    switch (CommandCode)
                {

                        //case XPHCmdCode.复位气象站: 
                            //不会有返回
                        //case XPHCmdCode.协议切换成Modbus:
                            //返回：(4个字节) 00 50 01 8C
                        //case XPHCmdCode.读气象站地址:
                            //返回：（5字节） 00 20 Add CRC16
                        //case XPHCmdCode.改写气象站地址: //00 10 00 81 Add CRC16 （7字节），返回：（5字节）  00 10 01 BD C0
                            

                        case XPHCmdCode.实时环境参数数据: //01 03 00 00 F1 D8
                            {
                                int len = StringHelper.BytesToShort(alldata,2);
                                if ( alldata.Length== (len + 6))
                                {
                                    //XPHAtmosphereStruct dto =(XPHAtmosphereStruct) ByteHelper.BytesToStruct(typeof(XPHAtmosphereStruct), alldata, 4,16*2);
                                    var DtoData = new XPHBaseDto(alldata, 4, 16 * 2);
                                    DtoData.RecordTime = DateTime.Parse( DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                    DtoDatas.Add(DtoData);
                                    //Console.WriteLine("处理"+DtoData.风速);
                                }
                            }
                            break;
                        case XPHCmdCode.读历史数据: // 01 03 00 37 B0 0E
                        case XPHCmdCode.历史数据重读: // 01 03 00 38 F0 0A
                            {
                                int len = StringHelper.BytesToShort(alldata, 2); //数据长度
                                int count = StringHelper.BytesToShort(alldata, 4);    //有效数据帧数
                                if (alldata.Length == (len + 6))
                                {
                                        DtoDatas = new List<XPHBaseDto>();
                                    for (int i = 0; i < count; i++)
                                    {
                                        //数据域：固定大小为1036个字节，每帧37字节，故最大有效帧1036/37=28帧。
                                        //有效数据帧数：由于历史数据包是等长的，该域指示数据域中包含的有效帧数，有效帧从数据域的0位置开始,5字节是日期，顺序往后排列。最大有效帧为28帧。
                                        //XPHAtmosphereStruct dto = (XPHAtmosphereStruct)ByteHelper.BytesToStruct(typeof(XPHAtmosphereStruct), alldata, 6+i*37, 16);
                                        var DtoData = new XPHBaseDto(alldata, 6+5 + i * 37, 16 * 2);
                                        DtoData.RecordTime =new DateTime(2000+(int)alldata[6 +  i * 37], (int)alldata[6 + i * 37+1], (int)alldata[6 + i * 37+2], (int)alldata[6 + i * 37+3], (int)alldata[6 + i * 37+4],0);
                                        DtoDatas.Add(DtoData);
                                        //Console.WriteLine("处理" + DtoData.风速);

                                    }
                                    
                                }
                            }
                            break;
                        default:
                            {
                                throw new NotImplementedException("还不认识该命令。");
                            }
                            //break;
                    }
                }
                #endregion
            }
           
            return this;
        }
      
        #endregion


    }


    public enum XPHCmdCode : byte
    {

        //0x00	实时环境参数数据    通用类型    仅读
        //0x20	系统参数配置  通用类型    读写
        //0x37	读历史数据   通用类型    仅读
        //0x38	历史数据重读  通用类型    仅读
        //0x7A	手动控制命令      仅写
        //0x80	复位气象站   通用类型    仅写

        实时环境参数数据 = 0x00,
        系统参数配置 = 0x20,  
        读历史数据 = 0x37,
        历史数据重读 = 0x38, 
        手动控制命令 = 0x7A,
        复位气象站 = 0x80,

        //自己加几个，手册上没写的
        读气象站地址,
        改写气象站地址,
        协议切换成Modbus,
    }



    /// <summary>
    /// 2000年开始的时间 
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct MDateTimeStruct
    {

//1	年(2000年以后的偏移)
//1	月
//1	日
//1	时(24小时制)
//1	分钟

        [MarshalAs(UnmanagedType.U1)]
        ushort 年;
        [MarshalAs(UnmanagedType.U1)]
        ushort 月;
        [MarshalAs(UnmanagedType.U1)]
        ushort 日;
        [MarshalAs(UnmanagedType.U1)]
        ushort 时;
        [MarshalAs(UnmanagedType.U1)]
        ushort 分钟;

    };
}
