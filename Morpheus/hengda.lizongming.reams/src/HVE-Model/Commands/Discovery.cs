using System;

namespace Model.Commands
{
	// Token: 0x0200002B RID: 43
	public class Discovery
	{
		// Token: 0x17000060 RID: 96
		// (get) Token: 0x06000122 RID: 290 RVA: 0x00003244 File Offset: 0x00001444
		// (set) Token: 0x06000121 RID: 289 RVA: 0x0000323B File Offset: 0x0000143B
		public string CommChannelName { get; set; }

		// Token: 0x06000123 RID: 291 RVA: 0x0000324C File Offset: 0x0000144C
		public Discovery()
		{
			this.CommChannelName = "Unknown";
		}

		// Token: 0x06000124 RID: 292 RVA: 0x0000325F File Offset: 0x0000145F
		public Discovery(string channelName)
		{
			this.CommChannelName = channelName;
		}

		// Token: 0x04000086 RID: 134
		public const string XmlElementName = "Discovery";
	}
}
