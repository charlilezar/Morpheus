using System;
using Model.Commands;
using Model.DataService;
using Model.Events;

namespace Model.MvvmMessages
{
	// Token: 0x02000098 RID: 152
	public class mvvmSentinelState : mvvmMessage
	{
		// Token: 0x17000193 RID: 403
		// (get) Token: 0x06000498 RID: 1176 RVA: 0x0000B354 File Offset: 0x00009554
		// (set) Token: 0x06000499 RID: 1177 RVA: 0x0000B35C File Offset: 0x0000955C
		public SentinelState State { get; private set; }

		// Token: 0x0600049A RID: 1178 RVA: 0x0000B365 File Offset: 0x00009565
		public mvvmSentinelState(SentryDataService dataService, SentinelState state) : base(dataService)
		{
			this.State = state;
		}

		// Token: 0x0600049B RID: 1179 RVA: 0x0000B375 File Offset: 0x00009575
		public mvvmSentinelState(SentryDataService dataService, EventResponse resp) : base(dataService)
		{
			this.State = resp.State;
		}
	}
}
