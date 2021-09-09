using System;
using Model.Commands;
using Model.DataService;

namespace Model.MvvmMessages
{
	// Token: 0x020000A9 RID: 169
	public class mvvmTestResponse : mvvmMessage
	{
		// Token: 0x170001A7 RID: 423
		// (get) Token: 0x060004D3 RID: 1235 RVA: 0x0000B631 File Offset: 0x00009831
		// (set) Token: 0x060004D2 RID: 1234 RVA: 0x0000B628 File Offset: 0x00009828
		public TestResponse Response { get; private set; }

		// Token: 0x060004D4 RID: 1236 RVA: 0x0000B639 File Offset: 0x00009839
		public mvvmTestResponse(SentryDataService dataService, TestResponse response) : base(dataService)
		{
			this.Response = response;
		}
	}
}
