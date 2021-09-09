using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HengDa.LiZongMing.REAMS
{
    public enum HDNaISCmdCode : byte
    {
        文件号查询 = 0xA0,
        读取剂量率 = 0x01,  //
        工作状态查询 = 0xC0,
        瞬时数据查询 = 0xD0, //
        设备编号查询 = 0xE0,
        开始采样 = 0xF0, //分定时还是定量
        停止采样 = 0x20,
        采样瞬时流量设定 = 0x30,//
        采样时间设定 = 0x40,//

        时间设置 = 0x60,//
        故障查询 = 0x70,//
        测试命令 = 0xFA,
        设置信息查询 = 0x80,
        滤膜编号设定 = 0x64,

        定量采样量设定 = 0x50,//
        采样次数 = 0x51,
        定时采样间隔 = 0x52,
        定时启动时间 = 0x53,
        定时启动使能 = 0x54,

    }
}
