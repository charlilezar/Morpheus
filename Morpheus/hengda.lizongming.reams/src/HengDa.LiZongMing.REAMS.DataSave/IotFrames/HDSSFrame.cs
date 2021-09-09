using Aming.Core;
using HengDa.LiZongMing.REAMS.Wpf.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HengDa.LiZongMing.REAMS.Wpf.IotFrames
{
    public class HDSSFrame : IFrame
    {
        public HDSSFrame()
        {
            //站点默认是01
            MacNo = 1;
        }
        /// <summary>
        /// 初始化帧对象
        /// </summary>
        /// <param name="iStationNo">当前设备站点号</param>
        public HDSSFrame(ushort iStationNo)
        {
            MacNo = iStationNo;
        }
        #region 公共属性
        public HDSSCmdCode CommandCode { get; set; }
        public ushort MacNo { get; set; }
        public string RAW { get; set; }
        /// <summary>
        /// 异常信息
        /// </summary>
        public string ErrMsg { get; set; }
        public Dto.SSBaseDto DtoData { get;set;}
        #endregion
        public override string ToString()
        {
            return RAW;
        }
        
        public byte[] Bytes { get; private set; }
        public bool Validate(byte[] alldata)
        {
            /******
             *  03H功能码的返回数据格式//1+1+1+2*N+2=7+2*N个字节；
             *  
             * ******************/
            if (alldata == null || alldata.Length == 0)
            {
                this.ErrMsg = $"返回结果为空！";
                return false;
            }
            if (alldata.Length <=2)
            {
                this.ErrMsg = $"返回结果长度不正确{StringHelper.ByteToHexStr(alldata)}";
                return false;
            }
            if(alldata[1]==0x03)
            {
                #region 此时功能码是请求寄存器数据
                //则第四位是字节长度码
                short iLen = (short)alldata[2];
                if ((alldata.Length - 5) != iLen)
                {
                    this.ErrMsg = $"03H功能码返回结果长度不是预期的{iLen}位：{StringHelper.ByteToHexStr(alldata)}。";
                    return false;
                }
                #endregion
            }
            //校验码判断是否正确
            byte[] bsCrc = BitConverter.GetBytes(this.BuildFram_Crc16(alldata, alldata.Length - 2));
            if (bsCrc[0]!= alldata[alldata.Length-2] || bsCrc[1]!= alldata[alldata.Length - 2])
            {
                //此时校验码不正确
                this.ErrMsg = $"返回数据的校验码不是预期的[{StringHelper.ByteToHexStr(bsCrc)}]：{StringHelper.ByteToHexStr(alldata)}。";
                return false;
            }
            return true;
        }
        public IFrame BuildFram(byte cmd, byte[] data)
        {
            return BuildFram(new byte[] { cmd }, data);
        }

        public List<byte> Datas { get; set; } = new List<byte>();
        public byte Crc { get; set; }
        public IFrame BuildFram(byte[] cmd, params byte[] datas)
        {
            /*
            工作状态查询0xC0
           3.1   发送68 03 03 68 C0 05 00 CH 0D
               实际计算后，内容带校验码是：  68 03 03 68 C0 05 00 C5 0D
           3.2   返回68 04 04 68 C1 05 00 xx CH 0D
                 说明：红色部分xx为机器工作的状态，其中包括：空闲00、定量采样01、定时采样02、等待中03、暂停中04,此例为空闲状态。
            * */

            List<byte> bs = new List<byte>();
            this.CommandCode = (HDSSCmdCode)cmd[0];
            byte site = 0x05;
            switch (CommandCode)
            {
                case HDSSCmdCode.文件号查询:
                case HDSSCmdCode.工作状态查询:
                case HDSSCmdCode.设备编号查询:
                case HDSSCmdCode.停止采样:
                case HDSSCmdCode.故障查询:
                case HDSSCmdCode.测试命令:
                case HDSSCmdCode.设置信息查询:
                    {
                        Datas = new List<byte> { 0x00 };
                    }
                    break;
                case HDSSCmdCode.瞬时数据查询:
                    {
                        //Datas = new List<byte> { 0x01<<7 & 0x01 << 6 & 0x01 << 3 & 0x01 << 1 };
                        Datas = new List<byte> { (0x01 << 7) | (0x01 << 6) | (0x01 << 3) | (0x01 << 1) };//修改不用jps，这里只取目前有的项目
                    }
                    break;
                case HDSSCmdCode.开始采样:
                case HDSSCmdCode.读取文件:
                case HDSSCmdCode.采样瞬时流量设定:
                case HDSSCmdCode.采样时间设定:
                case HDSSCmdCode.时间设置:
                case HDSSCmdCode.滤膜编号设定:
                case HDSSCmdCode.定量采样量设定:
                case HDSSCmdCode.采样次数:
                case HDSSCmdCode.定时采样间隔:
                case HDSSCmdCode.定时启动时间:
                case HDSSCmdCode.定时启动使能:
                    {
                        Datas = new List<byte>(datas);
                    }
                    break;
                default:
                    {
                        Datas = new List<byte>(datas);
                        Console.WriteLine("还不认识该命令");
                    }
                    break;
            }
            byte len = (byte)(1 + 1 + Datas.Count);
            bs.AddRange(new byte[] { 0x68, len, len, 0x68 });

            var crcData = new List<byte>() { (byte)CommandCode, site };
            crcData.AddRange(Datas);
            Crc = CrcTools.CrcAdd8Low(crcData.ToArray());

            bs.AddRange(crcData);
            bs.Add(Crc);
            bs.Add(0x0D);

            this.Bytes = bs.ToArray();
            this.RAW = this.Bytes.ByteToHexStr();

            return this;
        }
        public IFrame BuildFram(ushort iMacNo,byte cmd, byte[] data)
        {
            //添加设备编号，因为可能涉及多台干湿沉降仪同时连接；
            this.MacNo = iMacNo;
            return BuildFram(new byte[] { cmd }, data);
        }

        public IFrame DecodeFrame(byte[] alldata)
        {
            if (Validate(alldata))
            {
                #region 解析收到的数据
                byte bFN = alldata[4];
                bFN -= 0x01;//根据通信协议文档可以得出一个规律：返回内容的功能是在发送内容功能码上+1；但是有少量几个特殊
                HDSSCmdCode cFn = (HDSSCmdCode)bFN;//转换成对应的枚举值
                switch (cFn)
                {
                    case HDSSCmdCode.读取文件:
                        this.DtoData = new SSFileInfoDto(alldata, cFn);
                        break;
                    case HDSSCmdCode.文件号查询:
                    case HDSSCmdCode.工作状态查询:
                       
                    case HDSSCmdCode.瞬时数据查询:
                       
                    case HDSSCmdCode.设备编号查询:
                       
                    case HDSSCmdCode.开始采样:
                       
                    case HDSSCmdCode.停止采样:
                       
                    case HDSSCmdCode.故障查询:
                       
                    case HDSSCmdCode.采样瞬时流量设定:
                       
                    case HDSSCmdCode.采样时间设定:
                       
                    case HDSSCmdCode.定量采样量设定:
                       
                    case HDSSCmdCode.时间设置:
                       
                    case HDSSCmdCode.测试命令:
                       
                    case HDSSCmdCode.设置信息查询:
                       
                    case HDSSCmdCode.滤膜编号设定:
                       
                    case HDSSCmdCode.采样次数:
                       
                    case HDSSCmdCode.定时采样间隔:
                       
                    case HDSSCmdCode.定时启动时间:
                       
                    case HDSSCmdCode.定时启动使能:
                        {
                            DtoData = new SSBaseDto();
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
                DtoData = new SSBaseDto();
                DtoData.ErrMsg = this.ErrMsg;//存储帧头校验时的错误
                DtoData.Sucessfully = false;
            }
            return this;
        }
        #region 根据传入地址或命令
        /// <summary>
        /// 定义读取命令的bytes
        /// </summary>
        /// <param name="iStartAdr">起始地址</param>
        /// <param name="iAdrCnt">地址数量</param>
        /// <returns></returns>
        private byte[] BuildFram_GetReadBytes(ushort iStartAdr, ushort iAdrCnt)
        {
            byte[] SendBytes = new byte[8];
            byte[] bsStartAdr = BitConverter.GetBytes(iStartAdr);//起始地址
            byte[] bsCount = BitConverter.GetBytes(iAdrCnt);//地址数量
            SendBytes[0] = Convert.ToByte(this.MacNo);//站点号
            SendBytes[1] = 0x03;
            SendBytes[2] = bsStartAdr[1];
            SendBytes[3] = bsStartAdr[0];
            SendBytes[4] = bsCount[1];
            SendBytes[5] = bsCount[0];
            byte[] bsCrc = BitConverter.GetBytes(this.BuildFram_Crc16(SendBytes, 6));
            SendBytes[6] = bsCrc[0];
            SendBytes[7] = bsCrc[1];
            return SendBytes;
        }

        private byte[] BuildFram_GetWriteMultiBytes(ushort iStartAdr, ushort iAdrCnt, byte[] datas)
        {
            byte[] SendBytes = new byte[9 + iAdrCnt * 2];
            byte[] bsStartAdr = BitConverter.GetBytes(iStartAdr);//起始地址
            byte[] bsCount = BitConverter.GetBytes(iAdrCnt);//地址数量
            SendBytes[0] = Convert.ToByte(this.MacNo);//站点号
            SendBytes[1] = 0x10;
            SendBytes[2] = bsStartAdr[1];//起始地址
            SendBytes[3] = bsStartAdr[0];
            SendBytes[4] = bsCount[1];//地址数量
            SendBytes[5] = bsCount[0];
            SendBytes[6] = BitConverter.GetBytes(iAdrCnt * 2)[0];// 传入总字节数量
            for (int i = 0; i < iAdrCnt * 2; i++)
            {
                //填充数据，注意传入的datas长度必须是iAdrCnt值的2倍；
                if (datas.Length <= i)
                    SendBytes[i + 7] = 0x00;
                else
                    SendBytes[i + 7] = datas[i];
            }
            byte[] bsCrc = BitConverter.GetBytes(this.BuildFram_Crc16(SendBytes, SendBytes.Length - 2));//获取校验码
            SendBytes[SendBytes.Length - 2] = bsCrc[0];
            SendBytes[SendBytes.Length - 1] = bsCrc[1];
            return SendBytes;
        }
        /// <summary>
        /// 获取Crc16校验码
        /// </summary>
        /// <param name="modbusdata">纳入计算的字节</param>
        /// <param name="iLength">Length为实际需要纳入计算的字节的长度</param>
        /// <returns></returns>
        public short BuildFram_Crc16(byte[] modbusdata, int iLength)//Length为modbusdata的长度
        {
            //CRC(Modbus)校验码生成
            uint i, j;
            uint crc16 = 0xFFFF;
            for (i = 0; i < iLength; i++)
            {
                crc16 ^= modbusdata[i]; // CRC = BYTE xor CRC
                for (j = 0; j < 8; j++)
                {
                    if ((crc16 & 0x01) == 1) //如果CRC最后一位为1右移一位后carry=1则将CRC右移
                                             //一位后再与POLY16=0xA001进行xor运算
                        crc16 = (crc16 >> 1) ^ 0xA001;
                    else //如果CRC最后一位为0则只将CRC右移一位
                        crc16 = crc16 >> 1;
                }
            }
            return (short)crc16;
        }

       
        #endregion
    }
}
