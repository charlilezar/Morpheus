using Aming.Core;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HengDa.LiZongMing.REAMS.Wpf.Dto
{
    public class RNSBaseDto:BaseDto
    {
        /// <summary>
        /// 命令值
        /// </summary>
        [Description("命令值")]
        public HDRNSCmdCode Cmd { get; set; }
    }
    /// <summary>
    /// 感雨器DTO
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class RNSRealDataDto:RNSBaseDto
    {
        public RNSRealDataDto()
        {

        }
        public RNSRealDataDto(byte[] data, HDRNSCmdCode cmd, ushort iMacNo)
        {
            /***********
            * 解析数据，返回的数据长度为5+2*8（请求86个寄存器地址）=21
            * 第3个字节表示返回有效数据长度=2*8=16个字节；
            * ******************/
            this.MacNo = iMacNo;
            this.Cmd = cmd;
            if (data.Length == 21 && 0x03==data[1] && 16 == data[2])
            {
                #region 正确数据解析
                int iIndex = 3;
                this.Raining = StringHelper.BytesToUShort(new byte[] { data[iIndex++], data[iIndex++] }, 0) == 1;//03H寄存器地址存储状态，1位下雨，0位不下雨，数据类型为ushort
                iIndex += 2;//寄存器地址04H无内容
                this.RNSTemp = StringHelper.BytesToUShort(new byte[] { data[iIndex++], data[iIndex++] }, 0)/10M;//用05H寄存器存储温度，数据需要除以10才是实际温度，数据类型为ushort
                iIndex += 2;//寄存器地址06H无内容
                //用07H、08H、09H、0AH这4个寄存器地址存储雨量，数据类型为double
                this.RNSRainfall= (decimal)StringHelper.BytesToDouble(new byte[] { data[iIndex++], data[iIndex++], data[iIndex++], 
                    data[iIndex++], data[iIndex++], data[iIndex++], data[iIndex++], data[iIndex++] }, true);
                #endregion
                this.Sucessfully = true;
            }
            else if (data.Length == 5 && 0x83 == data[1])
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
        /// 下雨中
        /// </summary>
        [Description("是否下雨[true下雨，false不下雨]")]
        public bool Raining { get; set; }
        /// <summary>
        /// 雨感器温度
        /// </summary>
        [Description("感雨器温度[℃]")]
        public decimal RNSTemp { get; set; }
        /// <summary>
        /// 雨量
        /// </summary>
        [Description("雨量[单位]")]
        public decimal RNSRainfall { get; set; }
        #endregion
    }
    public class RNSRainingTemSetDto : RNSBaseDto
    {
        public RNSRainingTemSetDto()
        {

        }
        public RNSRainingTemSetDto(byte[] data, HDRNSCmdCode cmd, ushort iMacNo)
        {
            /**********
             * 设置单个寄存器地址，地址为0EH：
             * 正确的话返回：
             * 地址码（1byte）	功能码（1byte）	寄存器地址（2byte）	寄存器数据（2byte）	CRC（2byte），所以总共返回8个字节；
             * 如果错误的话，根据MODBUSRTU协议，会返回5个字节，但这个《RNS感雨器通信协议.docx》上未写明，只写了正确返回格式；
             * **********/
            this.MacNo = iMacNo;
            this.Cmd = cmd;
            if (data.Length == 8)
            {
                if (data[1] != 0x06)
                {
                    this.ErrMsg = $"设置感雨器下雨时加温阀值时反馈的功能码[{data[1]}]不是预期的6";
                    this.Sucessfully = false;
                    return;
                }
                else if (data[2] != 0x00 && data[3] != 0x0E && data[4] != 0x00 && data[5] != 0x01)
                {
                    //此时判断读取反馈的寄存地址是否是我们写入的地址
                    this.ErrMsg = $"设置感雨器下雨时加温阀值时反馈的设置地址不是预期的地址，数据：{StringHelper.ByteToHexStr(data)}";
                    this.Sucessfully = false;
                    return;
                }
            }
            else if (data.Length == 5 && data[1] == 0x86)
            {
                //此时设备反馈出错
                //此时返回错误消息
                this.ErrMsg = $"写入感雨器下雨时加温阀值时，设备反馈错误，代码[{data[2]}]";
                this.Sucessfully = false;
                return;
            }
            else
            {
                this.ErrMsg = $"写入感雨器下雨时加温阀值时，返回的数据不是预期的[{StringHelper.ByteToHexStr(data)}]";
                this.Sucessfully = false;
                return;
            }
            this.Sucessfully = true;
        }
    }
    public class RNSUnRainTemSetDto : RNSBaseDto
    {
        public RNSUnRainTemSetDto()
        {

        }
        public RNSUnRainTemSetDto(byte[] data, HDRNSCmdCode cmd, ushort iMacNo)
        {
            /**********
             * 设置单个寄存器地址，地址为10H：
             * 正确的话返回：
             * 地址码（1byte）	功能码（1byte）	寄存器地址（2byte）	寄存器数据（2byte）	CRC（2byte），所以总共返回8个字节；
             * 如果错误的话，根据MODBUSRTU协议，会返回5个字节，但这个《RNS感雨器通信协议.docx》上未写明，只写了正确返回格式；
             * **********/
            this.MacNo = iMacNo;
            this.Cmd = cmd;
            if (data.Length == 8)
            {
                if (data[1] != 0x06)
                {
                    this.ErrMsg = $"设置感雨器不下雨时加温阀值时反馈的功能码[{data[1]}]不是预期的6";
                    this.Sucessfully = false;
                    return;
                }
                else if (data[2] != 0x00 && data[3] != 0x10 && data[4] != 0x00 && data[5] != 0x01)
                {
                    //此时判断读取反馈的寄存地址是否是我们写入的地址
                    this.ErrMsg = $"设置感雨器不下雨时加温阀值时反馈的设置地址不是预期的地址，数据：{StringHelper.ByteToHexStr(data)}";
                    this.Sucessfully = false;
                    return;
                }
            }
            else if (data.Length == 5 && data[1] == 0x86)
            {
                //此时设备反馈出错
                //此时返回错误消息
                this.ErrMsg = $"写入感雨器不下雨时加温阀值时，设备反馈错误，代码[{data[2]}]";
                this.Sucessfully = false;
                return;
            }
            else
            {
                this.ErrMsg = $"写入感雨器不下雨时加温阀值时，返回的数据不是预期的[{StringHelper.ByteToHexStr(data)}]";
                this.Sucessfully = false;
                return;
            }
            this.Sucessfully = true;
        }
    }
}
