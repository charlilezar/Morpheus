using System;

namespace Model.Commands
{
	// Token: 0x02000010 RID: 16
	public class GetConfiguration
	{
		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000044 RID: 68 RVA: 0x0000232A File Offset: 0x0000052A
		// (set) Token: 0x06000043 RID: 67 RVA: 0x00002321 File Offset: 0x00000521
		public string Name { get; set; }

		// Token: 0x06000045 RID: 69 RVA: 0x00002332 File Offset: 0x00000532
		public GetConfiguration()
		{
			this.Name = "";
		}

		// Token: 0x06000046 RID: 70 RVA: 0x00002345 File Offset: 0x00000545
		public GetConfiguration(string name)
		{
			this.Name = name;
		}

		// Token: 0x04000020 RID: 32
		public const string XmlElementName = "GetConfiguration";
	}
}
