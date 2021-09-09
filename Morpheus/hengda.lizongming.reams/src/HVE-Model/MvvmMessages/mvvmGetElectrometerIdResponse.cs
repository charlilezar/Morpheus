using System;
using Model.Commands;
using Model.DataService;

namespace Model.MvvmMessages
{
	// Token: 0x020000A1 RID: 161
	public class mvvmGetElectrometerIdResponse : mvvmMessage
	{
		// Token: 0x1700019C RID: 412
		// (get) Token: 0x060004B5 RID: 1205 RVA: 0x0000B4B9 File Offset: 0x000096B9
		// (set) Token: 0x060004B4 RID: 1204 RVA: 0x0000B4B0 File Offset: 0x000096B0
		public string SerialNumber { get; private set; }

		// Token: 0x1700019D RID: 413
		// (get) Token: 0x060004B7 RID: 1207 RVA: 0x0000B4CA File Offset: 0x000096CA
		// (set) Token: 0x060004B6 RID: 1206 RVA: 0x0000B4C1 File Offset: 0x000096C1
		public string Revision { get; private set; }

		// Token: 0x060004B8 RID: 1208 RVA: 0x0000B4D2 File Offset: 0x000096D2
		public mvvmGetElectrometerIdResponse(SentryDataService dataService, GetElectrometerIdResponse resp) : base(dataService)
		{
			this.SerialNumber = resp.SerialNumber;
			this.Revision = resp.Revision;
		}
	}
}
