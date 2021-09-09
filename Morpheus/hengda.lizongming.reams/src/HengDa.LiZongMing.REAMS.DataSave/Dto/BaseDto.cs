using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HengDa.LiZongMing.REAMS.Wpf.Dto
{
    [AddINotifyPropertyChangedInterface]
    public class BaseDto : IMacDataBaseDto
    {
        /// <summary>
        /// 原始数据
        /// </summary>
        public byte[] Raw { get; set; }
        /// <summary>
        /// 设备站点号,该地址是MODBUS通讯时的第一个字节：设备站点号
        /// </summary>
        [Description("设备序号")]
        public ushort MacNo { get; set; }
        /// <summary>
        /// 命令值描述
        /// </summary>
        [Description("命令值描述")]
        public string Title { get; set; }
        /// <summary>
        /// 数据解析时的错误信息
        /// </summary>
        [Description("错误消息")]
        public string ErrMsg { get; set; }
        /// <summary>
        /// 数据解析是否成功
        /// </summary>
        [Description("是否解析成功")]
        public bool Sucessfully { get; set; } = true;
    }
}
