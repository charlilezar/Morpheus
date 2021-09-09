using System;

namespace Model.Commands
{
	// Token: 0x02000028 RID: 40
	public class EchoRequest
	{
		// Token: 0x1700005C RID: 92
		// (get) Token: 0x06000113 RID: 275 RVA: 0x00003183 File Offset: 0x00001383
		// (set) Token: 0x06000112 RID: 274 RVA: 0x0000317A File Offset: 0x0000137A
		public string EchoString { get; set; }

		// Token: 0x06000114 RID: 276 RVA: 0x0000318B File Offset: 0x0000138B
		public EchoRequest()
		{
		}

		// Token: 0x06000115 RID: 277 RVA: 0x00003193 File Offset: 0x00001393
		public EchoRequest(string value)
		{
			this.EchoString = value;
		}

		// Token: 0x0400007F RID: 127
		public const string XmlElementName = "EchoRequest";
	}
}
