using System;
using Model.Commands;
using Model.Configurations;
using Model.DataService;

namespace Model.MvvmMessages
{
	// Token: 0x0200009E RID: 158
	public class mvvmGetConfigurationResponse : mvvmMessage
	{
		// Token: 0x17000199 RID: 409
		// (get) Token: 0x060004AB RID: 1195 RVA: 0x0000B43E File Offset: 0x0000963E
		// (set) Token: 0x060004AC RID: 1196 RVA: 0x0000B446 File Offset: 0x00009646
		public Configuration Config { get; private set; }

		// Token: 0x060004AD RID: 1197 RVA: 0x0000B44F File Offset: 0x0000964F
		public mvvmGetConfigurationResponse(SentryDataService dataService, GetConfigurationResponse resp) : base(dataService)
		{
			this.Config = resp.Config;
		}
	}
}
