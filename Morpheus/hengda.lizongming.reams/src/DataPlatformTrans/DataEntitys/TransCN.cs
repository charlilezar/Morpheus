using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataPlatformTrans.DataEntitys
{
    public enum TransCNs
    {
        设置监测站时间 = 1022,
        实时数据上传 = 2011,
        停止察看实时数据 = 2012,
        上传大流量采样实时数据 =2014,
        取指定监测项给定时间的历史数据 = 2042,
        设置采样周期 = 3105,
        身份验证 =6011,
        完全初始化命令 = 6021,
        网络心跳包 = 6031,
        反馈应答 = 9011,
        反馈操作执行结果 = 9012,
    }
}
