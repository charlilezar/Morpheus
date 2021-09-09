using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Text;

namespace HengDa.LiZongMing.REAMS.Iot
{
    [AddINotifyPropertyChangedInterface]
    public class QingdianLog
    {
        /// <summary>
        /// 工况
        /// </summary>
        public decimal gongkuang { get; set; }
    }
}
