using System;
//using CommonLibrary;

namespace Model.MvvmMessages
{
	// Token: 0x02000095 RID: 149
	public class mvvmDebugMessage
	{
		// Token: 0x17000190 RID: 400
		// (get) Token: 0x0600048E RID: 1166 RVA: 0x0000B2DD File Offset: 0x000094DD
		// (set) Token: 0x0600048F RID: 1167 RVA: 0x0000B2E5 File Offset: 0x000094E5
		public DebugRecord Record { get; set; }

		// Token: 0x06000490 RID: 1168 RVA: 0x0000B2EE File Offset: 0x000094EE
		public mvvmDebugMessage(DebugRecord record)
		{
			this.Record = record;
		}
	}
}
