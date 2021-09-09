using System;
using Model.Commands;
using Model.DataService;
using Model.Electrometer;

namespace Model.MvvmMessages
{
	// Token: 0x020000A0 RID: 160
	public class mvvmGetElectrometerConfigurationResponse : mvvmMessage
	{
		// Token: 0x1700019B RID: 411
		// (get) Token: 0x060004B2 RID: 1202 RVA: 0x0000B493 File Offset: 0x00009693
		// (set) Token: 0x060004B1 RID: 1201 RVA: 0x0000B48A File Offset: 0x0000968A
		public ElectrometerConfiguration Configuration { get; private set; }

		// Token: 0x060004B3 RID: 1203 RVA: 0x0000B49B File Offset: 0x0000969B
		public mvvmGetElectrometerConfigurationResponse(SentryDataService dataService, GetElectrometerConfigurationResponse resp) : base(dataService)
		{
			this.Configuration = resp.Configuration;
		}
	}
}
