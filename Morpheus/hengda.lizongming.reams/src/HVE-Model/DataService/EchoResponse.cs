using System;

namespace Model.DataService
{
	// Token: 0x02000079 RID: 121
	public class EchoResponse
	{
		// Token: 0x170000EB RID: 235
		// (get) Token: 0x06000338 RID: 824 RVA: 0x00008F5A File Offset: 0x0000715A
		// (set) Token: 0x06000339 RID: 825 RVA: 0x00008F62 File Offset: 0x00007162
		public string EchoString { get; set; }

		// Token: 0x0600033A RID: 826 RVA: 0x00008F6B File Offset: 0x0000716B
		public EchoResponse()
		{
		}

		// Token: 0x0600033B RID: 827 RVA: 0x00008F73 File Offset: 0x00007173
		public EchoResponse(string echoString) : this()
		{
			this.EchoString = echoString;
		}
	}
}
