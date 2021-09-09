using System;

namespace Model.Commands
{
	// Token: 0x02000003 RID: 3
	public class NotifyEnable
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000004 RID: 4 RVA: 0x00002058 File Offset: 0x00000258
		// (set) Token: 0x06000005 RID: 5 RVA: 0x00002060 File Offset: 0x00000260
		public int NotifyPort { get; set; }

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000006 RID: 6 RVA: 0x00002069 File Offset: 0x00000269
		// (set) Token: 0x06000007 RID: 7 RVA: 0x00002071 File Offset: 0x00000271
		public bool Enable { get; set; }

		// Token: 0x06000008 RID: 8 RVA: 0x0000207A File Offset: 0x0000027A
		public NotifyEnable()
		{
			this.Enable = false;
			this.NotifyPort = 0;
		}

		// Token: 0x06000009 RID: 9 RVA: 0x00002090 File Offset: 0x00000290
		public NotifyEnable(bool enable, int port)
		{
			this.Enable = enable;
			this.NotifyPort = port;
		}

		// Token: 0x04000001 RID: 1
		public const string XmlElementName = "NotifyEnable";
	}
}
