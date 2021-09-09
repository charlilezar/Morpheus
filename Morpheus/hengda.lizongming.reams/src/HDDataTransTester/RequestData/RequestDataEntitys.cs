using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HDDataTransTester.RequestData.RequestDataEntitys
{
    public class RequestDataFilter
    {
        public RequestDataFilter(long sDeviceID,DateTime detStart)
        {
            this.DeviceId = sDeviceID;
            this.TimeStart = detStart;
        }
        /// <summary>
        /// 起始时间，不能为空，这个根据我们要查询的数据的CTime值来确定
        /// </summary>
        public DateTime TimeStart { get; set; }
        /// <summary>
        /// 结束时间，一般为空，当我们要补数据之前的时，可能会用到
        /// </summary>
        public DateTime? TimeEnd { get; set; }
        /// <summary>
        /// 设备ID号，这个不能为空
        /// </summary>
        public long DeviceId { get; set; }
        /// <summary>
        /// 读取数据的排序方式，默认就是CreationTime DESC，除非没这个字段，这样要要自定义。也就是说该字段不能为空
        /// </summary>
        public string Sorting { get; set; } = "CreationTime DESC";
        /// <summary>
        /// 租户ID，可以不传入，直接在http请求时自动写入
        /// </summary>
        public string TenantId { get; set; }
        /// <summary>
        /// 数据跳跃值，如果是1，则不跳跃，2的话中间挑开一条数据；默认为1
        /// </summary>
        public int SkipCount { get; set; } = 1;
        /// <summary>
        /// 读取最大数量，默认为1条
        /// </summary>
        public int MaxResultCount { get; set; } = 1;
    }
}
