using System;

namespace Model.Commands
{
	// Token: 0x0200000F RID: 15
	public class ClearPerformanceHistogram
	{
		// Token: 0x17000014 RID: 20
		// (get) Token: 0x0600003F RID: 63 RVA: 0x000022F9 File Offset: 0x000004F9
		// (set) Token: 0x06000040 RID: 64 RVA: 0x00002301 File Offset: 0x00000501
		public string Name { get; set; }

		// Token: 0x06000041 RID: 65 RVA: 0x0000230A File Offset: 0x0000050A
		public ClearPerformanceHistogram()
		{
		}

		// Token: 0x06000042 RID: 66 RVA: 0x00002312 File Offset: 0x00000512
		public ClearPerformanceHistogram(string name)
		{
			this.Name = name;
		}

		// Token: 0x0400001E RID: 30
		public const string XmlElementName = "ClearPerformanceHistogram";
	}
}
