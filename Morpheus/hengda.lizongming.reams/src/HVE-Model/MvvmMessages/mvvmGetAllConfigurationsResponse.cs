using System;
using System.Collections.Generic;
using Model.Commands;
using Model.Configurations;
using Model.DataService;

namespace Model.MvvmMessages
{
	// Token: 0x0200009D RID: 157
	public class mvvmGetAllConfigurationsResponse : mvvmMessage
	{
		// Token: 0x17000198 RID: 408
		// (get) Token: 0x060004A8 RID: 1192 RVA: 0x0000B418 File Offset: 0x00009618
		// (set) Token: 0x060004A9 RID: 1193 RVA: 0x0000B420 File Offset: 0x00009620
		public List<Configuration> ConfigurationList { get; private set; }

		// Token: 0x060004AA RID: 1194 RVA: 0x0000B429 File Offset: 0x00009629
		public mvvmGetAllConfigurationsResponse(SentryDataService dataService, GetAllConfigurationsResponse resp) : base(dataService)
		{
			this.ConfigurationList = resp.ConfigurationList;
		}
	}
}
