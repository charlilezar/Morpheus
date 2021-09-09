using System;
using Model.Commands;
using Model.DataService;

namespace Model.MvvmMessages
{
	// Token: 0x020000A8 RID: 168
	public class mvvmGetDatabaseSummaryResponse : mvvmMessage
	{
		// Token: 0x170001A6 RID: 422
		// (get) Token: 0x060004D0 RID: 1232 RVA: 0x0000B610 File Offset: 0x00009810
		// (set) Token: 0x060004CF RID: 1231 RVA: 0x0000B607 File Offset: 0x00009807
		public GetDatabaseSummaryResponse Response { get; private set; }

		// Token: 0x060004D1 RID: 1233 RVA: 0x0000B618 File Offset: 0x00009818
		public mvvmGetDatabaseSummaryResponse(SentryDataService dataService, GetDatabaseSummaryResponse response) : base(dataService)
		{
			this.Response = response;
		}
	}
}
