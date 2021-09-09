using Aming.Core;
using Aming.DTU;
using Aming.DTU.Config;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Connecting; 
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using MQTTnet.Client.Receiving;
using MQTTnet.Protocol;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Threading;

namespace Aming.DTU
{
    #region 相关delegate
    public delegate void ReceivedUniMessageCallBack(IDataEndPoint config, UniMessageBase msg);

    #endregion

}
