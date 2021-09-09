using System;

namespace Model.MvvmMessages
{
	// Token: 0x02000094 RID: 148
	public class mvvmSentryMessage
	{
		// Token: 0x1700018E RID: 398
		// (get) Token: 0x06000489 RID: 1161 RVA: 0x0000B2A5 File Offset: 0x000094A5
		// (set) Token: 0x0600048A RID: 1162 RVA: 0x0000B2AD File Offset: 0x000094AD
		public object Source { get; set; }

		// Token: 0x1700018F RID: 399
		// (get) Token: 0x0600048B RID: 1163 RVA: 0x0000B2B6 File Offset: 0x000094B6
		// (set) Token: 0x0600048C RID: 1164 RVA: 0x0000B2BE File Offset: 0x000094BE
		public string Message { get; set; }

		// Token: 0x0600048D RID: 1165 RVA: 0x0000B2C7 File Offset: 0x000094C7
		public mvvmSentryMessage(object source, string message)
		{
			this.Source = source;
			this.Message = message;
		}
	}
}
