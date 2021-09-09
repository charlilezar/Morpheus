using System;
using Model.Commands;
using Model.DataService;
using Model.Electrometer;

namespace Model.MvvmMessages
{
	// Token: 0x020000A4 RID: 164
	public class mvvmGetHpicConfigurationResponse : mvvmMessage
	{
		// Token: 0x170001A0 RID: 416
		// (get) Token: 0x060004BF RID: 1215 RVA: 0x0000B53F File Offset: 0x0000973F
		// (set) Token: 0x060004C0 RID: 1216 RVA: 0x0000B547 File Offset: 0x00009747
		public HpicConfiguration HpicConfiguration { get; private set; }

		// Token: 0x060004C1 RID: 1217 RVA: 0x0000B550 File Offset: 0x00009750
		public mvvmGetHpicConfigurationResponse(SentryDataService dataService, GetHpicConfigurationResponse resp) : base(dataService)
		{
			this.HpicConfiguration = resp.HpicConfiguration;
		}
	}
}
