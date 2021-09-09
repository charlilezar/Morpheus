using System;
using Model.DataService;

namespace Model.MvvmMessages
{
	// Token: 0x02000093 RID: 147
	public class mvvmMessage
	{
		// Token: 0x1700018D RID: 397
		// (get) Token: 0x06000486 RID: 1158 RVA: 0x0000B285 File Offset: 0x00009485
		// (set) Token: 0x06000487 RID: 1159 RVA: 0x0000B28D File Offset: 0x0000948D
		public SentryDataService DataService { get; private set; }

		// Token: 0x06000488 RID: 1160 RVA: 0x0000B296 File Offset: 0x00009496
		public mvvmMessage(SentryDataService dataService)
		{
			this.DataService = dataService;
		}
	}
}
