using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aming.Core;

namespace HengDa.LiZongMing.REAMS.Wpf.Dto
{
    public class XPHBaseDto : BaseDto
    {
        /// <summary>
        /// 命令值
        /// </summary>
        [Description("命令值")]
        public HDZCQCmdCode Cmd { get; set; }


        //40001	2	通道1(风速)
        //40002	2	通道2（雨量）
        //40003	2	通道3（温度）
        //40004	2	通道4（气压）
        //40005	2	通道5
        //40006	2	通道6（辐射）
        //40007	2	通道7（风向）
        //40008	2	通道8
        //40009	2	通道9（湿度）

        public DateTime RecordTime { get; set; }
        public short 风速 { get; set; }
        public short 雨量 { get; set; }
        public short 温度 { get; set; }
        public short 气压 { get; set; }
        public short _5 { get; set; }
        public short 辐射 { get; set; }
        public short 风向 { get; set; }
        public short _8 { get; set; }
        public short 湿度 { get; set; }
        public short _10 { get; set; }
        public short _11 { get; set; }
        public short _12 { get; set; }
        public short _13 { get; set; }
        public short _14 { get; set; }
        public short _15 { get; set; }
        public short _16 { get; set; }

         public XPHBaseDto(byte[] bytes, int startIndex, int length)
        {
            /*
             *2	通道1 (风速)
2	通道2（雨量）
2	通道3（温度）
2	通道4（气压）
2	通道5
2	通道6（辐射）
2	通道7（风向）
2	通道8
2	通道9（湿度）
2	通道10
2	通道11
2	通道12
2	通道13
2	通道14
2	通道15
2	通道16
            */
            风速 = StringHelper.BytesToShort(bytes, startIndex + 0);
            雨量 = StringHelper.BytesToShort(bytes, startIndex + 2);
            温度 = StringHelper.BytesToShort(bytes, startIndex + 4);
            气压 = StringHelper.BytesToShort(bytes, startIndex + 6);
            _5 = StringHelper.BytesToShort(bytes, startIndex + 8);
            辐射 = StringHelper.BytesToShort(bytes, startIndex + 10);
            风向 = StringHelper.BytesToShort(bytes, startIndex + 12);
            _8 = StringHelper.BytesToShort(bytes, startIndex + 14);
            湿度 = StringHelper.BytesToShort(bytes, startIndex + 16);
            _10 = StringHelper.BytesToShort(bytes, startIndex + 18);
            _11 = StringHelper.BytesToShort(bytes, startIndex + 20);
            _12 = StringHelper.BytesToShort(bytes, startIndex + 22);
            _13 = StringHelper.BytesToShort(bytes, startIndex + 24);
            _14 = StringHelper.BytesToShort(bytes, startIndex + 26);
            _15 = StringHelper.BytesToShort(bytes, startIndex + 28);
            _16 = StringHelper.BytesToShort(bytes, startIndex + 30);



        }

    }

}
