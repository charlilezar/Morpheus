using System;
using System.Data;

namespace Model.Commands
{
	// Token: 0x0200001A RID: 26
	public class SqlQueryResponse
	{
		// Token: 0x17000028 RID: 40
		// (get) Token: 0x06000086 RID: 134 RVA: 0x0000273A File Offset: 0x0000093A
		// (set) Token: 0x06000085 RID: 133 RVA: 0x00002731 File Offset: 0x00000931
		public string Status { get; set; }

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x06000088 RID: 136 RVA: 0x0000274B File Offset: 0x0000094B
		// (set) Token: 0x06000087 RID: 135 RVA: 0x00002742 File Offset: 0x00000942
		public int QueryTimeMilliseconds { get; set; }

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x0600008A RID: 138 RVA: 0x0000275C File Offset: 0x0000095C
		// (set) Token: 0x06000089 RID: 137 RVA: 0x00002753 File Offset: 0x00000953
		public int SerializationTimeMilliseconds { get; set; }

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x0600008C RID: 140 RVA: 0x0000276D File Offset: 0x0000096D
		// (set) Token: 0x0600008B RID: 139 RVA: 0x00002764 File Offset: 0x00000964
		public DataSet Dataset { get; set; }

		// Token: 0x0400003D RID: 61
		public const string XmlElementName = "SqlQueryResponse";
	}
}
