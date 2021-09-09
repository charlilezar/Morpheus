using Aming.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HengDa.LiZongMing.REAMS.Wpf.IotFrames
{
    public class HDRFrame : IFrame
    {
        public HDRFrame()
        {
            //站点默认是01
            MacNo = 1;
        }
        /// <summary>
        /// 初始化帧对象
        /// </summary>
        /// <param name="iStationNo">当前设备站点号</param>
        public HDRFrame(ushort iStationNo)
        {
            MacNo = iStationNo;
        }
        #region 公共属性
        public HDRCmdCode CommandCode { get; set; }
        public ushort MacNo { get; set; }
        public string RAW { get; set; }
        /// <summary>
        /// 异常信息
        /// </summary>
        public string ErrMsg { get; set; }
        public Dto.ZJCBaseDto DtoData { get;set;}
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
        public IFrame BuildFram(byte[] cmd, params byte[] datas)
        {
            List<byte> bs = new List<byte>();
            this.CommandCode = (HDRCmdCode)cmd[0];
            switch (CommandCode)
            {
                case HDRCmdCode.查询数据:
                    this.Bytes = this.BuildFram_GetReadBytes(2, 4);//一次性读取所有数据，除了第一个地址外，第一个地址是存储设备站点号的
                    break;
                default:
                    Console.WriteLine($"还不认识该命令{this.CommandCode}");
                    break;
            }
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
            /***********************
             * 解析命令：
             * 1、由于MODBUS RTU协议中返回的数据无法识别出当前命令是什么的，因为反馈的数据只包含功能码和数据内容，没有起始地址什么的，所以这里只能通过
             * 类HDRFrame中的CommandCode字段来分辨究竟是哪个命令发送后反馈的数据；
             * **************************/
            switch(this.CommandCode)
            {
                //case HDRCmdCode.查询数据:
                //    this.DtoData = new Dto.hdr(alldata, this.CommandCode,this.MacNo);
                //    break;
                //default:
                //    {
                //        DtoData = new Dto.ZJCBaseDto();
                //        DtoData.Cmd = this.CommandCode;
                //        DtoData.MacNo = this.MacNo;
                //        DtoData.ErrMsg = "命令[" + EnumHelper.GetEnumDescription<HDRCmdCode>(this.CommandCode) + "]未解析";
                //        DtoData.Sucessfully = false;
                //    }
                //    break;
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
