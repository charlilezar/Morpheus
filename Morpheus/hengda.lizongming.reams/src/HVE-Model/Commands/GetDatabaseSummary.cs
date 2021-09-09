using System;

namespace Model.Commands
{
	// Token: 0x0200001C RID: 28
	public class GetDatabaseSummary
	{
		// Token: 0x17000036 RID: 54
		// (get) Token: 0x060000A4 RID: 164 RVA: 0x0000285C File Offset: 0x00000A5C
		// (set) Token: 0x060000A3 RID: 163 RVA: 0x00002853 File Offset: 0x00000A53
		public bool IncludeCount { get; set; }

		// Token: 0x060000A5 RID: 165 RVA: 0x00002864 File Offset: 0x00000A64
		public GetDatabaseSummary()
		{
			this.IncludeCount = false;
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x00002873 File Offset: 0x00000A73
		public GetDatabaseSummary(bool includeCount)
		{
			this.IncludeCount = includeCount;
		}

		// Token: 0x0400004D RID: 77
		public const string XmlElementName = "GetDatabaseSummary";
	}
}
