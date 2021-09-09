using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HengDa.LiZongMing.REAMS
{
    public enum HDRNSCmdCode : byte
    {
        /***************
         * 目前直接根据界面显示需求，定义采集命令
         * ************/
        实时数据=0x01,
        设置下雨加热温度阀值=0x2,
        设置不下雨加热温度阀值 = 0x3,
        设置下雨及不下雨加热温度阀值 = 0x4,
    }
}
