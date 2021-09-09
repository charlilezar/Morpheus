using System;

namespace Model.Commands
{
	// Token: 0x0200001E RID: 30
	public class DeleteDatabaseRecords
	{
		// Token: 0x060000AF RID: 175 RVA: 0x000028DA File Offset: 0x00000ADA
		public DeleteDatabaseRecords()
		{
		}

		// Token: 0x060000B0 RID: 176 RVA: 0x000028E2 File Offset: 0x00000AE2
		public DeleteDatabaseRecords(string table, DateTime olderThanTime, bool compact)
		{
			this.TableName = table;
			this.Time = olderThanTime;
			this.Compact = compact;
		}

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x060000B1 RID: 177 RVA: 0x000028FF File Offset: 0x00000AFF
		// (set) Token: 0x060000B2 RID: 178 RVA: 0x00002907 File Offset: 0x00000B07
		public string TableName { get; set; }

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x060000B3 RID: 179 RVA: 0x00002910 File Offset: 0x00000B10
		// (set) Token: 0x060000B4 RID: 180 RVA: 0x00002918 File Offset: 0x00000B18
		public DateTime Time { get; set; }

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x060000B5 RID: 181 RVA: 0x00002921 File Offset: 0x00000B21
		// (set) Token: 0x060000B6 RID: 182 RVA: 0x00002929 File Offset: 0x00000B29
		public bool Compact { get; set; }

		// Token: 0x04000053 RID: 83
		public const string XmlElementName = "DeleteDatabaseRecords";
	}
}
