using System;
using Model.Commands;
using Model.DataService;

namespace Model.MvvmMessages
{
	// Token: 0x020000A3 RID: 163
	public class mvvmGetTimeResponse : mvvmMessage
	{
		// Token: 0x1700019F RID: 415
		// (get) Token: 0x060004BC RID: 1212 RVA: 0x0000B519 File Offset: 0x00009719
		// (set) Token: 0x060004BD RID: 1213 RVA: 0x0000B521 File Offset: 0x00009721
		public DateTime Time { get; set; }

		// Token: 0x060004BE RID: 1214 RVA: 0x0000B52A File Offset: 0x0000972A
		public mvvmGetTimeResponse(SentryDataService dataService, GetTimeResponse resp) : base(dataService)
		{
			this.Time = resp.Time;
		}
	}
}
