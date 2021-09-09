using System;
using Model.Commands;
using Model.DataService;

namespace Model.MvvmMessages
{
	// Token: 0x02000096 RID: 150
	public class mvvmCommandStatusResponse : mvvmMessage
	{
		// Token: 0x17000191 RID: 401
		// (get) Token: 0x06000491 RID: 1169 RVA: 0x0000B2FD File Offset: 0x000094FD
		// (set) Token: 0x06000492 RID: 1170 RVA: 0x0000B305 File Offset: 0x00009505
		public CommandStatusResponse Response { get; private set; }

		// Token: 0x06000493 RID: 1171 RVA: 0x0000B30E File Offset: 0x0000950E
		public mvvmCommandStatusResponse(SentryDataService dataService, CommandStatusResponse response) : base(dataService)
		{
			this.Response = response;
		}
	}
}
