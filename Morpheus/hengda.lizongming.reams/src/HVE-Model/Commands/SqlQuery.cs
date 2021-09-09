using System;

namespace Model.Commands
{
	// Token: 0x02000019 RID: 25
	public class SqlQuery
	{
		// Token: 0x17000027 RID: 39
		// (get) Token: 0x06000082 RID: 130 RVA: 0x00002712 File Offset: 0x00000912
		// (set) Token: 0x06000081 RID: 129 RVA: 0x00002709 File Offset: 0x00000909
		public string Query { get; set; }

		// Token: 0x06000083 RID: 131 RVA: 0x0000271A File Offset: 0x0000091A
		public SqlQuery()
		{
		}

		// Token: 0x06000084 RID: 132 RVA: 0x00002722 File Offset: 0x00000922
		public SqlQuery(string sqlQuery)
		{
			this.Query = sqlQuery;
		}

		// Token: 0x0400003B RID: 59
		public const string XmlElementName = "SqlQuery";
	}
}
