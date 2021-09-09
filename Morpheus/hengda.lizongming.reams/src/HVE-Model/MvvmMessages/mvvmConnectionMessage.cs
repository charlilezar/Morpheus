using System;
using Model.DataService;

namespace Model.MvvmMessages
{
	// Token: 0x020000AA RID: 170
	public class mvvmConnectionMessage
	{
		// Token: 0x170001A8 RID: 424
		// (get) Token: 0x060004D5 RID: 1237 RVA: 0x0000B649 File Offset: 0x00009849
		// (set) Token: 0x060004D6 RID: 1238 RVA: 0x0000B651 File Offset: 0x00009851
		public SentryDataService DataService { get; private set; }

		// Token: 0x170001A9 RID: 425
		// (get) Token: 0x060004D7 RID: 1239 RVA: 0x0000B65A File Offset: 0x0000985A
		// (set) Token: 0x060004D8 RID: 1240 RVA: 0x0000B662 File Offset: 0x00009862
		public mvvmConnectionMessage.ConnectedStatus Status { get; private set; }

		// Token: 0x060004D9 RID: 1241 RVA: 0x0000B66B File Offset: 0x0000986B
		public mvvmConnectionMessage(SentryDataService dataService, mvvmConnectionMessage.ConnectedStatus status)
		{
			this.DataService = dataService;
			this.Status = status;
		}

		// Token: 0x020000AB RID: 171
		public enum ConnectedStatus
		{
			// Token: 0x04000333 RID: 819
			Connected,
			// Token: 0x04000334 RID: 820
			Closed,
			// Token: 0x04000335 RID: 821
			Disconnected
		}
	}
}
