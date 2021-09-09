using System;
using Model.Commands;
using Model.DataService;

namespace Model.MvvmMessages
{
	// Token: 0x020000A7 RID: 167
	public class mvvmEventQueryResponse : mvvmMessage
	{
		// Token: 0x170001A5 RID: 421
		// (get) Token: 0x060004CC RID: 1228 RVA: 0x0000B5E6 File Offset: 0x000097E6
		// (set) Token: 0x060004CD RID: 1229 RVA: 0x0000B5EE File Offset: 0x000097EE
		public SqlEventQueryResponse Response { get; private set; }

		// Token: 0x060004CE RID: 1230 RVA: 0x0000B5F7 File Offset: 0x000097F7
		public mvvmEventQueryResponse(SentryDataService dataService, SqlEventQueryResponse resp) : base(dataService)
		{
			this.Response = resp;
		}
	}
}
