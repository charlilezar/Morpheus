using HengDa.LiZongMing.REAMS.Wpf.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HengDa.LiZongMing.REAMS.Dto
{
    public class HDRBaseDto : BaseDto
    {
        /// <summary>
        /// 命令值
        /// </summary>
        [Description("命令值")]
        public HDRNSCmdCode Cmd { get; set; }
    }
    public class HDRRealDataDto
    {
        public HDRRealDataDto()
        {

        }
        public HDRRealDataDto(HDRCmdCode cmd,ushort iMac,byte[] bs)
        {

        }
        #region 属性字段
        /// <summary>
        /// 累计降雨量
        /// </summary>
        [Description("累计降雨量[单位：？]")]
        public decimal  TotalRainfall { get; set; }
        /// <summary>
        /// 上一场降雨量
        /// </summary>
        [Description("上一场降雨量[单位：？]")]
        public decimal LastRainfall { get; set; }
        /// <summary>
        /// 本场降雨量
        /// </summary>
        [Description("本场降雨量[单位：？]")]
        public decimal CurrentRainfall { get; set; }
        #endregion
    }

}
